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
                new OnceReferenceCollectionSourceTemplate<IClass, IFeature, ICompiledFeature>(nameof(IClass.FeatureList), nameof(IFeature.ResolvedFeature)),
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, IClassType>(nameof(IClass.InheritanceList), nameof(IInheritance.ResolvedType)),
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, ISealableDictionary<IFeatureName, IFeatureInstance>>(nameof(IClass.InheritanceList), nameof(IInheritance.FeatureTable)),
                new SealedTableCollectionSourceTemplate<IClass, IGeneric, ITypeName, ICompiledType>(nameof(IClass.GenericList), nameof(IGeneric.ResolvedConformanceTable)),
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

            ISealableDictionary<IFeatureName, IFeatureInstance> MergedFeatureTable = null;

            IList<AncestorFeatureInfo> FeatureTableList = new List<AncestorFeatureInfo>();
            ListLocalAndInheritedFeatures(node, FeatureTableList);

            ISealableDictionary<ICompiledFeature, IList<InstanceNameInfo>> ByFeatureTable; // ICompiledFeature -> List of InstanceNameInfo
            ISealableDictionary<IFeatureName, InheritedInstanceInfo> ByNameTable; // FeatureName -> InheritedInstanceInfo
            SortByFeatureAndByName(FeatureTableList, out ByFeatureTable, out ByNameTable);

            if (!CheckInheritanceConsistency(ByFeatureTable, ByNameTable, node.ResolvedClassType.Item, ErrorList))
                Success = false;
            else if (!CheckAllPrecursorSelected(ByNameTable, ErrorList))
                Success = false;
            else if (!CheckPrecursorBodiesHaveAncestor(ByNameTable, ErrorList))
                Success = false;
            else
            {
                MergeInheritedFeatures(node, ByNameTable, out MergedFeatureTable);
                ClassType.MergeConformingParentTypes(node, node.ResolvedClassType.Item);
            }

            if (Success)
                data = MergedFeatureTable;

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

        private void SortByFeatureAndByName(IList<AncestorFeatureInfo> featureTableList, out ISealableDictionary<ICompiledFeature, IList<InstanceNameInfo>> byFeatureTable, out ISealableDictionary<IFeatureName, InheritedInstanceInfo> byNameTable)
        {
            byFeatureTable = new SealableDictionary<ICompiledFeature, IList<InstanceNameInfo>>(); // ICompiledFeature -> List of InstanceNameInfo
            byNameTable = new SealableDictionary<IFeatureName, InheritedInstanceInfo>(); // FeatureName -> InheritedInstanceInfo

            foreach (AncestorFeatureInfo FeatureInfoItem in featureTableList)
            {
                foreach (KeyValuePair<IFeatureName, IFeatureInstance> Entry in FeatureInfoItem.FeatureTable)
                {
                    IFeatureName Key = Entry.Key;
                    IFeatureInstance Value = Entry.Value;

                    Debug.Assert(Value.Feature != null);

                    CheckIfFeatureListed(byFeatureTable, FeatureInfoItem, Key, Value);
                    CheckIfFeatureNameListed(byNameTable, FeatureInfoItem, Key, Value);
                }
            }
        }

        private void CheckIfFeatureListed(ISealableDictionary<ICompiledFeature, IList<InstanceNameInfo>> byFeatureTable, AncestorFeatureInfo featureInfo, IFeatureName featureName, IFeatureInstance featureInstance)
        {
            bool FeatureAlreadyListed = false;
            foreach (KeyValuePair<ICompiledFeature, IList<InstanceNameInfo>> ImportedEntry in byFeatureTable)
            {
                ICompiledFeature ImportedKey = ImportedEntry.Key;
                IList<InstanceNameInfo> NameList = ImportedEntry.Value;

                // Feature already listed
                if (featureInstance.Feature == ImportedKey)
                {
                    UpdateNameList(featureInfo, featureName, featureInstance, NameList);
                    FeatureAlreadyListed = true;
                    break;
                }
            }
            if (!FeatureAlreadyListed)
            {
                IList<InstanceNameInfo> InitList = new List<InstanceNameInfo>();
                InstanceNameInfo NewInfo = new InstanceNameInfo(featureInfo, featureInstance, featureName);
                InitList.Add(NewInfo);

                byFeatureTable.Add(featureInstance.Feature, InitList);
            }
        }

        private void UpdateNameList(AncestorFeatureInfo featureInfo, IFeatureName featureName, IFeatureInstance featureInstance, IList<InstanceNameInfo> nameList)
        {
            OnceReference<InstanceNameInfo> PreviousInstance = new OnceReference<InstanceNameInfo>();

            int i;
            for (i = 0; i < nameList.Count; i++)
            {
                InstanceNameInfo Item = nameList[i];
                if (featureName.Name == Item.Name.Name)
                {
                    PreviousInstance.Item = Item;
                    break;
                }
            }

            // C inherit f from A and B, effectively or not, but keep or discontinue flags don't match.
            if (PreviousInstance.IsAssigned && (PreviousInstance.Item.Instance.IsForgotten == featureInstance.IsForgotten))
            {
                PreviousInstance.Item.SameIsKept = PreviousInstance.Item.Instance.IsKept == featureInstance.IsKept;
                PreviousInstance.Item.SameIsDiscontinued = PreviousInstance.Item.Instance.IsDiscontinued == featureInstance.IsDiscontinued;
            }

            if (!PreviousInstance.IsAssigned || (PreviousInstance.Item.Instance.IsForgotten && !featureInstance.IsForgotten))
            {
                InstanceNameInfo NewInfo = new InstanceNameInfo(featureInfo, featureInstance, featureName);
                if (i < nameList.Count)
                    nameList[i] = NewInfo;
                else
                    nameList.Add(NewInfo);
            }
        }

        private void CheckIfFeatureNameListed(ISealableDictionary<IFeatureName, InheritedInstanceInfo> byNameTable, AncestorFeatureInfo featureInfo, IFeatureName featureName, IFeatureInstance featureInstance)
        {
            bool FeatureAlreadyListed = false;
            bool NameAlreadyListed = false;

            foreach (KeyValuePair<IFeatureName, InheritedInstanceInfo> ImportedEntry in byNameTable)
            {
                IFeatureName ImportedKey = ImportedEntry.Key;
                InheritedInstanceInfo ImportedInstance = ImportedEntry.Value;
                IList<InstanceNameInfo> InstanceList = ImportedInstance.PrecursorInstanceList;

                if (featureName.Name == ImportedKey.Name)
                {
                    FeatureAlreadyListed = false;

                    Debug.Assert(featureInstance.Feature != null);

                    foreach (InstanceNameInfo Item in InstanceList)
                    {
                        Debug.Assert(Item.Instance.Feature != null);

                        if (featureInstance.Feature == Item.Instance.Feature)
                        {
                            FeatureAlreadyListed = true;
                            break;
                        }
                    }

                    if (!FeatureAlreadyListed)
                    {
                        InstanceNameInfo NewInfo = new InstanceNameInfo(featureInfo, featureInstance, featureName);
                        InstanceList.Add(NewInfo);
                    }

                    NameAlreadyListed = true;
                    break;
                }
            }
            if (!NameAlreadyListed)
            {
                IList<InstanceNameInfo> InitList = new List<InstanceNameInfo>();
                InstanceNameInfo NewInfo = new InstanceNameInfo(featureInfo, featureInstance, featureName);
                InitList.Add(NewInfo);

                InheritedInstanceInfo NewName = new InheritedInstanceInfo();
                NewName.PrecursorInstanceList = InitList;

                byNameTable.Add(featureName, NewName);
            }
        }

        private bool CheckInheritanceConsistency(ISealableDictionary<ICompiledFeature, IList<InstanceNameInfo>> byFeatureTable, ISealableDictionary<IFeatureName, InheritedInstanceInfo> byNameTable, IClassType localClassType, IErrorList errorList)
        {
            if (!IsKeepDiscontinueConsistent(byFeatureTable, errorList))
                return false;

            if (!IsSingleEffective(byNameTable, errorList))
                return false;

            bool AllRedefineConformant = true;
            bool AllFlagsTheSame = true;

            foreach (KeyValuePair<IFeatureName, InheritedInstanceInfo> ImportedEntry in byNameTable)
            {
                IFeatureName ImportedKey = ImportedEntry.Key;
                InheritedInstanceInfo ImportedInstance = ImportedEntry.Value;

                // If there is no effective instance for this name
                if (!ImportedInstance.EffectiveInstance.IsAssigned)
                    AllFlagsTheSame &= CompareNonEffectiveFlags(ImportedInstance, errorList);
                else
                    AllRedefineConformant &= CompareEffectiveFlags(ImportedInstance, errorList, localClassType);
            }
            if (!AllFlagsTheSame || !AllRedefineConformant)
                return false;

            return true;
        }

        private bool IsKeepDiscontinueConsistent(ISealableDictionary<ICompiledFeature, IList<InstanceNameInfo>> byFeatureTable, IErrorList errorList)
        {
            bool IsConsistent = true;
            foreach (KeyValuePair<ICompiledFeature, IList<InstanceNameInfo>> ImportedEntry in byFeatureTable)
            {
                IList<InstanceNameInfo> NameList = ImportedEntry.Value;

                foreach (InstanceNameInfo Item in NameList)
                    if (!Item.SameIsKept || !Item.SameIsDiscontinued)
                    {
                        // C inherit f from A and B, effectively or not, but keep or discontinue flags don't match.
                        errorList.AddError(new ErrorInheritanceConflict(Item.Location, Item.Name.Name));
                        IsConsistent = false;
                        break;
                    }
            }

            return IsConsistent;
        }

        private bool IsSingleEffective(ISealableDictionary<IFeatureName, InheritedInstanceInfo> byNameTable, IErrorList errorList)
        {
            bool IsSingle = true;
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
                            errorList.AddError(new ErrorMultipleEffectiveFeature(Item.Location, Item.Name.Name));
                            IsSingle = false;
                            break;
                        }
                    }
            }

            return IsSingle;
        }

        private bool CompareNonEffectiveFlags(InheritedInstanceInfo importedInstance, IErrorList errorList)
        {
            bool Result = true;

            IList<InstanceNameInfo> InstanceList = importedInstance.PrecursorInstanceList;
            importedInstance.IsKept = InstanceList[0].Instance.IsKept;
            importedInstance.IsDiscontinued = InstanceList[0].Instance.IsDiscontinued;

            if (InstanceList.Count > 1)
            {
                ICompiledType FeatureType = InstanceList[0].Instance.Feature.ResolvedAgentType.Item;

                for (int i = 1; i < InstanceList.Count && Result; i++)
                {
                    InstanceNameInfo ThisInstance = InstanceList[i];

                    Result &= importedInstance.IsKept == ThisInstance.Instance.IsKept;
                    Result &= importedInstance.IsDiscontinued == ThisInstance.Instance.IsDiscontinued;
                    Result &= ObjectType.TypesHaveIdenticalSignature(FeatureType, ThisInstance.Instance.Feature.ResolvedAgentType.Item);

                    if (!Result)
                    {
                        if (FeatureType is IIndexerType)
                            errorList.AddError(new ErrorIndexerInheritanceConflict(ThisInstance.Location));
                        else
                            errorList.AddError(new ErrorInheritanceConflict(ThisInstance.Location, ThisInstance.Name.Name));
                    }
                }
            }

            return Result;
        }

        private bool CompareEffectiveFlags(InheritedInstanceInfo importedInstance, IErrorList errorList, IClassType localClassType)
        {
            bool Result = true;

            importedInstance.IsKept = importedInstance.EffectiveInstance.Item.Instance.IsKept;
            importedInstance.IsDiscontinued = importedInstance.EffectiveInstance.Item.Instance.IsDiscontinued;

            // If the effective instance is a redefine.
            if (importedInstance.EffectiveInstance.Item.Ancestor == localClassType && importedInstance.EffectiveInstance.Item.Instance.Feature.ResolvedAgentType.IsAssigned)
            {
                ICompiledType DescendantFeatureType = importedInstance.EffectiveInstance.Item.Instance.Feature.ResolvedAgentType.Item;

                IList<InstanceNameInfo> InstanceList = importedInstance.PrecursorInstanceList;
                foreach (InstanceNameInfo Item in InstanceList)
                {
                    if (Item == importedInstance.EffectiveInstance.Item)
                        continue;

                    ICompiledType AncestorFeatureType = Item.Instance.Feature.ResolvedAgentType.Item;

                    if (!ObjectType.TypeConformToBase(DescendantFeatureType, AncestorFeatureType, errorList, (ISource)importedInstance.EffectiveInstance.Item.Instance.Feature, isConversionAllowed: false))
                    {
                        errorList.AddError(new ErrorInheritanceConflict(Item.Location, Item.Name.Name));
                        Result = false;
                    }
                }
            }

            return Result;
        }

        private bool CheckAllPrecursorSelected(ISealableDictionary<IFeatureName, InheritedInstanceInfo> byNameTable, IErrorList errorList)
        {
            IList<ISealableDictionary<IFeatureName, IList<ICompiledFeature>>> PrecursorSetList = new List<ISealableDictionary<IFeatureName, IList<ICompiledFeature>>>(); // FeatureName -> List<ICompiledFeature> (Precursor list)
            CheckAllPrecursorSelectedInNameTable(byNameTable, PrecursorSetList);

            bool Result = true;

            foreach (ISealableDictionary<IFeatureName, IList<ICompiledFeature>> PrecursorSet in PrecursorSetList)
                Result &= CheckPrecursorSelected(byNameTable, PrecursorSet, errorList);

            return Result;
        }

        private bool CheckPrecursorSelected(ISealableDictionary<IFeatureName, InheritedInstanceInfo> byNameTable, ISealableDictionary<IFeatureName, IList<ICompiledFeature>> precursorSet, IErrorList errorList)
        {
            bool Success = true;
            bool IsKept = false;

            foreach (KeyValuePair<IFeatureName, IList<ICompiledFeature>> SetMemberEntry in precursorSet)
            {
                IFeatureName SetMemberKey = SetMemberEntry.Key;
                InheritedInstanceInfo CorrespondingInstance = byNameTable[SetMemberKey];

                if (CorrespondingInstance.IsKept)
                    if (IsKept)
                    {
                        foreach (InstanceNameInfo Item in CorrespondingInstance.PrecursorInstanceList)
                            if (Item.Instance.IsKept)
                            {
                                errorList.AddError(new ErrorInheritanceConflict(Item.Location, Item.Name.Name));
                                Success = false;
                                break;
                            }
                    }
                    else
                        IsKept = true;
            }

            if (!IsKept && precursorSet.Count > 1)
            {
                foreach (KeyValuePair<IFeatureName, IList<ICompiledFeature>> SetMemberEntry in precursorSet)
                {
                    IFeatureName SetMemberKey = SetMemberEntry.Key;
                    InheritedInstanceInfo CorrespondingInstance = byNameTable[SetMemberKey];

                    foreach (InstanceNameInfo Item in CorrespondingInstance.PrecursorInstanceList)
                    {
                        errorList.AddError(new ErrorMissingSelectedPrecursor(Item.Location, Item.Name.Name));
                        Success = false;
                        break;
                    }

                    break;
                }

                Debug.Assert(!Success);
            }

            return Success;
        }

        private void CheckAllPrecursorSelectedInNameTable(ISealableDictionary<IFeatureName, InheritedInstanceInfo> byNameTable, IList<ISealableDictionary<IFeatureName, IList<ICompiledFeature>>> precursorSetList)
        {
            foreach (KeyValuePair<IFeatureName, InheritedInstanceInfo> Entry in byNameTable)
            {
                IFeatureName Key = Entry.Key;
                InheritedInstanceInfo Value = Entry.Value;

                IList<ICompiledFeature> PrecursorList = new List<ICompiledFeature>();
                foreach (InstanceNameInfo PrecursorItem in Value.PrecursorInstanceList)
                    FillPrecursorList(PrecursorList, PrecursorItem.Instance);

                bool FoundInSet = false;
                foreach (ISealableDictionary<IFeatureName, IList<ICompiledFeature>> PrecursorSet in precursorSetList)
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
                    ISealableDictionary<IFeatureName, IList<ICompiledFeature>> NewSet = new SealableDictionary<IFeatureName, IList<ICompiledFeature>>();
                    NewSet.Add(Key, PrecursorList);
                    precursorSetList.Add(NewSet);
                }
            }
        }

        private bool CheckPrecursorBodiesHaveAncestor(ISealableDictionary<IFeatureName, InheritedInstanceInfo> byNameTable, IErrorList errorList)
        {
            foreach (KeyValuePair<IFeatureName, InheritedInstanceInfo> ImportedEntry in byNameTable)
            {
                IFeatureName ImportedKey = ImportedEntry.Key;
                InheritedInstanceInfo ImportedInstance = ImportedEntry.Value;
                if (ImportedInstance.EffectiveInstance.IsAssigned)
                {
                    InstanceNameInfo Item = ImportedInstance.EffectiveInstance.Item;
                    ICompiledFeature EffectiveFeature = Item.Instance.Feature;

                    if (EffectiveFeature.HasPrecursorBody)
                    {
                        bool HasEffectiveAncestor = false;

                        foreach (InstanceNameInfo AncestorItem in ImportedInstance.PrecursorInstanceList)
                        {
                            if (AncestorItem == Item)
                                continue;

                            ICompiledFeature AncestorEffectiveFeature = AncestorItem.Instance.Feature;
                            if (AncestorEffectiveFeature.IsDeferredFeature)
                                continue;

                            HasEffectiveAncestor = true;
                        }

                        if (!HasEffectiveAncestor)
                        {
                            IFeature AsFeature = EffectiveFeature as IFeature;
                            Debug.Assert(AsFeature != null);

                            errorList.AddError(new ErrorMissingAncestor(AsFeature, Item.Name.Name));
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void MergeInheritedFeatures(IClass item, ISealableDictionary<IFeatureName, InheritedInstanceInfo> byNameTable, out ISealableDictionary<IFeatureName, IFeatureInstance> mergedFeatureTable)
        {
            mergedFeatureTable = new SealableDictionary<IFeatureName, IFeatureInstance>();

            foreach (KeyValuePair<IFeatureName, InheritedInstanceInfo> ImportedEntry in byNameTable)
            {
                IFeatureName ImportedKey = ImportedEntry.Key;
                InheritedInstanceInfo ImportedInstance = ImportedEntry.Value;

                IFeatureInstance NewInstance = MergeCreateNewInstance(item, ImportedKey, ImportedInstance, out InstanceNameInfo SelectedInstanceInfo);

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
                        OriginalPrecursor.Item = Item.Instance.OriginalPrecursor.Item;
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

        private IFeatureInstance MergeCreateNewInstance(IClass item, IFeatureName importedKey, InheritedInstanceInfo importedInstance, out InstanceNameInfo selectedInstanceInfo)
        {
            IFeatureInstance NewInstance;
            if (importedInstance.EffectiveInstance.IsAssigned)
                selectedInstanceInfo = importedInstance.EffectiveInstance.Item;
            else
            {
                IList<InstanceNameInfo> InstancePrecursorList = importedInstance.PrecursorInstanceList;

                selectedInstanceInfo = null;
                foreach (InstanceNameInfo Item in InstancePrecursorList)
                    if (Item.Instance.Owner == item)
                    {
                        selectedInstanceInfo = Item;
                        break;
                    }

                if (selectedInstanceInfo == null)
                    selectedInstanceInfo = InstancePrecursorList[0];
            }

            NewInstance = new FeatureInstance(selectedInstanceInfo.Instance.Owner, selectedInstanceInfo.Instance.Feature, importedInstance.IsKept, importedInstance.IsDiscontinued);
            return NewInstance;
        }

        private void FillPrecursorList(IList<ICompiledFeature> precursorList, IFeatureInstance instance)
        {
            foreach (IPrecursorInstance PrecursorItem in instance.PrecursorList)
            {
                IFeatureInstance Precursor = PrecursorItem.Precursor;

                Debug.Assert(Precursor.Feature != null);

                if (!precursorList.Contains(Precursor.Feature))
                    precursorList.Add(Precursor.Feature);

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
            ISealableDictionary<IFeatureName, IFeatureInstance> MergedFeatureTable = (ISealableDictionary<IFeatureName, IFeatureInstance>)data;
            IClassType ThisClassType = node.ResolvedClassType.Item;

            ThisClassType.FeatureTable.Merge(MergedFeatureTable);
            ThisClassType.FeatureTable.Seal();

            node.FeatureTable.Merge(MergedFeatureTable);
            node.FeatureTable.Seal();

            foreach (ICompiledType Item in node.GenericInstanceList)
            {
                Item.FeatureTable.Merge(MergedFeatureTable);
                Item.FeatureTable.Seal();
            }
        }
        #endregion
    }
}
