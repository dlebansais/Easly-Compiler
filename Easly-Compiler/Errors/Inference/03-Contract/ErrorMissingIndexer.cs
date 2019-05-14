namespace EaslyCompiler
{
    /// <summary>
    /// No indexer.
    /// </summary>
    public interface IErrorMissingIndexer : IError
    {
    }

    /// <summary>
    /// No indexer.
    /// </summary>
    internal class ErrorMissingIndexer : Error, IErrorMissingIndexer
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMissingIndexer"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorMissingIndexer(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Class has no indexer."; } }
        #endregion
    }
}
