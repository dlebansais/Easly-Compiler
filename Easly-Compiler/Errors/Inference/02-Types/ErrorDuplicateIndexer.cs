namespace EaslyCompiler
{
    /// <summary>
    /// More than one indexer.
    /// </summary>
    public interface IErrorDuplicateIndexer : IError
    {
    }

    /// <summary>
    /// More than one indexer.
    /// </summary>
    internal class ErrorDuplicateIndexer : Error, IErrorDuplicateIndexer
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDuplicateIndexer"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorDuplicateIndexer(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "A class can only have one indexer."; } }
        #endregion
    }
}
