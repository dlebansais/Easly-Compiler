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
                new OnceReferenceCollectionSourceTemplate<IClass, IClassType>(nameof(IClass.InheritanceList), nameof(IInheritance.ResolvedType)),
                new OnceReferenceCollectionSourceTemplate<IClass, IHashtableEx<IFeatureName, IHashtableEx<string, IClass>>>(nameof(IClass.InheritanceList), nameof(IInheritance.ExportTable)),
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
            bool Success = true;
            data = null;

            bool HasConflictingEntry = false;
            IList<IFeatureName> ListedIdentifiers;

            Debug.Assert(node.LocalExportTable.IsSealed);
            IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> MergedExportTable = node.LocalExportTable.CloneUnsealed();

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

                    foreach (KeyValuePair<IFeatureName, IHashtableEx<string, IClass>> Entry in MergedExportTable)
                    {
                        IFeatureName LocalName = Entry.Key;
                        IHashtableEx<string, IClass> LocalItem = Entry.Value;

                        if (InstanceName.Name == LocalName.Name)
                        {
                            if (InstanceItem != LocalItem)
                            {
                                AddSourceError(new ErrorDuplicateName(Inheritance, LocalName.Name));
                                ConflictingEntry = true;
                            }
                        }

                        else if (InstanceItem == LocalItem)
                        {
                            AddSourceError(new ErrorExportNameConflict(Inheritance, LocalName.Name, InstanceName.Name));
                            ConflictingEntry = true;
                        }
                    }

                    if (!ConflictingEntry)
                        MergedExportTable.Add(InstanceName, InstanceItem);
                    else
                        HasConflictingEntry = true;
                }
            }

            if (!HasConflictingEntry)
            {
                foreach (IExport Export in node.ExportList)
                {
                    IList<string> ListedClassList = new List<string>();
                    IHashtableEx<IFeatureName, IIdentifier> ListedExportList = new HashtableEx<IFeatureName, IIdentifier>();
                    for (int i = 0; i < Export.ClassIdentifierList.Count; i++)
                    {
                        IIdentifier Identifier = Export.ClassIdentifierList[i];

                        Debug.Assert(Identifier.ValidText.IsAssigned);
                        string ValidText = Identifier.ValidText.Item;

                        if (ValidText.ToLower() == LanguageClasses.Any.Name.ToLower())
                            ListedClassList.Add(LanguageClasses.Any.Name);

                        else if (node.ImportedClassTable.ContainsKey(ValidText))
                            ListedClassList.Add(ValidText);

                        else if (FeatureName.TableContain(MergedExportTable, ValidText, out IFeatureName Key, out IHashtableEx<string, IClass> Item))
                            ListedExportList.Add(Key, Identifier);

                        else
                        {
                            AddSourceError(new ErrorUnknownIdentifier(Identifier, ValidText));
                            Success = false;
                        }
                    }

                    ListedIdentifiers = new List<IFeatureName>();

                    foreach (KeyValuePair<IFeatureName, IIdentifier> Entry in ListedExportList)
                    {
                        IFeatureName ExportName = Entry.Key;
                        IIdentifier Identifier = Entry.Value;

                        if (ListedIdentifiers.Contains(ExportName))
                        {
                            AddSourceError(new ErrorIdentifierAlreadyListed(Identifier, ExportName.Name));
                            Success = false;
                        }
                        else
                            ListedIdentifiers.Add(ExportName);
                    }

                    List<IHashtableEx<string, IClass>> OtherClassTableList = new List<IHashtableEx<string, IClass>>();
                    foreach (IFeatureName ExportName in ListedIdentifiers)
                    {
                        IHashtableEx<string, IClass> OtherClassTable = MergedExportTable[ExportName];
                        OtherClassTableList.Add(OtherClassTable);
                    }

                    IHashtableEx<string, IClass> FilledClassTable = new HashtableEx<string, IClass>();

                    foreach (IHashtableEx<string, IClass> OtherClassTable in OtherClassTableList)
                        ResolveAsExportIdentifier(OtherClassTable, FilledClassTable);

                    bool AllClassIdentifiersHandled = true;
                    for (int i = 0; i < Export.ClassIdentifierList.Count; i++)
                    {
                        IIdentifier Identifier = Export.ClassIdentifierList[i];
                        string ValidIdentifier = Identifier.ValidText.Item;

                        if (ValidIdentifier.ToLower() == LanguageClasses.Any.Name.ToLower() || node.ImportedClassTable.ContainsKey(ValidIdentifier))
                            if (!ResolveAsClassIdentifier(ValidIdentifier, node.ImportedClassTable, FilledClassTable, Identifier, ErrorList))
                            {
                                AllClassIdentifiersHandled = false;
                                Success = false;
                            }
                    }

                    if (AllClassIdentifiersHandled)
                    {
                        IHashtableEx<string, IClass> ClassTable = node.LocalExportTable[Export.ValidExportName.Item];
                        foreach (KeyValuePair<string, IClass> Entry in FilledClassTable)
                        {
                            string ClassIdentifier = Entry.Key;
                            IClass ExportClassItem = Entry.Value;

                            ClassTable.Add(ClassIdentifier, ExportClassItem);
                        }

                        Export.ExportClassTable.Item = FilledClassTable;
                    }
                }
            }

            if (Success)
                data = MergedExportTable;

            return Success;
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

        private bool ResolveAsClassIdentifier(string validClassIdentifier, IHashtableEx<string, IImportedClass> importedClassTable, IHashtableEx<string, IClass> classTable, IIdentifier source, IList<IError> errorList)
        {
            if (classTable.ContainsKey(validClassIdentifier))
            {
                errorList.Add(new ErrorIdentifierAlreadyListed(source, validClassIdentifier));
                return false;
            }

            if (validClassIdentifier.ToLower() != LanguageClasses.Any.Name.ToLower())
            {
                IImportedClass Imported = importedClassTable[validClassIdentifier];
                classTable.Add(validClassIdentifier, Imported.Item);
            }

            return true;
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
