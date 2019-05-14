namespace EaslyCompiler
{
    /// <summary>
    /// Invalid 'new' expression.
    /// </summary>
    public interface IErrorConstantNewExpression : IError
    {
    }

    /// <summary>
    /// Invalid 'new' expression.
    /// </summary>
    internal class ErrorConstantNewExpression : Error, IErrorConstantNewExpression
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorConstantNewExpression"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorConstantNewExpression(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "New Expression cannot apply to a constant."; } }
        #endregion
    }
}
