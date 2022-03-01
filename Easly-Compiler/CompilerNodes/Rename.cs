namespace CompilerNode
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IRename.
    /// </summary>
    public interface IRename : INode, ISource
    {
        /// <summary>
        /// Gets or sets the feature to rename.
        /// </summary>
        BaseNode.Identifier SourceIdentifier { get; }

        /// <summary>
        /// Gets or sets the new name.
        /// </summary>
        BaseNode.Identifier DestinationIdentifier { get; }

        /// <summary>
        /// The valid value of <see cref="BaseNode.IRename.SourceIdentifier"/>.
        /// </summary>
        OnceReference<string> ValidSourceText { get; }

        /// <summary>
        /// The valid value of <see cref="BaseNode.IRename.DestinationIdentifier"/>.
        /// </summary>
        OnceReference<string> ValidDestinationText { get; }

        /// <summary>
        /// Checks and validates a rename clause.
        /// </summary>
        /// <typeparam name="TKey">The object type on which rename operates.</typeparam>
        /// <param name="renamedItemTables">Competing collections of renames this instance belongs to.</param>
        /// <param name="sourceIdentifierTable">Table of source to destination associations already resolved.</param>
        /// <param name="destinationIdentifierTable">Table of destination to source associations already resolved.</param>
        /// <param name="key2String">Provides the string from the key.</param>
        /// <param name="string2Key">Creates a key from a string.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if the rename is valid.</returns>
        bool CheckGenericRename<TKey>(IDictionaryIndex<TKey>[] renamedItemTables, ISealableDictionary<string, string> sourceIdentifierTable, ISealableDictionary<string, string> destinationIdentifierTable, Func<TKey, string> key2String, Func<string, TKey> string2Key, IErrorList errorList);
    }

    /// <summary>
    /// Compiler IRename.
    /// </summary>
    public class Rename : BaseNode.Rename, IRename
    {
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
        public void Reset(IRuleTemplateList ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                ValidSourceText = new OnceReference<string>();
                ValidDestinationText = new OnceReference<string>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types || ruleTemplateList == RuleTemplateSet.Contract || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IRuleTemplateList ruleTemplateList)
        {
            bool IsResolved = false;

            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsResolved = ValidSourceText.IsAssigned && ValidDestinationText.IsAssigned;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types || ruleTemplateList == RuleTemplateSet.Contract || ruleTemplateList == RuleTemplateSet.Body)
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
        /// The valid value of <see cref="BaseNode.IRename.SourceIdentifier"/>.
        /// </summary>
        public OnceReference<string> ValidSourceText { get; private set; } = new OnceReference<string>();

        /// <summary>
        /// The valid value of <see cref="BaseNode.IRename.DestinationIdentifier"/>.
        /// </summary>
        public OnceReference<string> ValidDestinationText { get; private set; } = new OnceReference<string>();

        /// <summary>
        /// Checks and validates a rename clause.
        /// </summary>
        /// <typeparam name="TKey">The object type on which rename operates.</typeparam>
        /// <param name="renamedItemTables">Competing collections of renames this instance belongs to.</param>
        /// <param name="sourceIdentifierTable">Table of source to destination associations already resolved.</param>
        /// <param name="destinationIdentifierTable">Table of destination to source associations already resolved.</param>
        /// <param name="key2String">Provides the string from the key.</param>
        /// <param name="string2Key">Creates a key from a string.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if the rename is valid.</returns>
        public virtual bool CheckGenericRename<TKey>(IDictionaryIndex<TKey>[] renamedItemTables, ISealableDictionary<string, string> sourceIdentifierTable, ISealableDictionary<string, string> destinationIdentifierTable, Func<TKey, string> key2String, Func<string, TKey> string2Key, IErrorList errorList)
        {
            IIdentifier RenameSourceIdentifier = (IIdentifier)SourceIdentifier;
            IIdentifier RenameDestinationIdentifier = (IIdentifier)DestinationIdentifier;

            // Validate source and destination strings.
            if (!StringValidation.IsValidIdentifier(RenameSourceIdentifier, SourceIdentifier.Text, out string ValidSourceIdentifier, out IErrorStringValidity StringError))
            {
                errorList.AddError(StringError);
                return false;
            }

            if (!StringValidation.IsValidIdentifier(RenameDestinationIdentifier, DestinationIdentifier.Text, out string ValidDestinationIdentifier, out StringError))
            {
                errorList.AddError(StringError);
                return false;
            }

            // Reject identity.
            if (ValidSourceIdentifier == ValidDestinationIdentifier)
            {
                errorList.AddError(new ErrorNameUnchanged(this, ValidSourceIdentifier));
                return false;
            }

            // Reject duplicates.
            if (sourceIdentifierTable.ContainsKey(ValidSourceIdentifier))
            {
                errorList.AddError(new ErrorIdentifierAlreadyListed(RenameSourceIdentifier, ValidSourceIdentifier));
                return false;
            }

            if (destinationIdentifierTable.ContainsKey(ValidSourceIdentifier))
            {
                errorList.AddError(new ErrorDoubleRename(RenameSourceIdentifier, destinationIdentifierTable[ValidSourceIdentifier], ValidSourceIdentifier));
                return false;
            }

            // Check that the source name exists.
            OnceReference<IDictionaryIndex<TKey>> SourceTable = new OnceReference<IDictionaryIndex<TKey>>();
            TKey SourceKey = default;

            foreach (IDictionaryIndex<TKey> Table in renamedItemTables)
            {
                foreach (TKey Entry in Table.Indexes)
                {
                    string ValidName = key2String(Entry);
                    if (ValidName == ValidSourceIdentifier)
                    {
                        SourceTable.Item = Table;
                        SourceKey = Entry;
                        break;
                    }
                }
                if (SourceTable.IsAssigned)
                    break;
            }

            if (!SourceTable.IsAssigned)
            {
                errorList.AddError(new ErrorUnknownIdentifier(RenameSourceIdentifier, ValidSourceIdentifier));
                return false;
            }

            // Check that no other rename uses this destination.
            if (destinationIdentifierTable.ContainsKey(ValidDestinationIdentifier))
            {
                errorList.AddError(new ErrorIdentifierAlreadyListed(RenameDestinationIdentifier, ValidDestinationIdentifier));
                return false;
            }

            Debug.Assert(!sourceIdentifierTable.ContainsKey(ValidSourceIdentifier));
            Debug.Assert(!destinationIdentifierTable.ContainsKey(ValidDestinationIdentifier));
            sourceIdentifierTable.Add(ValidSourceIdentifier, ValidDestinationIdentifier);
            destinationIdentifierTable.Add(ValidDestinationIdentifier, ValidSourceIdentifier);

            SourceTable.Item.ChangeKey(SourceKey, string2Key(ValidDestinationIdentifier));
            return true;
        }
        #endregion
    }
}
