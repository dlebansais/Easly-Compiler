namespace CompilerNode
{
    using Easly;
    using EaslyCompiler;
    using System.Collections.Generic;
    using System.Diagnostics;
    /// <summary>
    /// Compiler IClass.
    /// </summary>
    public partial class Class : BaseNode.Class, IClass
    {
        /// <summary>
        /// The class name, verified as valid.
        /// </summary>
        public string ValidClassName { get; private set; }

        /// <summary>
        /// The class source name, verified as valid.
        /// </summary>
        public string ValidSourceName { get; private set; }

        /// <summary>
        /// The list of imported libraries.
        /// </summary>
        public IList<ILibrary> ImportedLibraryList { get; } = new List<ILibrary>();

        /// <summary>
        /// The table of imported classes.
        /// </summary>
        public ISealableDictionary<string, IImportedClass> ImportedClassTable { get; } = new SealableDictionary<string, IImportedClass>();

        /// <summary>
        /// Validates the class name and class source name, and update <see cref="ValidClassName"/> and <see cref="ValidSourceName"/>.
        /// </summary>
        /// <param name="classTable">Table of valid class names and their sources, updated upon return.</param>
        /// <param name="validatedClassList">List of classes with valid names, updated upon return.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if class names are valid.</returns>
        public virtual bool CheckClassNames(ISealableDictionary<string, ISealableDictionary<string, IClass>> classTable, IList<IClass> validatedClassList, IErrorList errorList)
        {
            IName ClassEntityName = (IName)EntityName;

            // Verify the class name is a valid string.
            if (!StringValidation.IsValidIdentifier(ClassEntityName, EntityName.Text, out string ValidEntityName, out IErrorStringValidity StringError))
            {
                errorList.AddError(StringError);
                return false;
            }

            ValidClassName = ValidEntityName;

            if (FromIdentifier.IsAssigned)
            {
                // Verify the class source name is a valid string.
                IIdentifier ClassFromIdentifier = (IIdentifier)FromIdentifier.Item;

                if (!StringValidation.IsValidIdentifier(ClassFromIdentifier, FromIdentifier.Item.Text, out string ValidFromIdentifier, out StringError))
                {
                    errorList.AddError(StringError);
                    return false;
                }

                ValidSourceName = ValidFromIdentifier;
            }
            else
                ValidSourceName = string.Empty;

            // Add this class with valid names to the list.
            validatedClassList.Add(this);

            if (classTable.ContainsKey(ValidClassName))
            {
                ISealableDictionary<string, IClass> SourceNameTable = classTable[ValidClassName];

                if (SourceNameTable.ContainsKey(ValidSourceName))
                {
                    // Report a source name collision if the class has one.
                    if (FromIdentifier.IsAssigned)
                    {
                        errorList.AddError(new ErrorDuplicateName(ClassEntityName, ValidClassName));
                        return false;
                    }
                }
                else
                    SourceNameTable.Add(ValidSourceName, this);
            }
            else
            {
                ISealableDictionary<string, IClass> SourceNameTable = new SealableDictionary<string, IClass>
                {
                    { ValidSourceName, this }
                };

                classTable.Add(ValidClassName, SourceNameTable);
            }

            return true;
        }

        /// <summary>
        /// Validates a class import clauses.
        /// </summary>
        /// <param name="libraryTable">Imported libraries.</param>
        /// <param name="classTable">Imported classes.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if imports are valid.</returns>
        public virtual bool CheckClassConsistency(ISealableDictionary<string, ISealableDictionary<string, ILibrary>> libraryTable, ISealableDictionary<string, ISealableDictionary<string, IClass>> classTable, IErrorList errorList)
        {
            bool Success = true;

            // Process all import clauses separately.
            foreach (IImport ImportItem in ImportList)
            {
                if (!ImportItem.CheckImportConsistency(libraryTable, out ILibrary MatchingLibrary, errorList))
                {
                    Success = false;
                    continue;
                }

                if (ImportedLibraryList.Contains(MatchingLibrary))
                {
                    Success = false;
                    errorList.AddError(new ErrorDuplicateImport((IIdentifier)ImportItem.LibraryIdentifier, MatchingLibrary.ValidLibraryName, MatchingLibrary.ValidSourceName));
                    continue;
                }

                if (!Library.MergeImports(ImportedClassTable, ImportItem, MatchingLibrary, errorList))
                {
                    Success = false;
                    continue;
                }

                ImportedLibraryList.Add(MatchingLibrary);
            }

            // Check import specifications.
            foreach (KeyValuePair<string, IImportedClass> Entry in ImportedClassTable)
            {
                IImportedClass Imported = Entry.Value;
                if (Imported.Item == this)
                {
                    string NewName = Entry.Key;

                    if (NewName != ValidClassName)
                    {
                        Success = false;
                        errorList.AddError(new ErrorNameChanged(Imported.ImportLocation, ValidClassName, NewName));
                    }

                    if (Imported.IsTypeAssigned && Imported.ImportType != BaseNode.ImportType.Latest)
                    {
                        Success = false;
                        errorList.AddError(new ErrorImportTypeConflict(Imported.ImportLocation, ValidClassName));
                    }

                    break;
                }
            }

            // If not referenced by an imported library, a class should be able to reference itself.
            if (!ImportedClassTable.ContainsKey(ValidClassName))
            {
                IImportedClass SelfImport = new ImportedClass(this, BaseNode.ImportType.Latest);
                ImportedClassTable.Add(ValidClassName, SelfImport);

#if COVERAGE
                string ImportString = SelfImport.ToString();
#endif
            }

            foreach (KeyValuePair<string, IImportedClass> Entry in ImportedClassTable)
            {
                IImportedClass Imported = Entry.Value;
                Imported.SetParentSource(this);
            }

            ImportedClassTable.Seal();

            Debug.Assert(Success || !errorList.IsEmpty);
            return Success;
        }

        /// <summary>
        /// Merges a class import with previous imports.
        /// </summary>
        /// <param name="importedClassTable">The already resolved imports.</param>
        /// <param name="mergedClassTable">The new classes to import.</param>
        /// <param name="importLocation">The import location.</param>
        /// <param name="mergedImportType">The import specification for all <paramref name="mergedClassTable"/>.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if the merge is successful.</returns>
        public static bool MergeClassTables(ISealableDictionary<string, IImportedClass> importedClassTable, ISealableDictionary<string, IImportedClass> mergedClassTable, IImport importLocation, BaseNode.ImportType mergedImportType, IErrorList errorList)
        {
            bool Success = true;

            foreach (KeyValuePair<string, IImportedClass> Entry in mergedClassTable)
            {
                string ClassName = Entry.Key;
                IImportedClass MergedClassItem = Entry.Value;

                // The merged class may have an import specification already, but if so it must match the one used here.
                // We can assume we use mergedImportType after this.
                if (MergedClassItem.IsTypeAssigned && MergedClassItem.ImportType != mergedImportType)
                {
                    errorList.AddError(new ErrorImportTypeConflict(importLocation, ClassName));
                    Success = false;
                }

                // If a class is already imported with this name somehow.
                if (importedClassTable.ContainsKey(ClassName))
                {
                    // It must be the same class.
                    IImportedClass ClassItem = importedClassTable[ClassName];
                    if (ClassItem.Item != MergedClassItem.Item)
                    {
                        errorList.AddError(new ErrorNameAlreadyUsed(importLocation, ClassName));
                        Success = false;
                        continue;
                    }

                    // If the already existing imported class has a specification, it must match the one use for merge.
                    if (ClassItem.IsTypeAssigned)
                    {
                        if (ClassItem.ImportType != mergedImportType)
                        {
                            errorList.AddError(new ErrorImportTypeConflict(importLocation, ClassName));
                            Success = false;
                            continue;
                        }
                    }
                    else
                        ClassItem.SetImportType(mergedImportType);

                    // If the import location isn't specified yet, use the imported library.
                    if (!ClassItem.IsLocationAssigned)
                        ClassItem.SetImportLocation(importLocation);
                }
                else
                {
                    // New class, at least by name. Make sure it's not an already imported class using a different name.
                    Success &= MergeClassTablesWithNewClass(importedClassTable, ClassName, MergedClassItem, importLocation, mergedImportType, errorList);
                }
            }

            Debug.Assert(Success || !errorList.IsEmpty);
            return Success;
        }

        private static bool MergeClassTablesWithNewClass(ISealableDictionary<string, IImportedClass> importedClassTable, string className, IImportedClass mergedClassItem, IImport importLocation, BaseNode.ImportType mergedImportType, IErrorList errorList)
        {
            bool Success = true;

            foreach (KeyValuePair<string, IImportedClass> ImportedEntry in importedClassTable)
            {
                IImportedClass ClassItem = ImportedEntry.Value;
                if (ClassItem.Item == mergedClassItem.Item)
                {
                    string OldName = ImportedEntry.Key;
                    errorList.AddError(new ErrorClassAlreadyImported(importLocation, OldName, className));
                    Success = false;
                    break;
                }
            }

            // First time this class is imported, use the merge import type specification and location since they are known.
            if (Success)
            {
                mergedClassItem.SetImportType(mergedImportType);
                mergedClassItem.SetImportLocation(importLocation);

                importedClassTable.Add(className, mergedClassItem);
            }

            return Success;
        }
    }
}
