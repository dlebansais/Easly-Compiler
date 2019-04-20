namespace EaslyCompiler
{
    /// <summary>
    /// Can't use Result.
    /// </summary>
    public interface IErrorUnavailableResult : IError
    {
    }

    /// <summary>
    /// Can't use Result.
    /// </summary>
    internal class ErrorUnavailableResult : Error, IErrorUnavailableResult
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorUnavailableResult"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorUnavailableResult(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "The keyword 'result' cannot be used in this context."; } }
        #endregion
    }
}
