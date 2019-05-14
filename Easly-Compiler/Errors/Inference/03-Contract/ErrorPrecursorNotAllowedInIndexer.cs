namespace EaslyCompiler
{
    /// <summary>
    /// Invalid precursor call.
    /// </summary>
    public interface IErrorPrecursorNotAllowedInIndexer : IError
    {
    }

    /// <summary>
    /// Invalid precursor call.
    /// </summary>
    internal class ErrorPrecursorNotAllowedInIndexer : Error, IErrorPrecursorNotAllowedInIndexer
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorPrecursorNotAllowedInIndexer"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorPrecursorNotAllowedInIndexer(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Precursor not allowed in indexer."; } }
        #endregion
    }
}
