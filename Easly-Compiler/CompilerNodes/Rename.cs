namespace CompilerNode
{
    using System;
    using System.Collections.Generic;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IRename.
    /// </summary>
    public interface IRename : BaseNode.IRename, INode, ISource
    {
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
        bool CheckGenericRename<TKey>(IHashtableIndex<TKey>[] renamedItemTables, IHashtableEx<string, string> sourceIdentifierTable, IHashtableEx<string, string> destinationIdentifierTable, Func<TKey, string> key2String, Func<string, TKey> string2Key, IList<IError> errorList);
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

        #region Compiler
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
        public virtual bool CheckGenericRename<TKey>(IHashtableIndex<TKey>[] renamedItemTables, IHashtableEx<string, string> sourceIdentifierTable, IHashtableEx<string, string> destinationIdentifierTable, Func<TKey, string> key2String, Func<string, TKey> string2Key, IList<IError> errorList)
        {
            IIdentifier RenameSourceIdentifier = (IIdentifier)SourceIdentifier;
            IIdentifier RenameDestinationIdentifier = (IIdentifier)DestinationIdentifier;

            // Validate source and destination strings.
            if (!StringValidation.IsValidIdentifier(RenameSourceIdentifier, SourceIdentifier.Text, out string ValidSourceIdentifier, out IErrorStringValidity StringError))
            {
                errorList.Add(StringError);
                return false;
            }

            if (!StringValidation.IsValidIdentifier(RenameDestinationIdentifier, DestinationIdentifier.Text, out string ValidDestinationIdentifier, out StringError))
            {
                errorList.Add(StringError);
                return false;
            }

            // Reject identity.
            if (ValidSourceIdentifier == ValidDestinationIdentifier)
            {
                errorList.Add(new ErrorNameUnchanged(this, ValidSourceIdentifier));
                return false;
            }

            // Reject duplicates.
            if (sourceIdentifierTable.ContainsKey(ValidSourceIdentifier))
            {
                errorList.Add(new ErrorIdentifierAlreadyListed(RenameSourceIdentifier, ValidSourceIdentifier));
                return false;
            }

            if (destinationIdentifierTable.ContainsKey(ValidSourceIdentifier))
            {
                errorList.Add(new ErrorDoubleRename(RenameSourceIdentifier, destinationIdentifierTable[ValidSourceIdentifier], ValidSourceIdentifier));
                return false;
            }

            // Check that the source name exists.
            OnceReference<IHashtableIndex<TKey>> SourceTable = new OnceReference<IHashtableIndex<TKey>>();
            TKey SourceKey = default;

            foreach (IHashtableIndex<TKey> Table in renamedItemTables)
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
                errorList.Add(new ErrorUnknownIdentifier(RenameSourceIdentifier, ValidSourceIdentifier));
                return false;
            }

            // Check that no other rename uses this destination.
            foreach (IHashtableIndex<TKey> Table in renamedItemTables)
                foreach (TKey Entry in Table.Indexes)
                {
                    string ValidName = key2String(Entry);
                    if (ValidName == ValidDestinationIdentifier)
                    {
                        errorList.Add(new ErrorIdentifierAlreadyListed(RenameDestinationIdentifier, ValidDestinationIdentifier));
                        return false;
                    }
                }

            sourceIdentifierTable.Add(ValidSourceIdentifier, ValidDestinationIdentifier);
            destinationIdentifierTable.Add(ValidDestinationIdentifier, ValidSourceIdentifier);

            SourceTable.Item.ChangeKey(SourceKey, string2Key(ValidDestinationIdentifier));
            return true;
        }
        #endregion
    }
}
