namespace EaslyCompiler
{
    /// <summary>
    /// Invalid argument.
    /// </summary>
    public interface IErrorConstantExpected : IError
    {
    }

    /// <summary>
    /// Invalid argument.
    /// </summary>
    internal class ErrorConstantExpected : Error, IErrorConstantExpected
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorConstantExpected"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorConstantExpected(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "A constant is expected."; } }
        #endregion
    }
}
