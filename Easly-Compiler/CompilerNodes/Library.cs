namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler ILibrary.
    /// </summary>
    public interface ILibrary : BaseNode.ILibrary, INode, INodeWithReplicatedBlocks, ISource
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Library.ImportBlocks"/>.
        /// </summary>
        IList<IImport> ImportList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Library.ClassIdentifierBlocks"/>.
        /// </summary>
        IList<IIdentifier> ClassIdentifierList { get; }

        /// <summary>
        /// The library name, verified as valid.
        /// </summary>
        string ValidLibraryName { get; }

        /// <summary>
        /// The library source name, verified as valid.
        /// </summary>
        string ValidSourceName { get; }

        /// <summary>
        /// Table of imported classes.
        /// </summary>
        IHashtableEx<string, IImportedClass> ImportedClassTable { get; }

        /// <summary>
        /// List of libraries imported by this one.
        /// </summary>
        IList<ILibrary> ImportedLibraryList { get; }

        /// <summary>
        /// True if all imports have been resolved.
        /// </summary>
        bool IsResolved { get; }

        /// <summary>
        /// Validates the library name and library source name, and update <see cref="ValidLibraryName"/> and <see cref="ValidSourceName"/>.
        /// </summary>
        /// <param name="libraryTable">Table of valid library names and their sources, updated upon return.</param>
        /// <param name="validatedLibraryList">List of classes with valid names, updated upon return.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if library names are valid.</returns>
        bool CheckLibraryNames(IHashtableEx<string, IHashtableEx<string, ILibrary>> libraryTable, IList<ILibrary> validatedLibraryList, IList<IError> errorList);

        /// <summary>
        /// Initializes the list of classes belonging to the library.
        /// </summary>
        /// <param name="classTable">Valid class names.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if initialization succeeded.</returns>
        bool InitLibraryTables(IHashtableEx<string, IHashtableEx<string, IClass>> classTable, IList<IError> errorList);

        /// <summary>
        /// Resolves reference from libraries to classes and other libraries.
        /// </summary>
        /// <param name="libraryTable">The table of known libraries.</param>
        /// <param name="resolvedLibraryList">The list of libraries that have been resolved so far.</param>
        /// <param name="importChanged">Indicates that the import specifier has changed.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if the method succeeded.</returns>
        bool Resolve(IHashtableEx<string, IHashtableEx<string, ILibrary>> libraryTable, IList<ILibrary> resolvedLibraryList, ref bool importChanged, IList<IError> errorList);
    }

    /// <summary>
    /// Compiler ILibrary.
    /// </summary>
    public class Library : BaseNode.Library, ILibrary
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Library.ImportBlocks"/>.
        /// </summary>
        public IList<IImport> ImportList { get; } = new List<IImport>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Library.ClassIdentifierBlocks"/>.
        /// </summary>
        public IList<IIdentifier> ClassIdentifierList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyArgument">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyArgument, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyArgument)
            {
                case nameof(ImportBlocks):
                    TargetList = (IList)ImportList;
                    break;

                case nameof(ClassIdentifierBlocks):
                    TargetList = (IList)ClassIdentifierList;
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
        /// The library name, verified as valid.
        /// </summary>
        public string ValidLibraryName { get; private set; }

        /// <summary>
        /// The library source name, verified as valid.
        /// </summary>
        public string ValidSourceName { get; private set; }

        /// <summary>
        /// Table of imported classes.
        /// </summary>
        public IHashtableEx<string, IImportedClass> ImportedClassTable { get; } = new HashtableEx<string, IImportedClass>();

        /// <summary>
        /// List of libraries imported by this one.
        /// </summary>
        public IList<ILibrary> ImportedLibraryList { get; } = new List<ILibrary>();

        /// <summary>
        /// True if all imports have been resolved.
        /// </summary>
        public bool IsResolved { get { return ImportList.Count == 0; } }

        /// <summary>
        /// Validates the library name and library source name, and update <see cref="ValidLibraryName"/> and <see cref="ValidSourceName"/>.
        /// </summary>
        /// <param name="libraryTable">Table of valid library names and their sources, updated upon return.</param>
        /// <param name="validatedLibraryList">List of classes with valid names, updated upon return.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if library names are valid.</returns>
        public virtual bool CheckLibraryNames(IHashtableEx<string, IHashtableEx<string, ILibrary>> libraryTable, IList<ILibrary> validatedLibraryList, IList<IError> errorList)
        {
            IErrorStringValidity StringError;
            IName LibraryEntityName = (IName)EntityName;

            // Verify the library name is a valid string.
            if (!StringValidation.IsValidIdentifier(LibraryEntityName, EntityName.Text, out string ValidEntityName, out StringError))
            {
                errorList.Add(StringError);
                return false;
            }

            ValidLibraryName = ValidEntityName;

            if (FromIdentifier.IsAssigned)
            {
                // Verify the library source name is a valid string.
                IIdentifier LibraryFromIdentifier = (IIdentifier)FromIdentifier.Item;

                if (!StringValidation.IsValidIdentifier(LibraryFromIdentifier, FromIdentifier.Item.Text, out string ValidFromIdentifier, out StringError))
                {
                    errorList.Add(StringError);
                    return false;
                }

                ValidSourceName = ValidFromIdentifier;
            }
            else
                ValidSourceName = string.Empty;

            // Add this library with valid names to the list.
            validatedLibraryList.Add(this);

            if (libraryTable.ContainsKey(ValidLibraryName))
            {
                IHashtableEx<string, ILibrary> SourceNameTable = libraryTable[ValidLibraryName];

                if (SourceNameTable.ContainsKey(ValidSourceName))
                {
                    // Report a source name collision if the class has one.
                    if (FromIdentifier.IsAssigned)
                    {
                        errorList.Add(new ErrorDuplicateName(LibraryEntityName, ValidLibraryName));
                        return false;
                    }
                }

                else
                    SourceNameTable.Add(ValidSourceName, this);
            }
            else
            {
                IHashtableEx<string, ILibrary> SourceNameTable = new HashtableEx<string, ILibrary>
                {
                    { ValidSourceName, this }
                };

                libraryTable.Add(ValidLibraryName, SourceNameTable);
            }

            return true;
        }

        /// <summary>
        /// Initializes the list of classes belonging to the library.
        /// </summary>
        /// <param name="classTable">Valid class names.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if initialization succeeded.</returns>
        public virtual bool InitLibraryTables(IHashtableEx<string, IHashtableEx<string, IClass>> classTable, IList<IError> errorList)
        {
            bool Success = true;

            foreach (IIdentifier ClassIdentifier in ClassIdentifierList)
            {
                // Verify the class identifier is a valid string.
                if (!StringValidation.IsValidIdentifier(ClassIdentifier, ClassIdentifier.Text, out string ValidClassIdentifier, out IErrorStringValidity StringError))
                {
                    Success = false;
                    errorList.Add(StringError);
                    continue;
                }

                if (ValidClassIdentifier.ToLower() == LanguageClasses.Any.Name.ToLower())
                {
                    // 'Any' is implicit and just ignored.
                }
                else
                {
                    // Check that the class name is known.
                    if (!classTable.ContainsKey(ValidClassIdentifier))
                    {
                        Success = false;
                        errorList.Add(new ErrorUnknownIdentifier(ClassIdentifier, ValidClassIdentifier));
                        continue;
                    }

                    IHashtableEx<string, IClass> SourceNameTable = classTable[ValidClassIdentifier];

                    // And it's from the same source.
                    if (!SourceNameTable.ContainsKey(ValidSourceName))
                    {
                        Success = false;
                        errorList.Add(new ErrorUnknownIdentifier(ClassIdentifier, ValidClassIdentifier));
                        continue;
                    }

                    // The class must be imported only once.
                    if (ImportedClassTable.ContainsKey(ValidClassIdentifier))
                    {
                        Success = false;
                        errorList.Add(new ErrorIdentifierAlreadyListed(ClassIdentifier, ValidClassIdentifier));
                        continue;
                    }

                    // Add it, leaving the import mode open for now.
                    IImportedClass Imported = new ImportedClass(SourceNameTable[ValidSourceName]);
                    ImportedClassTable.Add(ValidClassIdentifier, Imported);

#if DEBUG
                    // For code coverage purpose
                    string ImportString = Imported.ToString();
#endif
                }
            }

            Debug.Assert(Success || errorList.Count > 0);
            return Success;
        }

        /// <summary>
        /// Resolves reference from libraries to classes and other libraries.
        /// </summary>
        /// <param name="libraryTable">The table of known libraries.</param>
        /// <param name="resolvedLibraryList">The list of libraries that have been resolved so far.</param>
        /// <param name="importChanged">Indicates that the import specifier has changed.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if the method succeeded.</returns>
        public virtual bool Resolve(IHashtableEx<string, IHashtableEx<string, ILibrary>> libraryTable, IList<ILibrary> resolvedLibraryList, ref bool importChanged, IList<IError> errorList)
        {
            List<IImport> ToRemove = new List<IImport>();
            bool Success = true;

            foreach (IImport ImportItem in ImportList)
            {
                // Check the import and obtain the matching library.
                if (!ImportItem.CheckImportConsistency(libraryTable, out ILibrary MatchingLibrary, errorList))
                {
                    Success = false;
                    continue;
                }

                // You can't import the same library twice.
                if (ImportedLibraryList.Contains(MatchingLibrary))
                {
                    Success = false;
                    errorList.Add(new ErrorDuplicateImport((IIdentifier)ImportItem.LibraryIdentifier, MatchingLibrary.ValidLibraryName, MatchingLibrary.ValidSourceName));
                    continue;
                }

                // If the imported library hasn't been resolved yet, ignore it for now.
                if (!resolvedLibraryList.Contains(MatchingLibrary))
                    continue;

                // The imported library was resolved, merge this import with it.
                if (!MergeImports(ImportedClassTable, ImportItem, MatchingLibrary, errorList))
                {
                    Success = false;
                    continue;
                }

                ImportedLibraryList.Add(MatchingLibrary);
                ToRemove.Add(ImportItem);
                importChanged = true;
            }

            foreach (IImport ImportItem in ToRemove)
                ImportList.Remove(ImportItem);

            Debug.Assert(Success || errorList.Count > 0);
            return Success;
        }

        /// <summary>
        /// Merges an import clause with already imported classes.
        /// </summary>
        /// <param name="importedClassTable">Already imported classes.</param>
        /// <param name="importItem">The merged import.</param>
        /// <param name="matchingLibrary">The library referenced by <paramref name="importItem"/>.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if the merge succeeded.</returns>
        public static bool MergeImports(IHashtableEx<string, IImportedClass> importedClassTable, IImport importItem, ILibrary matchingLibrary, IList<IError> errorList)
        {
            // Clone imported class objects from the imported library.
            IHashtableEx<string, IImportedClass> MergedClassTable = new HashtableEx<string, IImportedClass>();
            foreach (KeyValuePair<string, IImportedClass> Entry in matchingLibrary.ImportedClassTable)
            {
                IImportedClass Clone = new ImportedClass(Entry.Value);
                MergedClassTable.Add(Entry.Key, Clone);
            }

            if (!importItem.CheckRenames(MergedClassTable, errorList))
                return false;

            if (!Class.MergeClassTables(importedClassTable, MergedClassTable, importItem, importItem.Type, errorList))
                return false;

            return true;
        }
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            if (ValidLibraryName != null)
                return $"Library '{ValidLibraryName}'";
            else
                return $"Library '{EntityName.Text}'";
        }
        #endregion
    }
}
