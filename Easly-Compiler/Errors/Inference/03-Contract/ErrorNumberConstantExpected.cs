namespace EaslyCompiler
{
    /// <summary>
    /// Expression not a constant number.
    /// </summary>
    public interface IErrorNumberConstantExpected : IError
    {
    }

    /// <summary>
    /// Expression not a constant number.
    /// </summary>
    internal class ErrorNumberConstantExpected : Error, IErrorNumberConstantExpected
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorNumberConstantExpected"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorNumberConstantExpected(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Constant number expected."; } }
        #endregion
    }
}
