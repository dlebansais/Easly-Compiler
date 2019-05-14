namespace EaslyCompiler
{
    /// <summary>
    /// Invalid arguments.
    /// </summary>
    public interface IErrorConstantQueryExpression : IError
    {
    }

    /// <summary>
    /// Invalid arguments.
    /// </summary>
    internal class ErrorConstantQueryExpression : Error, IErrorConstantQueryExpression
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorConstantQueryExpression"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorConstantQueryExpression(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Query Expression cannot apply to a constant."; } }
        #endregion
    }
}
