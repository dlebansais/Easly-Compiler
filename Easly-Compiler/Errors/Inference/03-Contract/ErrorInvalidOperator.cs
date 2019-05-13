namespace EaslyCompiler
{
    /// <summary>
    /// Bad operator.
    /// </summary>
    public interface IErrorInvalidOperator : IError
    {
        /// <summary>
        /// The operator name.
        /// </summary>
        string OperatorName { get; }
    }

    /// <summary>
    /// Bad operator.
    /// </summary>
    internal class ErrorInvalidOperator : Error, IErrorInvalidOperator
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidOperator"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="operatorName">The operator name.</param>
        public ErrorInvalidOperator(ISource source, string operatorName)
            : base(source)
        {
            OperatorName = operatorName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The operator name.
        /// </summary>
        public string OperatorName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Invalid operator '{OperatorName}'."; } }
        #endregion
    }
}
