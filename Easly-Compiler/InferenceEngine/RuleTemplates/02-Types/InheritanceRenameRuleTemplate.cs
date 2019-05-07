namespace EaslyCompiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IInheritance"/>.
    /// </summary>
    public interface IInheritanceRenameRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IInheritance"/>.
    /// </summary>
    public class InheritanceRenameRuleTemplate : RuleTemplate<IInheritance, InheritanceRenameRuleTemplate>, IInheritanceRenameRuleTemplate
    {
        #region Init
        static InheritanceRenameRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IInheritance, ITypeName>(nameof(IInheritance.ResolvedClassParentTypeName)),
                new OnceReferenceSourceTemplate<IInheritance, IClassType>(nameof(IInheritance.ResolvedClassParentType)),
                new SealedTableSourceTemplate<IInheritance, string, IImportedClass>(nameof(IClass.ImportedClassTable), TemplateClassStart<IInheritance>.Default),
                new SealedTableSourceTemplate<IInheritance, IClassType, IObjectType>(nameof(IClass.InheritedClassTypeTable), TemplateClassStart<IInheritance>.Default),
                new SealedTableSourceTemplate<IInheritance, IFeatureName, IHashtableEx<string, IClass>>(nameof(IInheritance.ResolvedClassParentType) + Dot + nameof(IClassType.ExportTable)),
                new SealedTableSourceTemplate<IInheritance, IFeatureName, ITypedefType>(nameof(IInheritance.ResolvedClassParentType) + Dot + nameof(IClassType.TypedefTable)),
                new SealedTableSourceTemplate<IInheritance, IFeatureName, IDiscrete>(nameof(IInheritance.ResolvedClassParentType) + Dot + nameof(IClassType.DiscreteTable)),
                new SealedTableSourceTemplate<IInheritance, IFeatureName, IFeatureInstance>(nameof(IInheritance.ResolvedClassParentType) + Dot + nameof(IClassType.FeatureTable)),
                new OnceReferenceCollectionSourceTemplate<IInheritance, IRename, string>(nameof(IInheritance.RenameList), nameof(IRename.ValidSourceText)),
                new OnceReferenceCollectionSourceTemplate<IInheritance, IRename, string>(nameof(IInheritance.RenameList), nameof(IRename.ValidDestinationText)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IInheritance, IHashtableEx<IFeatureName, IHashtableEx<string, IClass>>>(nameof(IInheritance.ExportTable)),
                new OnceReferenceDestinationTemplate<IInheritance, IHashtableEx<IFeatureName, ITypedefType>>(nameof(IInheritance.TypedefTable)),
                new OnceReferenceDestinationTemplate<IInheritance, IHashtableEx<IFeatureName, IDiscrete>>(nameof(IInheritance.DiscreteTable)),
                new OnceReferenceDestinationTemplate<IInheritance, IHashtableEx<IFeatureName, IFeatureInstance>>(nameof(IInheritance.FeatureTable)),
                new OnceReferenceDestinationTemplate<IInheritance, ITypeName>(nameof(IInheritance.ResolvedTypeName)),
                new OnceReferenceDestinationTemplate<IInheritance, IClassType>(nameof(IInheritance.ResolvedType)),
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
        public override bool CheckConsistency(IInheritance node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IClassType ParentTypeWithRename = null;
            IClassType ResolvedParent = node.ResolvedClassParentType.Item;

            IHashtableEx<string, string> SourceIdentifierTable = new HashtableEx<string, string>(); // string (source) -> string (destination)
            IHashtableEx<string, string> DestinationIdentifierTable = new HashtableEx<string, string>(); // string (destination) -> string (source)
            IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> RenamedExportTable = ResolvedParent.ExportTable.CloneUnsealed();
            IHashtableEx<IFeatureName, ITypedefType> RenamedTypedefTable = ResolvedParent.TypedefTable.CloneUnsealed();
            IHashtableEx<IFeatureName, IDiscrete> RenamedDiscreteTable = ResolvedParent.DiscreteTable.CloneUnsealed();

            IHashtableEx<IFeatureName, IFeatureInstance> RenamedFeatureTable = new HashtableEx<IFeatureName, IFeatureInstance>();
            foreach (KeyValuePair<IFeatureName, IFeatureInstance> Entry in ResolvedParent.FeatureTable)
            {
                IFeatureName AncestorFeatureName = Entry.Key;
                IFeatureInstance AncestorFeatureInstance = Entry.Value;

                RenamedFeatureTable.Add(AncestorFeatureName, AncestorFeatureInstance.Clone(ResolvedParent));
            }

            foreach (IRename RenameItem in node.RenameList)
                Success &= RenameItem.CheckGenericRename(new IHashtableIndex<IFeatureName>[] { RenamedExportTable, RenamedTypedefTable, RenamedDiscreteTable, RenamedFeatureTable }, SourceIdentifierTable, DestinationIdentifierTable, (IFeatureName item) => item.Name, (string name) => new FeatureName(name), ErrorList);

            if (Success)
            {
                Success &= ResolveInstancingAfterRename(node, RenamedExportTable, RenamedTypedefTable, RenamedDiscreteTable, RenamedFeatureTable);
                if (Success)
                {
                    IClass EmbeddingClass = node.EmbeddingClass;

                    ParentTypeWithRename = ClassType.Create(ResolvedParent.BaseClass, ResolvedParent.TypeArgumentTable, EmbeddingClass.ResolvedClassType.Item);
                    ParentTypeWithRename.ExportTable.Merge(RenamedExportTable);
                    ParentTypeWithRename.ExportTable.Seal();
                    ParentTypeWithRename.TypedefTable.Merge(RenamedTypedefTable);
                    ParentTypeWithRename.TypedefTable.Seal();
                    ParentTypeWithRename.DiscreteTable.Merge(RenamedDiscreteTable);
                    ParentTypeWithRename.DiscreteTable.Seal();
                    ParentTypeWithRename.FeatureTable.Merge(RenamedFeatureTable);
                    ParentTypeWithRename.FeatureTable.Seal();
                }
            }

            if (Success)
                data = ParentTypeWithRename;

            return Success;
        }

        private bool ResolveInstancingAfterRename(IInheritance node, IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> exportTable, IHashtableEx<IFeatureName, ITypedefType> typedefTable, IHashtableEx<IFeatureName, IDiscrete> discreteTable, IHashtableEx<IFeatureName, IFeatureInstance> featureTable)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            IHashtableEx<string, IImportedClass> ImportedClassTable = EmbeddingClass.ImportedClassTable;

            bool Success = true;

            Success &= ResolveIdentifierList(node.ForgetList, out IHashtableEx<string, IIdentifier> ForgetTable);
            Success &= ResolveIdentifierList(node.KeepList, out IHashtableEx<string, IIdentifier> KeepTable);
            Success &= ResolveIdentifierList(node.DiscontinueList, out IHashtableEx<string, IIdentifier> DiscontinueTable);

            if (!Success)
                return false;

            if (!RemoveForgottenIdentifiers(exportTable, typedefTable, discreteTable, featureTable, ForgetTable))
                return false;
            if (!RemoveForgottenIndexer(featureTable, node))
                return false;

            Success &= CheckIdentifierContext(exportTable, KeepTable);
            Success &= CheckIdentifierContext(exportTable, DiscontinueTable);
            Success &= CheckIdentifierContext(typedefTable, KeepTable);
            Success &= CheckIdentifierContext(typedefTable, DiscontinueTable);
            Success &= CheckIdentifierContext(discreteTable, KeepTable);
            Success &= CheckIdentifierContext(discreteTable, DiscontinueTable);

            if (!Success)
                return false;

            if (!ResolveFeatureInstancingList(featureTable, KeepTable, (IFeatureInstance instance) => instance.SetIsKept(true)))
                return false;

            if (!ResolveFeatureInstancingList(featureTable, DiscontinueTable, (IFeatureInstance instance) => instance.SetIsDiscontinued(true)))
                return false;

            foreach (IExportChange ExportChangeItem in node.ExportChangeList)
                Success &= ExportChangeItem.ApplyChange(ImportedClassTable, exportTable, ErrorList);

            return Success;
        }

        private bool ResolveIdentifierList(IList<IIdentifier> identifierList, out IHashtableEx<string, IIdentifier> resultTable)
        {
            resultTable = new HashtableEx<string, IIdentifier>();

            foreach (IIdentifier IdentifierItem in identifierList)
            {
                string ValidText = IdentifierItem.ValidText.Item;

                if (resultTable.ContainsKey(ValidText))
                {
                    AddSourceError(new ErrorIdentifierAlreadyListed(IdentifierItem, ValidText));
                    return false;
                }

                resultTable.Add(ValidText, IdentifierItem);
            }

            return true;
        }

        private bool RemoveForgottenIdentifiers(IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> exportTable, IHashtableEx<IFeatureName, ITypedefType> typedefTable, IHashtableEx<IFeatureName, IDiscrete> discreteTable, IHashtableEx<IFeatureName, IFeatureInstance> featureTable, IHashtableEx<string, IIdentifier> forgetTable)
        {
            foreach (KeyValuePair<string, IIdentifier> IdentifierEntry in forgetTable)
            {
                string ValidIdentifier = IdentifierEntry.Key;
                IIdentifier IdentifierItem = IdentifierEntry.Value;
                bool Removed = false;

                RemoveIdentifierFromTable(exportTable as IDictionary, ValidIdentifier, ref Removed);
                RemoveIdentifierFromTable(typedefTable as IDictionary, ValidIdentifier, ref Removed);
                RemoveIdentifierFromTable(discreteTable as IDictionary, ValidIdentifier, ref Removed);
                RemoveIdentifierFromTable(featureTable as IDictionary, ValidIdentifier, ref Removed);

                if (!Removed)
                {
                    AddSourceError(new ErrorUnknownIdentifier(IdentifierItem, ValidIdentifier));
                    return false;
                }
            }

            return true;
        }

        private void RemoveIdentifierFromTable(IDictionary table, string identifier, ref bool isRemoved)
        {
            foreach (DictionaryEntry Entry in table)
            {
                IFeatureName EntryName = Entry.Key as IFeatureName;
                Debug.Assert(EntryName != null);

                if (EntryName.Name == identifier)
                {
                    table.Remove(EntryName);
                    isRemoved = true;
                    break;
                }
            }
        }

        private bool RemoveForgottenIndexer(IHashtableEx<IFeatureName, IFeatureInstance> featureTable, IInheritance inheritanceItem)
        {
            if (inheritanceItem.ForgetIndexer)
            {
                bool Removed = false;

                foreach (KeyValuePair<IFeatureName, IFeatureInstance> Entry in featureTable)
                    if (Entry.Key == FeatureName.IndexerFeatureName)
                    {
                        IFeatureInstance CurrentInstance = Entry.Value;
                        CurrentInstance.SetIsForgotten(true);
                        Removed = true;
                        break;
                    }

                if (!Removed)
                {
                    AddSourceError(new ErrorIndexerInheritance(inheritanceItem));
                    return false;
                }
            }

            return true;
        }

        private bool CheckIdentifierContext(IHashtableIndex<IFeatureName> testTable, IHashtableEx<string, IIdentifier> identifierTable)
        {
            foreach (KeyValuePair<string, IIdentifier> IdentifierEntry in identifierTable)
            {
                string ValidIdentifier = IdentifierEntry.Key;
                IIdentifier IdentifierItem = IdentifierEntry.Value;

                foreach (IFeatureName EntryName in testTable.Indexes)
                    if (EntryName.Name == ValidIdentifier)
                    {
                        AddSourceError(new ErrorInvalidIdentifierContext(IdentifierItem, ValidIdentifier));
                        return false;
                    }
            }

            return true;
        }

        private bool ResolveFeatureInstancingList(IHashtableEx<IFeatureName, IFeatureInstance> featureTable, IHashtableEx<string, IIdentifier> identifierTable, Action<IFeatureInstance> handler)
        {
            foreach (KeyValuePair<string, IIdentifier> IdentifierEntry in identifierTable)
            {
                string ValidIdentifier = IdentifierEntry.Key;
                IIdentifier IdentifierItem = IdentifierEntry.Value;

                OnceReference<IFeatureInstance> CurrentInstance = new OnceReference<IFeatureInstance>();
                foreach (KeyValuePair<IFeatureName, IFeatureInstance> Entry in featureTable)
                {
                    IFeatureName EntryName = Entry.Key;
                    if (EntryName.Name == ValidIdentifier)
                    {
                        CurrentInstance.Item = Entry.Value;
                        break;
                    }
                }

                if (!CurrentInstance.IsAssigned)
                {
                    AddSourceError(new ErrorUnknownIdentifier(IdentifierItem, ValidIdentifier));
                    return false;
                }

                handler(CurrentInstance.Item);
            }

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IInheritance node, object data)
        {
            IClassType ParentTypeWithRename = (IClassType)data;
            IClass EmbeddingClass = node.EmbeddingClass;

            node.ExportTable.Item = ParentTypeWithRename.ExportTable;
            node.TypedefTable.Item = ParentTypeWithRename.TypedefTable;
            node.DiscreteTable.Item = ParentTypeWithRename.DiscreteTable;
            node.FeatureTable.Item = ParentTypeWithRename.FeatureTable;

            ITypeName NewTypeName = new TypeName(ParentTypeWithRename.TypeFriendlyName);

            node.ResolvedTypeName.Item = NewTypeName;
            node.ResolvedType.Item = ParentTypeWithRename;

            EmbeddingClass.TypeTable.Add(NewTypeName, ParentTypeWithRename);
            EmbeddingClass.InheritanceTable.Add(NewTypeName, ParentTypeWithRename);
        }
        #endregion
    }
}
