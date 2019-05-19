namespace EaslyCompiler
{
    /// <summary>
    /// Invalid 'new' expression.
    /// </summary>
    public interface IErrorInvalidOldExpression : IError
    {
    }

    /// <summary>
    /// Invalid 'new' expression.
    /// </summary>
    internal class ErrorInvalidOldExpression : Error, IErrorInvalidOldExpression
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidOldExpression"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorInvalidOldExpression(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Old Expression invalid in this context."; } }
        #endregion
    }
}
