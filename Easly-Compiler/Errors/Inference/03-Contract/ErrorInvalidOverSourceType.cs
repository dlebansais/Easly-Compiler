namespace EaslyCompiler
{
    /// <summary>
    /// Invalid source for an over loop.
    /// </summary>
    public interface IErrorInvalidOverSourceType : IError
    {
    }

    /// <summary>
    /// Invalid source for an over loop.
    /// </summary>
    internal class ErrorInvalidOverSourceType : Error, IErrorInvalidOverSourceType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidOverSourceType"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorInvalidOverSourceType(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type contains an indexer but also inherits from 'Over Loop Source'."; } }
        #endregion
    }
}
