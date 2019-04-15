﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllFeaturesRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllFeaturesRuleTemplate : RuleTemplate<IClass, AllFeaturesRuleTemplate>, IAllFeaturesRuleTemplate
    {
        #region Init
        static AllFeaturesRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IClass, IClassType>(nameof(IClass.ResolvedClassType)),
                new SealedTableSourceTemplate<IClass, IFeatureName, IFeatureInstance>(nameof(IClass.LocalFeatureTable)),
                new OnceReferenceCollectionSourceTemplate<IClass, ICompiledFeature>(nameof(IClass.FeatureList), nameof(IFeature.ResolvedFeature)),
                new OnceReferenceCollectionSourceTemplate<IClass, IClassType>(nameof(IClass.InheritanceList), nameof(IInheritance.ResolvedType)),
                new OnceReferenceCollectionSourceTemplate<IClass, IHashtableEx<IFeatureName, IFeatureInstance>>(nameof(IClass.InheritanceList), nameof(IInheritance.FeatureTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, IFeatureName, IFeatureInstance>(nameof(IClass.FeatureTable)),
            };
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        /// <param name="dataList">Optional data collected during inspection of sources.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        public override bool CheckConsistency(IClass node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IList<AncestorFeatureInfo> FeatureTableList = new List<AncestorFeatureInfo>();
            ListLocalAndInheritedFeatures(node, FeatureTableList);

            IHashtableEx<ICompiledFeature, IList<InstanceNameInfo>> ByFeatureTable; // ICompiledFeature -> List of InstanceNameInfo
            IHashtableEx<IFeatureName, InheritedInstanceInfo> ByNameTable; // FeatureName -> InheritedInstanceInfo
            SortByFeatureAndByName(FeatureTableList, out ByFeatureTable, out ByNameTable);

            if (!CheckInheritanceConsistency(ByFeatureTable, ByNameTable, node.ResolvedClassType.Item, ErrorList))
                Success = false;
            else if (!CheckAllPrecursorSelected(ByNameTable, ErrorList))
                Success = false;
            else if (!CheckPrecursorBodiesHaveAncestor(ByNameTable, ErrorList))
                Success = false;
            else
            {
                MergeInheritedFeatures(node, ByNameTable, out IHashtableEx<IFeatureName, IFeatureInstance> MergedFeatureTable);
                ClassType.MergeConformingParentTypes(node, node.ResolvedClassType.Item, ErrorList);
            }

            return Success;
        }

        private void ListLocalAndInheritedFeatures(IClass item, IList<AncestorFeatureInfo> featureTableList)
        {
            AncestorFeatureInfo LocalFeatures = new AncestorFeatureInfo();
            LocalFeatures.Ancestor = item.ResolvedClassType.Item;
            LocalFeatures.FeatureTable = item.LocalFeatureTable;
            LocalFeatures.Location = new OnceReference<IInheritance>();
            featureTableList.Add(LocalFeatures);

            foreach (IInheritance InheritanceItem in item.InheritanceList)
            {
                AncestorFeatureInfo InheritedFeatures = new AncestorFeatureInfo();
                IClassType InheritanceParent = InheritanceItem.ResolvedType.Item;
                InheritedFeatures.Ancestor = InheritanceParent;
                InheritedFeatures.FeatureTable = InheritanceParent.FeatureTable;
                InheritedFeatures.Location = new OnceReference<IInheritance>();
                InheritedFeatures.Location.Item = InheritanceItem;
                featureTableList.Add(InheritedFeatures);
            }
        }

        private void SortByFeatureAndByName(IList<AncestorFeatureInfo> featureTableList, out IHashtableEx<ICompiledFeature, IList<InstanceNameInfo>> byFeatureTable, out IHashtableEx<IFeatureName, InheritedInstanceInfo> byNameTable)
        {
            byFeatureTable = new HashtableEx<ICompiledFeature, IList<InstanceNameInfo>>(); // ICompiledFeature -> List of InstanceNameInfo
            byNameTable = new HashtableEx<IFeatureName, InheritedInstanceInfo>(); // FeatureName -> InheritedInstanceInfo

            bool FeatureAlreadyListed;
            bool NameAlreadyListed;

            foreach (AncestorFeatureInfo FeatureInfoItem in featureTableList)
            {
                foreach (KeyValuePair<IFeatureName, IFeatureInstance> Entry in FeatureInfoItem.FeatureTable)
                {
                    IFeatureName Key = Entry.Key;
                    IFeatureInstance Value = Entry.Value;

                    if (Value.Feature.IsAssigned)
                    {
                        FeatureAlreadyListed = false;
                        NameAlreadyListed = false;
                        foreach (KeyValuePair<ICompiledFeature, IList<InstanceNameInfo>> ImportedEntry in byFeatureTable)
                        {
                            ICompiledFeature ImportedKey = ImportedEntry.Key;
                            IList<InstanceNameInfo> NameList = ImportedEntry.Value;

                            // Feature already listed
                            if (Value.Feature.Item == ImportedKey)
                            {
                                OnceReference<InstanceNameInfo> PreviousInstance = new OnceReference<InstanceNameInfo>();

                                int i;
                                for (i = 0; i < NameList.Count; i++)
                                {
                                    InstanceNameInfo Item = NameList[i];
                                    if (Key.Name == Item.Name.Name)
                                    {
                                        PreviousInstance.Item = Item;
                                        break;
                                    }
                                }

                                // C inherit f from A and B, effectively or not, but keep or discontinue flags don't match.
                                if (PreviousInstance.IsAssigned && (PreviousInstance.Item.Instance.IsForgotten == Value.IsForgotten))
                                {
                                    PreviousInstance.Item.SameIsKept = PreviousInstance.Item.Instance.IsKept == Value.IsKept;
                                    PreviousInstance.Item.SameIsDiscontinued = PreviousInstance.Item.Instance.IsDiscontinued == Value.IsDiscontinued;
                                }

                                if (!PreviousInstance.IsAssigned || (PreviousInstance.Item.Instance.IsForgotten && !Value.IsForgotten))
                                {
                                    InstanceNameInfo NewInfo = new InstanceNameInfo(FeatureInfoItem, Value, Key);
                                    if (i < NameList.Count)
                                        NameList[i] = NewInfo;
                                    else
                                        NameList.Add(NewInfo);
                                }

                                FeatureAlreadyListed = true;
                                break;
                            }
                        }
                        if (!FeatureAlreadyListed)
                        {
                            IList<InstanceNameInfo> InitList = new List<InstanceNameInfo>();
                            InstanceNameInfo NewInfo = new InstanceNameInfo(FeatureInfoItem, Value, Key);
                            InitList.Add(NewInfo);

                            byFeatureTable.Add(Value.Feature.Item, InitList);
                        }
                    }

                    FeatureAlreadyListed = false;
                    NameAlreadyListed = false;
                    foreach (KeyValuePair<IFeatureName, InheritedInstanceInfo> ImportedEntry in byNameTable)
                    {
                        IFeatureName ImportedKey = ImportedEntry.Key;
                        InheritedInstanceInfo ImportedInstance = ImportedEntry.Value;
                        IList<InstanceNameInfo> InstanceList = ImportedInstance.PrecursorInstanceList;

                        if (Key.Name == ImportedKey.Name)
                        {
                            FeatureAlreadyListed = false;

                            if (Value.Feature.IsAssigned)
                                foreach (InstanceNameInfo Item in InstanceList)
                                    if (Item.Instance.Feature.IsAssigned && Value.Feature.Item == Item.Instance.Feature.Item)
                                    {
                                        FeatureAlreadyListed = true;
                                        break;
                                    }

                            if (!FeatureAlreadyListed)
                            {
                                InstanceNameInfo NewInfo = new InstanceNameInfo(FeatureInfoItem, Value, Key);
                                InstanceList.Add(NewInfo);
                            }

                            NameAlreadyListed = true;
                            break;
                        }
                    }
                    if (!NameAlreadyListed)
                    {
                        IList<InstanceNameInfo> InitList = new List<InstanceNameInfo>();
                        InstanceNameInfo NewInfo = new InstanceNameInfo(FeatureInfoItem, Value, Key);
                        InitList.Add(NewInfo);

                        InheritedInstanceInfo NewName = new InheritedInstanceInfo();
                        NewName.PrecursorInstanceList = InitList;

                        byNameTable.Add(Key, NewName);
                    }
                }
            }
        }

        private bool CheckInheritanceConsistency(IHashtableEx<ICompiledFeature, IList<InstanceNameInfo>> byFeatureTable, IHashtableEx<IFeatureName, InheritedInstanceInfo> byNameTable, IClassType localClassType, IList<IError> errorList)
        {
            bool AllFlagsTheSame = true;
            foreach (KeyValuePair<ICompiledFeature, IList<InstanceNameInfo>> ImportedEntry in byFeatureTable)
            {
                IList<InstanceNameInfo> NameList = ImportedEntry.Value;

                foreach (InstanceNameInfo Item in NameList)
                    if (!Item.SameIsKept || !Item.SameIsDiscontinued)
                    {
                        // C inherit f from A and B, effectively or not, but keep or discontinue flags don't match.
                        AllFlagsTheSame = false;
                        errorList.Add(new ErrorInheritanceConflict(Item.Location, Item.Name.Name));
                        break;
                    }
            }
            if (!AllFlagsTheSame)
                return false;

            bool MultipleEffective = false;
            foreach (KeyValuePair<IFeatureName, InheritedInstanceInfo> ImportedEntry in byNameTable)
            {
                IFeatureName ImportedKey = ImportedEntry.Key;
                InheritedInstanceInfo ImportedInstance = ImportedEntry.Value;
                IList<InstanceNameInfo> InstanceList = ImportedInstance.PrecursorInstanceList;

                foreach (InstanceNameInfo Item in InstanceList)
                    if (!Item.Instance.IsForgotten)
                    {
                        if (!ImportedInstance.EffectiveInstance.IsAssigned)
                            ImportedInstance.EffectiveInstance.Item = Item;

                        else
                        {
                            errorList.Add(new ErrorMultipleEffectiveFeature(Item.Location, Item.Name.Name));
                            MultipleEffective = true;
                            break;
                        }
                    }
            }
            if (MultipleEffective)
                return false;

            bool AllRedefineConformant = true;

            AllFlagsTheSame = true;
            foreach (KeyValuePair<IFeatureName, InheritedInstanceInfo> ImportedEntry in byNameTable)
            {
                IFeatureName ImportedKey = ImportedEntry.Key;
                InheritedInstanceInfo ImportedInstance = ImportedEntry.Value;

                // If there is no effective instance for this name
                if (!ImportedInstance.EffectiveInstance.IsAssigned)
                {
                    IList<InstanceNameInfo> InstanceList = ImportedInstance.PrecursorInstanceList;
                    ImportedInstance.IsKept = InstanceList[0].Instance.IsKept;
                    ImportedInstance.IsDiscontinued = InstanceList[0].Instance.IsDiscontinued;

                    if (InstanceList.Count > 1)
                    {
                        ICompiledType FeatureType = InstanceList[0].Instance.Feature.Item.ResolvedFeatureType.Item;

                        for (int i = 1; i < InstanceList.Count; i++)
                        {
                            InstanceNameInfo ThisInstance = InstanceList[i];

                            if (ImportedInstance.IsKept != ThisInstance.Instance.IsKept ||
                                ImportedInstance.IsDiscontinued != ThisInstance.Instance.IsDiscontinued ||
                                !ObjectType.TypesHaveIdenticalSignature(FeatureType, ThisInstance.Instance.Feature.Item.ResolvedFeatureType.Item))
                            {
                                errorList.Add(new ErrorInheritanceConflict(ThisInstance.Location, ThisInstance.Name.Name));
                                AllFlagsTheSame = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    ImportedInstance.IsKept = ImportedInstance.EffectiveInstance.Item.Instance.IsKept;
                    ImportedInstance.IsDiscontinued = ImportedInstance.EffectiveInstance.Item.Instance.IsDiscontinued;

                    // If the effective instance is a redefine.
                    if (ImportedInstance.EffectiveInstance.Item.Ancestor == localClassType && ImportedInstance.EffectiveInstance.Item.Instance.Feature.Item.ResolvedFeatureType.IsAssigned)
                    {
                        ICompiledType DescendantFeatureType = ImportedInstance.EffectiveInstance.Item.Instance.Feature.Item.ResolvedFeatureType.Item;

                        IList<InstanceNameInfo> InstanceList = ImportedInstance.PrecursorInstanceList;
                        foreach (InstanceNameInfo Item in InstanceList)
                        {
                            if (Item == ImportedInstance.EffectiveInstance.Item)
                                continue;

                            ICompiledType AncestorFeatureType = Item.Instance.Feature.Item.ResolvedFeatureType.Item;
                            IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();

                            if (!ObjectType.TypeConformToBase(DescendantFeatureType, AncestorFeatureType, SubstitutionTypeTable, errorList, (ISource)ImportedInstance.EffectiveInstance.Item.Instance.Feature.Item, true))
                            {
                                errorList.Add(new ErrorInheritanceConflict(Item.Location, Item.Name.Name));
                                AllRedefineConformant = false;
                            }
                        }
                    }
                }
            }
            if (!AllFlagsTheSame || !AllRedefineConformant)
                return false;

            return true;
        }

        private bool CheckAllPrecursorSelected(IHashtableEx<IFeatureName, InheritedInstanceInfo> byNameTable, IList<IError> errorList)
        {
            IList<IHashtableEx<IFeatureName, IList<ICompiledFeature>>> PrecursorSetList = new List<IHashtableEx<IFeatureName, IList<ICompiledFeature>>>(); // FeatureName -> List<ICompiledFeature> (Precursor list)

            foreach (KeyValuePair<IFeatureName, InheritedInstanceInfo> Entry in byNameTable)
            {
                IFeatureName Key = Entry.Key;
                InheritedInstanceInfo Value = Entry.Value;

                IList<ICompiledFeature> PrecursorList = new List<ICompiledFeature>();
                foreach (InstanceNameInfo PrecursorItem in Value.PrecursorInstanceList)
                    FillPrecursorList(PrecursorList, PrecursorItem.Instance);

                bool FoundInSet = false;
                foreach (IHashtableEx<IFeatureName, IList<ICompiledFeature>> PrecursorSet in PrecursorSetList)
                {
                    foreach (KeyValuePair<IFeatureName, IList<ICompiledFeature>> SetMemberEntry in PrecursorSet)
                    {
                        IFeatureName SetMemberKey = SetMemberEntry.Key;
                        IList<ICompiledFeature> SetMemberPrecursorList = SetMemberEntry.Value;

                        if (PrecursorListIntersect(PrecursorList, SetMemberPrecursorList))
                        {
                            PrecursorSet.Add(Key, PrecursorList);
                            FoundInSet = true;
                            break;
                        }
                    }
                    if (FoundInSet)
                        break;
                }
                if (!FoundInSet)
                {
                    IHashtableEx<IFeatureName, IList<ICompiledFeature>> NewSet = new HashtableEx<IFeatureName, IList<ICompiledFeature>>();
                    NewSet.Add(Key, PrecursorList);
                }
            }

            bool MissingKeptFlag = false;
            foreach (IHashtableEx<IFeatureName, IList<ICompiledFeature>> PrecursorSet in PrecursorSetList)
            {
                if (PrecursorSet.Count == 1)
                    continue;

                bool IsKept = false;
                foreach (KeyValuePair<IFeatureName, IList<ICompiledFeature>> SetMemberEntry in PrecursorSet)
                {
                    IFeatureName SetMemberKey = SetMemberEntry.Key;
                    InheritedInstanceInfo CorrespondingInstance = byNameTable[SetMemberKey];

                    if (CorrespondingInstance.IsKept)
                        if (IsKept)
                        {
                            foreach (InstanceNameInfo Item in CorrespondingInstance.PrecursorInstanceList)
                                if (Item.Instance.IsKept)
                                {
                                    errorList.Add(new ErrorInheritanceConflict(Item.Location, Item.Name.Name));
                                    break;
                                }
                        }
                        else
                            IsKept = true;
                }
                if (!IsKept)
                {
                    foreach (KeyValuePair<IFeatureName, IList<ICompiledFeature>> SetMemberEntry in PrecursorSet)
                    {
                        IFeatureName SetMemberKey = SetMemberEntry.Key;
                        InheritedInstanceInfo CorrespondingInstance = byNameTable[SetMemberKey];

                        foreach (InstanceNameInfo Item in CorrespondingInstance.PrecursorInstanceList)
                        {
                            errorList.Add(new ErrorMissingSelectedPrecursor(Item.Location, Item.Name.Name));
                            break;
                        }

                        break;
                    }

                    MissingKeptFlag = true;
                }
            }
            if (MissingKeptFlag)
                return false;

            return true;
        }

        private bool CheckPrecursorBodiesHaveAncestor(IHashtableEx<IFeatureName, InheritedInstanceInfo> byNameTable, IList<IError> errorList)
        {
            foreach (KeyValuePair<IFeatureName, InheritedInstanceInfo> ImportedEntry in byNameTable)
            {
                IFeatureName ImportedKey = ImportedEntry.Key;
                InheritedInstanceInfo ImportedInstance = ImportedEntry.Value;
                if (ImportedInstance.EffectiveInstance.IsAssigned)
                {
                    InstanceNameInfo Item = ImportedInstance.EffectiveInstance.Item;
                    ICompiledFeature EffectiveFeature = Item.Instance.Feature.Item;

                    if (EffectiveFeature.HasPrecursorBody)
                    {
                        bool HasEffectiveAncestor = false;

                        foreach (InstanceNameInfo AncestorItem in ImportedInstance.PrecursorInstanceList)
                        {
                            if (AncestorItem == Item)
                                continue;

                            ICompiledFeature AncestorEffectiveFeature = AncestorItem.Instance.Feature.Item;
                            if (AncestorEffectiveFeature.IsDeferredFeature)
                                continue;

                            HasEffectiveAncestor = true;
                        }

                        if (!HasEffectiveAncestor)
                        {
                            errorList.Add(new ErrorMissingAncestor(Item.Location, Item.Name.Name));
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void MergeInheritedFeatures(IClass item, IHashtableEx<IFeatureName, InheritedInstanceInfo> byNameTable, out IHashtableEx<IFeatureName, IFeatureInstance> mergedFeatureTable)
        {
            mergedFeatureTable = new HashtableEx<IFeatureName, IFeatureInstance>();

            foreach (KeyValuePair<IFeatureName, InheritedInstanceInfo> ImportedEntry in byNameTable)
            {
                IFeatureName ImportedKey = ImportedEntry.Key;
                InheritedInstanceInfo ImportedInstance = ImportedEntry.Value;
                InstanceNameInfo SelectedInstanceInfo;

                IFeatureInstance NewInstance;
                if (ImportedInstance.EffectiveInstance.IsAssigned)
                    SelectedInstanceInfo = ImportedInstance.EffectiveInstance.Item;
                else
                {
                    IList<InstanceNameInfo> InstancePrecursorList = ImportedInstance.PrecursorInstanceList;

                    SelectedInstanceInfo = null;
                    foreach (InstanceNameInfo Item in InstancePrecursorList)
                        if (Item.Instance.Owner.Item == item)
                        {
                            SelectedInstanceInfo = Item;
                            break;
                        }

                    if (SelectedInstanceInfo == null)
                        SelectedInstanceInfo = InstancePrecursorList[0];
                }

                NewInstance = new FeatureInstance(SelectedInstanceInfo.Instance.Owner.Item, SelectedInstanceInfo.Instance.Feature.Item, ImportedInstance.IsKept, ImportedInstance.IsDiscontinued);

                StableReference<IPrecursorInstance> OriginalPrecursor = new StableReference<IPrecursorInstance>();
                IList<IPrecursorInstance> PrecursorList = new List<IPrecursorInstance>();
                foreach (InstanceNameInfo Item in ImportedInstance.PrecursorInstanceList)
                {
                    if (Item == SelectedInstanceInfo)
                    {
                        foreach (IPrecursorInstance PrecursorInstance in Item.Instance.PrecursorList)
                            PrecursorList.Add(PrecursorInstance);
                    }
                    else
                    {
                        IPrecursorInstance NewPrecursor = new PrecursorInstance(Item.Ancestor, Item.Instance);
                        PrecursorList.Add(NewPrecursor);
                    }

                    if (Item.Instance.OriginalPrecursor.IsAssigned)
                    {
                        if (OriginalPrecursor.IsAssigned && OriginalPrecursor.Item != Item.Instance.OriginalPrecursor.Item)
                        {
                        }

                        OriginalPrecursor.Item = Item.Instance.OriginalPrecursor.Item;
                    }
                }

                if (OriginalPrecursor.IsAssigned)
                    NewInstance.OriginalPrecursor.Item = OriginalPrecursor.Item;
                else if (PrecursorList.Count > 0)
                    NewInstance.OriginalPrecursor.Item = PrecursorList[0];

                Debug.Assert(NewInstance.PrecursorList.Count == 0);
                foreach (IPrecursorInstance PrecursorInstance in PrecursorList)
                    NewInstance.PrecursorList.Add(PrecursorInstance);

                mergedFeatureTable.Add(ImportedKey, NewInstance);
            }
        }

        private void FillPrecursorList(IList<ICompiledFeature> precursorList, IFeatureInstance instance)
        {
            foreach (IPrecursorInstance PrecursorItem in instance.PrecursorList)
            {
                IFeatureInstance Precursor = PrecursorItem.Precursor;
                if (Precursor.Feature.IsAssigned && !precursorList.Contains(Precursor.Feature.Item))
                    precursorList.Add(Precursor.Feature.Item);

                FillPrecursorList(precursorList, Precursor);
            }
        }

        private bool PrecursorListIntersect(IList<ICompiledFeature> list1, IList<ICompiledFeature> list2)
        {
            foreach (ICompiledFeature Item in list2)
                if (list1.Contains(Item))
                    return true;

            return false;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IClass node, object data)
        {
        }
        #endregion
    }
}
