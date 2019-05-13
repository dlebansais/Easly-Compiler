namespace EaslyCompiler
{
    /// <summary>
    /// One or more results used in an expression don't match a counterpart.
    /// </summary>
    public interface IErrorExpressionResultMismatch : IError
    {
    }

    /// <summary>
    /// One or more results used in an expression don't match a counterpart.
    /// </summary>
    internal class ErrorExpressionResultMismatch : Error, IErrorExpressionResultMismatch
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorExpressionResultMismatch"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorExpressionResultMismatch(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Results of expressions don't match."; } }
        #endregion
    }
}
