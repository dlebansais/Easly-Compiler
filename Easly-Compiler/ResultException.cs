namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

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
        #endregion
    }
}
