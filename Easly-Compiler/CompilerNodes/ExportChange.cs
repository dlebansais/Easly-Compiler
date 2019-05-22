namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IExportChange.
    /// </summary>
    public interface IExportChange : BaseNode.IExportChange, INode, INodeWithReplicatedBlocks, ISource
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.ExportChange.IdentifierBlocks"/>.
        /// </summary>
        IList<IIdentifier> IdentifierList { get; }

        /// <summary>
        /// Table of valid identifiers.
        /// </summary>
        IHashtableEx<string, IIdentifier> IdentifierTable { get; }

        /// <summary>
        /// Valid export identifier.
        /// </summary>
        OnceReference<string> ValidExportIdentifier { get; }

        /// <summary>
        /// Apply changes in this instance to arguments.
        /// </summary>
        /// <param name="importedClassTable">The table of imported classes</param>
        /// <param name="exportTable">The list of exports to change.</param>
        /// <param name="errorList">The list of errors found.</param>
        bool ApplyChange(IHashtableEx<string, IImportedClass> importedClassTable, IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> exportTable, IErrorList errorList);
    }

    /// <summary>
    /// Compiler IExportChange.
    /// </summary>
    public class ExportChange : BaseNode.ExportChange, IExportChange
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.ExportChange.IdentifierBlocks"/>.
        /// </summary>
        public IList<IIdentifier> IdentifierList { get; } = new List<IIdentifier>();

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
                case nameof(IdentifierBlocks):
                    TargetList = (IList)IdentifierList;
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
                IdentifierTable = new HashtableEx<string, IIdentifier>();
                ValidExportIdentifier = new OnceReference<string>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types || ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
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
                IsResolved = IdentifierTable.IsSealed;
                Debug.Assert(ValidExportIdentifier.IsAssigned == IsResolved);
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Table of valid identifiers.
        /// </summary>
        public IHashtableEx<string, IIdentifier> IdentifierTable { get; private set; } = new HashtableEx<string, IIdentifier>();

        /// <summary>
        /// Valid export identifier.
        /// </summary>
        public OnceReference<string> ValidExportIdentifier { get; private set; } = new OnceReference<string>();

        /// <summary>
        /// Apply changes in this instance to arguments.
        /// </summary>
        /// <param name="importedClassTable">The table of imported classes</param>
        /// <param name="exportTable">The list of exports to change.</param>
        /// <param name="errorList">The list of errors found.</param>
        public virtual bool ApplyChange(IHashtableEx<string, IImportedClass> importedClassTable, IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> exportTable, IErrorList errorList)
        {
            if (!ExportIdentifierExists(exportTable, errorList, out IFeatureName CurrentExportName, out IHashtableEx<string, IClass> CurrentClassTable))
                return false;

            IHashtableEx<string, IHashtableEx<string, IClass>> ListedExportTable = new HashtableEx<string, IHashtableEx<string, IClass>>(); // string (export name) -> hashtable // string (class name) -> Class
            IHashtableEx<string, IClass> ListedClassTable = new HashtableEx<string, IClass>(); // string (class name) -> Class

            bool InvalidExportChange = false;
            foreach (IIdentifier IdentifierItem in IdentifierList)
            {
                Debug.Assert(IdentifierItem.ValidText.IsAssigned);
                string ValidIdentifier = IdentifierItem.ValidText.Item;

                if (FeatureName.TableContain(exportTable, ValidIdentifier, out IFeatureName EntryName, out IHashtableEx<string, IClass> ListedExport))
                {
                    Debug.Assert(!ListedExportTable.ContainsKey(ValidIdentifier));
                    ListedExportTable.Add(ValidIdentifier, ListedExport);
                }
                else if (importedClassTable.ContainsKey(ValidIdentifier))
                {
                    Debug.Assert(!ListedClassTable.ContainsKey(ValidIdentifier));
                    ListedClassTable.Add(ValidIdentifier, importedClassTable[ValidIdentifier].Item);
                }
                else if (ValidIdentifier.ToLower() != LanguageClasses.Any.Name.ToLower())
                {
                    errorList.AddError(new ErrorUnknownIdentifier(IdentifierItem, ValidIdentifier));
                    InvalidExportChange = true;
                }
            }

            if (InvalidExportChange)
                return false;

            UpdateTables(exportTable, CurrentExportName, CurrentClassTable, ListedExportTable, ListedClassTable);

            return true;
        }

        private bool ExportIdentifierExists(IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> exportTable, IErrorList errorList, out IFeatureName currentExportName, out IHashtableEx<string, IClass> currentClassTable)
        {
            currentExportName = null;
            currentClassTable = null;
            bool Found = false;

            IIdentifier NodeExportIdentifier = (IIdentifier)ExportIdentifier;
            Debug.Assert(NodeExportIdentifier.ValidText.IsAssigned);
            string ValidIdentifier = NodeExportIdentifier.ValidText.Item;

            foreach (KeyValuePair<IFeatureName, IHashtableEx<string, IClass>> Entry in exportTable)
            {
                IFeatureName EntryName = Entry.Key;
                if (EntryName.Name == ValidIdentifier)
                {
                    currentExportName = EntryName;
                    currentClassTable = Entry.Value;
                    Found = true;
                    break;
                }
            }

            if (!Found)
                errorList.AddError(new ErrorUnknownIdentifier(NodeExportIdentifier, ValidIdentifier));

            return Found;
        }

        private void UpdateTables(IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> exportTable, IFeatureName currentExportName, IHashtableEx<string, IClass> currentClassTable, IHashtableEx<string, IHashtableEx<string, IClass>> listedExportTable, IHashtableEx<string, IClass> listedClassTable)
        {
            IHashtableEx<string, IClass> ChangedClassTable = new HashtableEx<string, IClass>();

            foreach (KeyValuePair<string, IClass> ListedEntry in currentClassTable)
            {
                string ClassIdentifier = ListedEntry.Key;
                IClass ListedClass = ListedEntry.Value;

                if (!ChangedClassTable.ContainsKey(ClassIdentifier))
                    ChangedClassTable.Add(ClassIdentifier, ListedClass);
            }

            foreach (KeyValuePair<string, IHashtableEx<string, IClass>> Entry in listedExportTable)
            {
                IHashtableEx<string, IClass> ClassTable = Entry.Value;

                foreach (KeyValuePair<string, IClass> ListedEntry in ClassTable)
                {
                    string ClassIdentifier = ListedEntry.Key;
                    IClass ListedClass = ListedEntry.Value;

                    if (!ChangedClassTable.ContainsKey(ClassIdentifier))
                        ChangedClassTable.Add(ClassIdentifier, ListedClass);
                }
            }

            foreach (KeyValuePair<string, IClass> ListedEntry in listedClassTable)
            {
                string ClassIdentifier = ListedEntry.Key;
                IClass ListedClass = ListedEntry.Value;

                if (!ChangedClassTable.ContainsKey(ClassIdentifier))
                    ChangedClassTable.Add(ClassIdentifier, ListedClass);
            }

            exportTable[currentExportName] = ChangedClassTable;
        }
        #endregion
    }
}
