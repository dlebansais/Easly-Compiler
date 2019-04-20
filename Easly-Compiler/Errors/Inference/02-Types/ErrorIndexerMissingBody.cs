namespace EaslyCompiler
{
    /// <summary>
    /// Indexer must have a body.
    /// </summary>
    public interface IErrorIndexerMissingBody : IError
    {
    }

    /// <summary>
    /// Indexer must have a body.
    /// </summary>
    internal class ErrorIndexerMissingBody : Error, IErrorIndexerMissingBody
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorIndexerMissingBody"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorIndexerMissingBody(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Indexer must have a body."; } }
        #endregion
    }
}
