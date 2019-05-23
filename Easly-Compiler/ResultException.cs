namespace EaslyCompiler
{
    using System.Collections.Generic;
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

        /// <summary>
        /// Adds an exception identifier.
        /// </summary>
        /// <param name="text">The identifier text.</param>
        void Add(string text);
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
        /// Adds an exception identifier.
        /// </summary>
        /// <param name="text">The identifier text.</param>
        public virtual void Add(string text)
        {
            IIdentifier ExceptionIdentifier = new Identifier(text);
            Add(ExceptionIdentifier);
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
        /// <param name="mergedResult">The list of exceptions to update.</param>
        /// <param name="other">The optional list of exceptions to merge.</param>
        public static void Merge(IResultException mergedResult, OnceReference<IResultException> other)
        {
            if (other.IsAssigned)
                Merge(mergedResult, other.Item);
        }

        /// <summary>
        /// Merges two lists of exceptions if they are available.
        /// </summary>
        /// <param name="mergedResult">The list of exceptions to update.</param>
        /// <param name="otherList">The list of exceptions to merge.</param>
        public static void Merge(IResultException mergedResult, IEnumerable<IIdentifier> otherList)
        {
            MergeIdentifierList((List<IIdentifier>)mergedResult, otherList);
        }

        private static void MergeIdentifierList(List<IIdentifier> mergedList, IEnumerable<IIdentifier> otherList)
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
