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
                new SealedTableSourceTemplate<IClass, IFeatureName, IHashtableEx<string, IClass>>(nameof(IClass.LocalExportTable)),
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, IClassType>(nameof(IClass.InheritanceList), nameof(IInheritance.ResolvedType)),
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, IHashtableEx<IFeatureName, IHashtableEx<string, IClass>>>(nameof(IClass.InheritanceList), nameof(IInheritance.ExportTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, IFeatureName, IHashtableEx<string, IClass>>(nameof(IClass.ExportTable)),
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
            IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> MergedExportTable = node.LocalExportTable.CloneUnsealed();

            Success = !HasConflictingEntry(node, MergedExportTable);
            if (Success)
                foreach (IExport Export in node.ExportList)
                    Success &= MergeExportEntry(node, Export, MergedExportTable);

            if (Success)
                data = MergedExportTable;

            return Success;
        }

        private bool HasConflictingEntry(IClass node, IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> mergedExportTable)
        {
            bool Result = false;

            foreach (IInheritance Inheritance in node.InheritanceList)
            {
                Debug.Assert(Inheritance.ResolvedType.IsAssigned);
                IClassType InheritanceParent = Inheritance.ResolvedType.Item;

                IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> InheritedExportTable = InheritanceParent.ExportTable;

                // TODO: verify InheritedExportTable == Inheritance.ExportTable since the source is on the later.
                foreach (KeyValuePair<IFeatureName, IHashtableEx<string, IClass>> InstanceEntry in InheritedExportTable)
                {
                    IFeatureName InstanceName = InstanceEntry.Key;
                    IHashtableEx<string, IClass> InstanceItem = InstanceEntry.Value;
                    bool ConflictingEntry = false;

                    foreach (KeyValuePair<IFeatureName, IHashtableEx<string, IClass>> Entry in mergedExportTable)
                    {
                        IFeatureName LocalName = Entry.Key;
                        IHashtableEx<string, IClass> LocalItem = Entry.Value;

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

        private bool MergeExportEntry(IClass node, IExport export, IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> mergedExportTable)
        {
            bool Success = true;

            IHashtableEx<IFeatureName, IIdentifier> ListedExportList = MergeExportClassIdentifiers(node, export, mergedExportTable, ref Success);
            IList<IFeatureName> ListedIdentifiers = new List<IFeatureName>(ListedExportList.Indexes);

            List<IHashtableEx<string, IClass>> OtherClassTableList = new List<IHashtableEx<string, IClass>>();
            foreach (IFeatureName ExportName in ListedIdentifiers)
            {
                IHashtableEx<string, IClass> OtherClassTable = mergedExportTable[ExportName];
                OtherClassTableList.Add(OtherClassTable);
            }

            IHashtableEx<string, IClass> FilledClassTable = new HashtableEx<string, IClass>();

            foreach (IHashtableEx<string, IClass> OtherClassTable in OtherClassTableList)
                ResolveAsExportIdentifier(OtherClassTable, FilledClassTable);

            CheckMultipleClassIdentifiers(node, export, FilledClassTable, ref Success);

            IHashtableEx<string, IClass> ClassTable = node.LocalExportTable[export.ValidExportName.Item];
            foreach (KeyValuePair<string, IClass> Entry in FilledClassTable)
            {
                string ClassIdentifier = Entry.Key;
                IClass ExportClassItem = Entry.Value;

                ClassTable.Add(ClassIdentifier, ExportClassItem);
            }

            export.ExportClassTable.Item = FilledClassTable;

            return Success;
        }

        private IHashtableEx<IFeatureName, IIdentifier> MergeExportClassIdentifiers(IClass node, IExport export, IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> mergedExportTable, ref bool success)
        {
            IList<string> ListedClassList = new List<string>();
            IHashtableEx<IFeatureName, IIdentifier> ListedExportList = new HashtableEx<IFeatureName, IIdentifier>();

            for (int i = 0; i < export.ClassIdentifierList.Count; i++)
            {
                IIdentifier Identifier = export.ClassIdentifierList[i];

                Debug.Assert(Identifier.ValidText.IsAssigned);
                string ValidText = Identifier.ValidText.Item;

                if (ValidText.ToLower() == LanguageClasses.Any.Name.ToLower())
                    ListedClassList.Add(LanguageClasses.Any.Name);
                else if (node.ImportedClassTable.ContainsKey(ValidText))
                    ListedClassList.Add(ValidText);
                else if (FeatureName.TableContain(mergedExportTable, ValidText, out IFeatureName Key, out IHashtableEx<string, IClass> Item))
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

        private void ResolveAsExportIdentifier(IHashtableEx<string, IClass> otherClassTable, IHashtableEx<string, IClass> classTable)
        {
            foreach (KeyValuePair<string, IClass> Entry in otherClassTable)
            {
                string ClassIdentifier = Entry.Key;
                IClass ClassItem = Entry.Value;

                if (!classTable.ContainsKey(ClassIdentifier))
                    classTable.Add(ClassIdentifier, ClassItem);
            }
        }

        private void CheckMultipleClassIdentifiers(IClass node, IExport export, IHashtableEx<string, IClass> filledClassTable, ref bool success)
        {
            for (int i = 0; i < export.ClassIdentifierList.Count; i++)
            {
                IIdentifier Identifier = export.ClassIdentifierList[i];
                string ValidIdentifier = Identifier.ValidText.Item;

                if (ValidIdentifier.ToLower() != LanguageClasses.Any.Name.ToLower() && node.ImportedClassTable.ContainsKey(ValidIdentifier))
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
            IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> MergedExportTable = (IHashtableEx<IFeatureName, IHashtableEx<string, IClass>>)data;

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
