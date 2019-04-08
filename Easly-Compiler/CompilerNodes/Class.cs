namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IClass.
    /// </summary>
    public interface IClass : BaseNode.IClass, INode, INodeWithReplicatedBlocks, ISource, IScopeHolder
    {
        /// <summary>
        /// The class path with replication info.
        /// </summary>
        string FullClassPath { get; }

        /// <summary>
        /// Initializes the class path.
        /// </summary>
        void SetFullClassPath();

        /// <summary>
        /// Initializes the class path with replication info.
        /// </summary>
        /// <param name="replicationPattern">The replication pattern used.</param>
        /// <param name="source">The source text.</param>
        void SetFullClassPath(string replicationPattern, string source);

        /// <summary>
        /// The class-specific counter, for the <see cref="BaseNode.PreprocessorMacro.Counter"/> macro.
        /// </summary>
        int ClassCounter { get; }

        /// <summary>
        /// Increments <see cref="ClassCounter"/>.
        /// </summary>
        void IncrementClassCounter();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.InvariantBlocks"/>.
        /// </summary>
        IList<IAssertion> InvariantList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ConversionBlocks"/>.
        /// </summary>
        IList<IIdentifier> ConversionList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.FeatureBlocks"/>.
        /// </summary>
        IList<IFeature> FeatureList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ClassReplicateBlocks"/>.
        /// </summary>
        IList<IClassReplicate> ClassReplicateList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.DiscreteBlocks"/>.
        /// </summary>
        IList<IDiscrete> DiscreteList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.InheritanceBlocks"/>.
        /// </summary>
        IList<IInheritance> InheritanceList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.TypedefBlocks"/>.
        /// </summary>
        IList<ITypedef> TypedefList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ExportBlocks"/>.
        /// </summary>
        IList<IExport> ExportList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ImportBlocks"/>.
        /// </summary>
        IList<IImport> ImportList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.GenericBlocks"/>.
        /// </summary>
        IList<IGeneric> GenericList { get; }
        /// <summary>
        /// The class name, verified as valid.
        /// </summary>
        string ValidClassName { get; }

        /// <summary>
        /// The class source name, verified as valid.
        /// </summary>
        string ValidSourceName { get; }

        /// <summary>
        /// The list of imported libraries.
        /// </summary>
        IList<ILibrary> ImportedLibraryList { get; }

        /// <summary>
        /// The table of imported classes.
        /// </summary>
        IHashtableEx<string, IImportedClass> ImportedClassTable { get; }

        /// <summary>
        /// Validates the class name and class source name, and update <see cref="ValidClassName"/> and <see cref="ValidSourceName"/>.
        /// </summary>
        /// <param name="classTable">Table of valid class names and their sources, updated upon return.</param>
        /// <param name="validatedClassList">List of classes with valid names, updated upon return.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if class names are valid.</returns>
        bool CheckClassNames(IHashtableEx<string, IHashtableEx<string, IClass>> classTable, IList<IClass> validatedClassList, IList<IError> errorList);

        /// <summary>
        /// Validate a class import clauses.
        /// </summary>
        /// <param name="libraryTable">Imported libraries.</param>
        /// <param name="classTable">Imported classes.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if imports are valid.</returns>
        bool CheckClassConsistency(IHashtableEx<string, IHashtableEx<string, ILibrary>> libraryTable, IHashtableEx<string, IHashtableEx<string, IClass>> classTable, IList<IError> errorList);
    }

    /// <summary>
    /// Compiler IClass.
    /// </summary>
    public class Class : BaseNode.Class, IClass
    {
        #region Compiler
        /// <summary>
        /// The class path with replication info.
        /// </summary>
        public string FullClassPath { get; private set; }

        /// <summary>
        /// Initializes the class path.
        /// </summary>
        public void SetFullClassPath()
        {
            FullClassPath = ClassPath;
        }

        /// <summary>
        /// Initializes the class path with replication info.
        /// </summary>
        /// <param name="replicationPattern">The replication pattern used.</param>
        /// <param name="source">The source text.</param>
        public void SetFullClassPath(string replicationPattern, string source)
        {
            FullClassPath = $"{ClassPath};{replicationPattern}={source}";
        }

        /// <summary>
        /// The class-specific counter, for the <see cref="BaseNode.PreprocessorMacro.Counter"/> macro.
        /// </summary>
        public int ClassCounter { get; private set; }

        /// <summary>
        /// Increments <see cref="ClassCounter"/>.
        /// </summary>
        public virtual void IncrementClassCounter()
        {
            ClassCounter++;
        }
        #endregion

        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.InvariantBlocks"/>.
        /// </summary>
        public IList<IAssertion> InvariantList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ConversionBlocks"/>.
        /// </summary>
        public IList<IIdentifier> ConversionList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.FeatureBlocks"/>.
        /// </summary>
        public IList<IFeature> FeatureList { get; } = new List<IFeature>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ClassReplicateBlocks"/>.
        /// </summary>
        public IList<IClassReplicate> ClassReplicateList { get; } = new List<IClassReplicate>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.DiscreteBlocks"/>.
        /// </summary>
        public IList<IDiscrete> DiscreteList { get; } = new List<IDiscrete>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.InheritanceBlocks"/>.
        /// </summary>
        public IList<IInheritance> InheritanceList { get; } = new List<IInheritance>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.TypedefBlocks"/>.
        /// </summary>
        public IList<ITypedef> TypedefList { get; } = new List<ITypedef>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ExportBlocks"/>.
        /// </summary>
        public IList<IExport> ExportList { get; } = new List<IExport>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ImportBlocks"/>.
        /// </summary>
        public IList<IImport> ImportList { get; } = new List<IImport>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.GenericBlocks"/>.
        /// </summary>
        public IList<IGeneric> GenericList { get; } = new List<IGeneric>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyName">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyName, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyName)
            {
                case nameof(InvariantBlocks):
                    TargetList = (IList)InvariantList;
                    break;

                case nameof(ConversionBlocks):
                    TargetList = (IList)ConversionList;
                    break;

                case nameof(FeatureBlocks):
                    TargetList = (IList)FeatureList;
                    break;

                case nameof(ClassReplicateBlocks):
                    TargetList = (IList)ClassReplicateList;
                    break;

                case nameof(DiscreteBlocks):
                    TargetList = (IList)DiscreteList;
                    break;

                case nameof(InheritanceBlocks):
                    TargetList = (IList)InheritanceList;
                    break;

                case nameof(TypedefBlocks):
                    TargetList = (IList)TypedefList;
                    break;

                case nameof(ExportBlocks):
                    TargetList = (IList)ExportList;
                    break;

                case nameof(ImportBlocks):
                    TargetList = (IList)ImportList;
                    break;

                case nameof(GenericBlocks):
                    TargetList = (IList)GenericList;
                    break;
            }

            Debug.Assert(TargetList != null);
            Debug.Assert(TargetList.Count == 0);

            foreach (BaseNode.INode Node in nodeList)
                TargetList.Add(Node);
        }
        #endregion

        #region Implementation of ISource
        /// <summary>
        /// The parent node, null if root.
        /// </summary>
        public ISource ParentSource { get; private set; }

        /// <summary>
        /// The parent class, null if none.
        /// </summary>
        public IClass EmbeddingClass { get; private set; }

        /// <summary>
        /// The parent feature, null if none.
        /// </summary>
        public IFeature EmbeddingFeature { get; private set; }

        /// <summary>
        /// The parent overload, null if none.
        /// </summary>
        public IQueryOverload EmbeddingOverload { get; private set; }

        /// <summary>
        /// The parent body, null if none.
        /// </summary>
        public IBody EmbeddingBody { get; private set; }

        /// <summary>
        /// The parent assertion, null if none.
        /// </summary>
        public IAssertion EmbeddingAssertion { get; private set; }

        /// <summary>
        /// Initializes parents based on the provided <paramref name="parentSource"/> node.
        /// </summary>
        /// <param name="parentSource">The parent node.</param>
        public virtual void InitializeSource(ISource parentSource)
        {
            ParentSource = parentSource;

            EmbeddingClass = parentSource is IClass AsClass ? AsClass : parentSource?.EmbeddingClass;
            EmbeddingFeature = parentSource is IFeature AsFeature ? AsFeature : parentSource?.EmbeddingFeature;
            EmbeddingOverload = parentSource is IQueryOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
            EmbeddingBody = parentSource is IBody AsBody ? AsBody : parentSource?.EmbeddingBody;
            EmbeddingAssertion = parentSource is IAssertion AsAssertion ? AsAssertion : parentSource?.EmbeddingAssertion;
        }
        #endregion

        #region Classes and Libraries name collision check
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
        public IHashtableEx<string, IImportedClass> ImportedClassTable { get; } = new HashtableEx<string, IImportedClass>();

        /// <summary>
        /// Validates the class name and class source name, and update <see cref="ValidClassName"/> and <see cref="ValidSourceName"/>.
        /// </summary>
        /// <param name="classTable">Table of valid class names and their sources, updated upon return.</param>
        /// <param name="validatedClassList">List of classes with valid names, updated upon return.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if class names are valid.</returns>
        public virtual bool CheckClassNames(IHashtableEx<string, IHashtableEx<string, IClass>> classTable, IList<IClass> validatedClassList, IList<IError> errorList)
        {
            IName ClassEntityName = (IName)EntityName;

            // Verify the class name is a valid string.
            if (!StringValidation.IsValidIdentifier(ClassEntityName, EntityName.Text, out string ValidEntityName, out IErrorStringValidity StringError))
            {
                errorList.Add(StringError);
                return false;
            }

            ValidClassName = ValidEntityName;

            if (FromIdentifier.IsAssigned)
            {
                // Verify the class source name is a valid string.
                IIdentifier ClassFromIdentifier = (IIdentifier)FromIdentifier.Item;

                if (!StringValidation.IsValidIdentifier(ClassFromIdentifier, FromIdentifier.Item.Text, out string ValidFromIdentifier, out StringError))
                {
                    errorList.Add(StringError);
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
                IHashtableEx<string, IClass> SourceNameTable = classTable[ValidClassName];

                if (SourceNameTable.ContainsKey(ValidSourceName))
                {
                    // Report a source name collision if the class has one.
                    if (FromIdentifier.IsAssigned)
                    {
                        errorList.Add(new ErrorDuplicateName(ClassEntityName, ValidClassName));
                        return false;
                    }
                }

                else
                    SourceNameTable.Add(ValidSourceName, this);
            }
            else
            {
                IHashtableEx<string, IClass> SourceNameTable = new HashtableEx<string, IClass>
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
        public virtual bool CheckClassConsistency(IHashtableEx<string, IHashtableEx<string, ILibrary>> libraryTable, IHashtableEx<string, IHashtableEx<string, IClass>> classTable, IList<IError> errorList)
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
                    errorList.Add(new ErrorDuplicateImport((IIdentifier)ImportItem.LibraryIdentifier, MatchingLibrary.ValidLibraryName, MatchingLibrary.ValidSourceName));
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
                        errorList.Add(new ErrorNameChanged(Imported.ImportLocation, ValidClassName, NewName));
                    }

                    if (Imported.IsTypeAssigned && Imported.ImportType != BaseNode.ImportType.Latest)
                    {
                        Success = false;
                        errorList.Add(new ErrorImportTypeConflict(Imported.ImportLocation, ValidClassName));
                    }

                    break;
                }
            }

            // If not referenced by an imported library, a class should be able to reference itself.
            if (!ImportedClassTable.ContainsKey(ValidClassName))
            {
                IImportedClass SelfImport = new ImportedClass(this, BaseNode.ImportType.Latest);
                ImportedClassTable.Add(ValidClassName, SelfImport);
            }

            foreach (KeyValuePair<string, IImportedClass> Entry in ImportedClassTable)
            {
                IImportedClass Imported = Entry.Value;
                Imported.SetParentSource(this);
            }

            ImportedClassTable.Seal();

            Debug.Assert(Success || errorList.Count > 0);
            return Success;
        }

        /// <summary>
        /// Merges a class import with previous imports.
        /// </summary>
        /// <param name="importedClassTable">The already reoved imports.</param>
        /// <param name="locallyImportedClassTable">The current import.</param>
        /// <param name="importLocation">The import location.</param>
        /// <param name="localImportType">The import specification.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if the merge is successful.</returns>
        public static bool MergeClassTables(IHashtableEx<string, IImportedClass> importedClassTable, IHashtableEx<string, IImportedClass> locallyImportedClassTable, IImport importLocation, BaseNode.ImportType localImportType, IList<IError> errorList)
        {
            bool AllClassesAdded = true;

            foreach (KeyValuePair<string, IImportedClass> Entry in locallyImportedClassTable)
            {
                string ClassName = Entry.Key;
                IImportedClass LocalClassItem = Entry.Value;

                if (importedClassTable.ContainsKey(ClassName))
                {
                    IImportedClass ClassItem = importedClassTable[ClassName];
                    if (ClassItem.Item != LocalClassItem.Item)
                    {
                        errorList.Add(new ErrorNameAlreadyUsed(importLocation, ClassName));
                        AllClassesAdded = false;
                        continue;
                    }

                    Debug.Assert(ClassItem.IsTypeAssigned);

                    if (LocalClassItem.IsTypeAssigned)
                    {
                        if (ClassItem.ImportType != LocalClassItem.ImportType)
                        {
                            errorList.Add(new ErrorImportTypeConflict(importLocation, ClassName));
                            AllClassesAdded = false;
                            continue;
                        }
                    }
                    else
                    {
                        if (ClassItem.ImportType != localImportType)
                        {
                            errorList.Add(new ErrorImportTypeConflict(importLocation, ClassName));
                            AllClassesAdded = false;
                            continue;
                        }

                        if (!ClassItem.IsLocationAssigned)
                            ClassItem.SetImportLocation(importLocation);
                    }
                }
                else
                {
                    bool AlreadyImported = false;
                    foreach (KeyValuePair<string, IImportedClass> ImportedEntry in importedClassTable)
                    {
                        IImportedClass ClassItem = ImportedEntry.Value;
                        if (ClassItem.Item == LocalClassItem.Item)
                        {
                            string OldName = ImportedEntry.Key;
                            errorList.Add(new ErrorClassAlreadyImported(importLocation, OldName, ClassName));
                            AlreadyImported = true;
                            break;
                        }
                    }

                    if (AlreadyImported)
                    {
                        AllClassesAdded = false;
                        continue;
                    }

                    if (LocalClassItem.IsTypeAssigned)
                    {
                        if (LocalClassItem.ImportType != localImportType)
                        {
                            errorList.Add(new ErrorImportTypeConflict(importLocation, ClassName));
                            AllClassesAdded = false;
                        }
                    }
                    else
                    {
                        LocalClassItem.SetImportType(localImportType);
                        LocalClassItem.SetImportLocation(importLocation);
                    }

                    importedClassTable.Add(ClassName, LocalClassItem);
                }
            }

            Debug.Assert(AllClassesAdded || errorList.Count > 0);
            return AllClassesAdded;
        }
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            if (ValidClassName != null)
                return $"Class '{ValidClassName}'";
            else
                return $"Class '{EntityName.Text}'";
        }
        #endregion
    }
}
