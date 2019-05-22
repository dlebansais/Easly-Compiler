namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// Exceptions that a node can throw.
    /// </summary>
    public interface IResultException : IReadOnlyCollection<IIdentifier>
    {
        /// <summary>
        /// Gets the identifier at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        IIdentifier At(int index);
    }

    /// <summary>
    /// Exceptions that a node can throw.
    /// </summary>
    public class ResultException : List<IIdentifier>, IResultException
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultException"/> class.
        /// </summary>
        public ResultException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultException"/> class.
        /// </summary>
        /// <param name="exceptionList">The list of exceptions.</param>
        public ResultException(IList<IIdentifier> exceptionList)
        {
            AddRange(exceptionList);
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the identifier at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        public IIdentifier At(int index)
        {
            return this[index];
        }

        /// <summary>
        /// Propagates a list of exceptions if they are available.
        /// </summary>
        /// <param name="other">The optionally assigned list of exception.</param>
        /// <param name="result">The list of exceptions if available; Otherwise, null.</param>
        public static void Propagate(OnceReference<IResultException> other, out IResultException result)
        {
            if (other.IsAssigned)
                result = other.Item;
            else
                result = null;
        }

        /// <summary>
        /// Merges two lists of exceptions if they are available.
        /// </summary>
        /// <param name="other1">The first optionally assigned list of exception.</param>
        /// <param name="other2">The second optionally assigned list of exception.</param>
        /// <param name="result">The list of exceptions if available; Otherwise, null.</param>
        public static void Merge(OnceReference<IResultException> other1, OnceReference<IResultException> other2, out IResultException result)
        {
            if (other1.IsAssigned && other2.IsAssigned)
            {
                IList<IIdentifier> MergedList = new List<IIdentifier>();
                MergeIdentifierList(MergedList, other1.Item);
                MergeIdentifierList(MergedList, other2.Item);

                result = new ResultException(MergedList);
            }
            else
                result = null;
        }

        private static void MergeIdentifierList(IList<IIdentifier> mergedList, IResultException otherList)
        {
            foreach (IIdentifier OtherItem in otherList)
            {
                string OtherText = OtherItem.ValidText.Item;

                bool IsFound = false;
                foreach (IIdentifier Item in mergedList)
                {
                    string Text = Item.ValidText.Item;
                    IsFound |= Text == OtherText;
                }

                if (!IsFound)
                    mergedList.Add(OtherItem);
            }
        }
        #endregion
    }
}
