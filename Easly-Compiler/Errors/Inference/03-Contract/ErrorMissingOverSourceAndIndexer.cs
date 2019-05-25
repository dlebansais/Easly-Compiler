namespace EaslyCompiler
{
    /// <summary>
    /// Invalid source for an over loop.
    /// </summary>
    public interface IErrorMissingOverSourceAndIndexer : IError
    {
    }

    /// <summary>
    /// Invalid source for an over loop.
    /// </summary>
    internal class ErrorMissingOverSourceAndIndexer : Error, IErrorMissingOverSourceAndIndexer
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMissingOverSourceAndIndexer"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorMissingOverSourceAndIndexer(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type has no indexer taking an index and does not inherit from 'Over Loop Source' either."; } }
        #endregion
    }
}
