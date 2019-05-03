namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IImport.
    /// </summary>
    public interface IImport : BaseNode.IImport, INode, INodeWithReplicatedBlocks, ISource
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Import.RenameBlocks"/>.
        /// </summary>
        IList<IRename> RenameList { get; }

        /// <summary>
        /// Validates an import and return the matching library.
        /// </summary>
        /// <param name="libraryTable">Table of valid library names and their sources, updated upon return.</param>
        /// <param name="matchingLibrary">The matching library upon return.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if library names are valid.</returns>
        bool CheckImportConsistency(IHashtableEx<string, IHashtableEx<string, ILibrary>> libraryTable, out ILibrary matchingLibrary, IErrorList errorList);

        /// <summary>
        /// Check all rename clauses separately.
        /// </summary>
        /// <param name="importedClassTable">Table of imported classes.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if all rename clauses are valid.</returns>
        bool CheckRenames(IHashtableEx<string, IImportedClass> importedClassTable, IErrorList errorList);
    }

    /// <summary>
    /// Compiler IImport.
    /// </summary>
    public class Import : BaseNode.Import, IImport
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Import.RenameBlocks"/>.
        /// </summary>
        public IList<IRename> RenameList { get; } = new List<IRename>();

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
                case nameof(RenameBlocks):
                    TargetList = (IList)RenameList;
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
        public IOverload EmbeddingOverload { get; private set; }

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
            EmbeddingOverload = parentSource is IOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
            EmbeddingBody = parentSource is IBody AsBody ? AsBody : parentSource?.EmbeddingBody;
            EmbeddingAssertion = parentSource is IAssertion AsAssertion ? AsAssertion : parentSource?.EmbeddingAssertion;
        }

        /// <summary>
        /// Reset some intermediate results.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to reset.</param>
        public void Reset(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsResolved = false;

            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Classes and Libraries name collision check
        /// <summary>
        /// Validates an import and return the matching library.
        /// </summary>
        /// <param name="libraryTable">Table of valid library names and their sources, updated upon return.</param>
        /// <param name="matchingLibrary">The matching library upon return.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if library names are valid.</returns>
        public virtual bool CheckImportConsistency(IHashtableEx<string, IHashtableEx<string, ILibrary>> libraryTable, out ILibrary matchingLibrary, IErrorList errorList)
        {
            IErrorStringValidity StringError;
            string ValidFromIdentifier;
            IIdentifier ImportLibraryIdentifier = (IIdentifier)LibraryIdentifier;
            IHashtableEx<string, ILibrary> SourceNameTable;

            matchingLibrary = null;

            if (!StringValidation.IsValidIdentifier(ImportLibraryIdentifier, LibraryIdentifier.Text, out string ValidLibraryIdentifier, out StringError))
            {
                errorList.AddError(StringError);
                return false;
            }

            // Match the library name and source name.
            if (!libraryTable.ContainsKey(ValidLibraryIdentifier))
            {
                errorList.AddError(new ErrorUnknownIdentifier(ImportLibraryIdentifier, ValidLibraryIdentifier));
                return false;
            }

            SourceNameTable = libraryTable[ValidLibraryIdentifier];

            if (FromIdentifier.IsAssigned)
            {
                IIdentifier ImportFromIdentifier = (IIdentifier)FromIdentifier.Item;

                if (!StringValidation.IsValidIdentifier(ImportFromIdentifier, FromIdentifier.Item.Text, out ValidFromIdentifier, out StringError))
                {
                    errorList.AddError(StringError);
                    return false;
                }
            }
            else
                ValidFromIdentifier = string.Empty;

            if (!SourceNameTable.ContainsKey(ValidFromIdentifier))
            {
                errorList.AddError(new ErrorUnknownIdentifier(ImportLibraryIdentifier, ValidLibraryIdentifier));
                return false;
            }

            matchingLibrary = SourceNameTable[ValidFromIdentifier];
            return true;
        }

        /// <summary>
        /// Check all rename clauses separately.
        /// </summary>
        /// <param name="importedClassTable">Table of imported classes.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if all rename clauses are valid.</returns>
        public virtual bool CheckRenames(IHashtableEx<string, IImportedClass> importedClassTable, IErrorList errorList)
        {
            IHashtableEx<string, string> SourceIdentifierTable = new HashtableEx<string, string>(); // string (source) -> string (destination)
            IHashtableEx<string, string> DestinationIdentifierTable = new HashtableEx<string, string>(); // string (destination) -> string (source)

            bool Success = true;
            foreach (IRename RenameItem in RenameList)
                Success &= RenameItem.CheckGenericRename(new IHashtableIndex<string>[] { importedClassTable }, SourceIdentifierTable, DestinationIdentifierTable, (string key) => key, (string s) => s, errorList);

            Debug.Assert(Success || !errorList.IsEmpty);
            return Success;
        }
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return $"Import '{LibraryIdentifier.Text}', {Type}, {RenameList.Count} rename(s)";
        }
        #endregion
    }
}
