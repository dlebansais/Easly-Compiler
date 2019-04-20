namespace EaslyCompiler
{
    /// <summary>
    /// Two bodies of different type in the same indexer feature.
    /// </summary>
    public interface IErrorIndexerBodyTypeMismatch : IError
    {
    }

    /// <summary>
    /// Two bodies of different type in the same indexer feature.
    /// </summary>
    internal class ErrorIndexerBodyTypeMismatch : Error, IErrorIndexerBodyTypeMismatch
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorIndexerBodyTypeMismatch"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorIndexerBodyTypeMismatch(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Indexer has mismatching bodies."; } }
        #endregion
    }
}
