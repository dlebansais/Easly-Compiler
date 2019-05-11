namespace EaslyCompiler
{
    /// <summary>
    /// An invalid expression.
    /// </summary>
    public interface IErrorInvalidExpression : IError
    {
    }

    /// <summary>
    /// An invalid expression.
    /// </summary>
    internal class ErrorInvalidExpression : Error, IErrorInvalidExpression
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidExpression"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorInvalidExpression(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Invalid Expression."; } }
        #endregion
    }
}
