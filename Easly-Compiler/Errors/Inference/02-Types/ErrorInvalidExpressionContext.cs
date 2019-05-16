namespace EaslyCompiler
{
    /// <summary>
    /// Expression used outside a valid context.
    /// </summary>
    public interface IErrorInvalidExpressionContext : IError
    {
    }

    /// <summary>
    /// Expression used outside a valid context.
    /// </summary>
    internal class ErrorInvalidExpressionContext : Error, IErrorInvalidExpressionContext
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidExpressionContext"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorInvalidExpressionContext(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Expression not allowed in this context."; } }
        #endregion
    }
}
