namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllExportsRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllExportsRuleTemplate : RuleTemplate<IClass, AllExportsRuleTemplate>, IAllExportsRuleTemplate
    {
        #region Init
        static AllExportsRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IClass, IClassType>(nameof(IClass.ResolvedClassType)),
                new SealedTableSourceTemplate<IClass, IFeatureName, ISealableDictionary<string, IClass>>(nameof(IClass.LocalExportTable)),
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, IClassType>(nameof(IClass.InheritanceList), nameof(IInheritance.ResolvedType)),
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>>>(nameof(IClass.InheritanceList), nameof(IInheritance.ExportTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, IFeatureName, ISealableDictionary<string, IClass>>(nameof(IClass.ExportTable)),
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
            bool Success;
            data = null;

            Debug.Assert(node.LocalExportTable.IsSealed);
            ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> MergedExportTable = node.LocalExportTable.CloneUnsealed();

            Success = !HasConflictingEntry(node, MergedExportTable);
            if (Success)
                foreach (IExport Export in node.ExportList)
                    Success &= MergeExportEntry(node, Export, MergedExportTable);

            if (Success)
                data = MergedExportTable;

            return Success;
        }

        private bool HasConflictingEntry(IClass node, ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> mergedExportTable)
        {
            bool Result = false;

            foreach (IInheritance Inheritance in node.InheritanceList)
            {
                Debug.Assert(Inheritance.ExportTable.IsAssigned);

                ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> InheritedExportTable = Inheritance.ExportTable.Item;

                foreach (KeyValuePair<IFeatureName, ISealableDictionary<string, IClass>> InstanceEntry in InheritedExportTable)
                {
                    IFeatureName InstanceName = InstanceEntry.Key;
                    ISealableDictionary<string, IClass> InstanceItem = InstanceEntry.Value;
                    bool ConflictingEntry = false;

                    foreach (KeyValuePair<IFeatureName, ISealableDictionary<string, IClass>> Entry in mergedExportTable)
                    {
                        IFeatureName LocalName = Entry.Key;
                        ISealableDictionary<string, IClass> LocalItem = Entry.Value;

                        if (InstanceName.Name == LocalName.Name)
                        {
                            if (InstanceItem != LocalItem)
                            {
                                AddSourceError(new ErrorDuplicateName(Inheritance, LocalName.Name));
                                ConflictingEntry = true;
                                Result = true;
                            }
                        }
                        else if (InstanceItem == LocalItem)
                        {
                            AddSourceError(new ErrorExportNameConflict(Inheritance, LocalName.Name, InstanceName.Name));
                            ConflictingEntry = true;
                            Result = true;
                        }
                    }

                    if (!ConflictingEntry && !mergedExportTable.ContainsKey(InstanceName))
                        mergedExportTable.Add(InstanceName, InstanceItem);
                }
            }

            return Result;
        }

        private bool MergeExportEntry(IClass node, IExport export, ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> mergedExportTable)
        {
            bool Success = true;

            ISealableDictionary<IFeatureName, IIdentifier> ListedExportList = MergeExportClassIdentifiers(node, export, mergedExportTable, ref Success);
            IList<IFeatureName> ListedIdentifiers = new List<IFeatureName>(ListedExportList.Indexes);

            List<ISealableDictionary<string, IClass>> OtherClassTableList = new List<ISealableDictionary<string, IClass>>();
            foreach (IFeatureName ExportName in ListedIdentifiers)
            {
                ISealableDictionary<string, IClass> OtherClassTable = mergedExportTable[ExportName];
                OtherClassTableList.Add(OtherClassTable);
            }

            ISealableDictionary<string, IClass> FilledClassTable = new SealableDictionary<string, IClass>();

            foreach (ISealableDictionary<string, IClass> OtherClassTable in OtherClassTableList)
                ResolveAsExportIdentifier(OtherClassTable, FilledClassTable);

            CheckMultipleClassIdentifiers(node, export, FilledClassTable, ref Success);

            ISealableDictionary<string, IClass> ClassTable = node.LocalExportTable[export.ValidExportName.Item];
            foreach (KeyValuePair<string, IClass> Entry in FilledClassTable)
            {
                string ClassIdentifier = Entry.Key;
                IClass ExportClassItem = Entry.Value;

                ClassTable.Add(ClassIdentifier, ExportClassItem);
            }

            export.ExportClassTable.Item = FilledClassTable;

            return Success;
        }

        private ISealableDictionary<IFeatureName, IIdentifier> MergeExportClassIdentifiers(IClass node, IExport export, ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> mergedExportTable, ref bool success)
        {
            IList<string> ListedClassList = new List<string>();
            ISealableDictionary<IFeatureName, IIdentifier> ListedExportList = new SealableDictionary<IFeatureName, IIdentifier>();

            for (int i = 0; i < export.ClassIdentifierList.Count; i++)
            {
                IIdentifier Identifier = export.ClassIdentifierList[i];

                Debug.Assert(Identifier.ValidText.IsAssigned);
                string ValidText = Identifier.ValidText.Item;

                if (ValidText.ToUpperInvariant() == LanguageClasses.Any.Name.ToUpperInvariant())
                    ListedClassList.Add(LanguageClasses.Any.Name);
                else if (node.ImportedClassTable.ContainsKey(ValidText))
                    ListedClassList.Add(ValidText);
                else if (FeatureName.TableContain(mergedExportTable, ValidText, out IFeatureName Key, out ISealableDictionary<string, IClass> Item))
                {
                    if (ListedExportList.ContainsKey(Key))
                    {
                        AddSourceError(new ErrorIdentifierAlreadyListed(Identifier, ValidText));
                        success = false;
                    }
                    else
                        ListedExportList.Add(Key, Identifier);
                }
                else
                {
                    AddSourceError(new ErrorUnknownIdentifier(Identifier, ValidText));
                    success = false;
                }
            }

            return ListedExportList;
        }

        private void ResolveAsExportIdentifier(ISealableDictionary<string, IClass> otherClassTable, ISealableDictionary<string, IClass> classTable)
        {
            foreach (KeyValuePair<string, IClass> Entry in otherClassTable)
            {
                string ClassIdentifier = Entry.Key;
                IClass ClassItem = Entry.Value;

                if (!classTable.ContainsKey(ClassIdentifier))
                    classTable.Add(ClassIdentifier, ClassItem);
            }
        }

        private void CheckMultipleClassIdentifiers(IClass node, IExport export, ISealableDictionary<string, IClass> filledClassTable, ref bool success)
        {
            for (int i = 0; i < export.ClassIdentifierList.Count; i++)
            {
                IIdentifier Identifier = export.ClassIdentifierList[i];
                string ValidIdentifier = Identifier.ValidText.Item;

                if (ValidIdentifier.ToUpperInvariant() != LanguageClasses.Any.Name.ToUpperInvariant() && node.ImportedClassTable.ContainsKey(ValidIdentifier))
                {
                    IImportedClass Imported = node.ImportedClassTable[ValidIdentifier];

                    if (filledClassTable.ContainsKey(ValidIdentifier))
                    {
                        AddSourceError(new ErrorIdentifierAlreadyListed(Identifier, ValidIdentifier));
                        success = false;
                    }
                    else
                        filledClassTable.Add(ValidIdentifier, Imported.Item);
                }
            }
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IClass node, object data)
        {
            ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> MergedExportTable = (ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>>)data;

            Debug.Assert(node.ResolvedClassType.IsAssigned);
            IClassType ThisClassType = node.ResolvedClassType.Item;

            ThisClassType.ExportTable.Merge(MergedExportTable);
            ThisClassType.ExportTable.Seal();

            node.ExportTable.Merge(MergedExportTable);
            node.ExportTable.Seal();

            foreach (IClassType Item in node.GenericInstanceList)
            {
                Item.ExportTable.Merge(MergedExportTable);
                Item.ExportTable.Seal();
            }
        }
        #endregion
    }
}
