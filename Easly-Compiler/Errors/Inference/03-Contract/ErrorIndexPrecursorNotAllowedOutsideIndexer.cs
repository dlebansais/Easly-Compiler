namespace EaslyCompiler
{
    /// <summary>
    /// Invalid precursor call.
    /// </summary>
    public interface IErrorIndexPrecursorNotAllowedOutsideIndexer : IError
    {
    }

    /// <summary>
    /// Invalid precursor call.
    /// </summary>
    internal class ErrorIndexPrecursorNotAllowedOutsideIndexer : Error, IErrorIndexPrecursorNotAllowedOutsideIndexer
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorIndexPrecursorNotAllowedOutsideIndexer"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorIndexPrecursorNotAllowedOutsideIndexer(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Index precursor not allowed outside indexer."; } }
        #endregion
    }
}
