namespace EaslyCompiler
{
    /// <summary>
    /// Invalid indexer inheritance.
    /// </summary>
    public interface IErrorIndexerInheritance : IError
    {
    }

    /// <summary>
    /// Invalid indexer inheritance.
    /// </summary>
    internal class ErrorIndexerInheritance : Error, IErrorIndexerInheritance
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorIndexerInheritance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorIndexerInheritance(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Indexer inheritance has invalid settings."; } }
        #endregion
    }
}
