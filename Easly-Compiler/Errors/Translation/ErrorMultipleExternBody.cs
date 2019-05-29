namespace EaslyCompiler
{
    /// <summary>
    /// Class can't be inherited.
    /// </summary>
    public interface IErrorMultipleExternBody : IError
    {
    }

    /// <summary>
    /// Class can't be inherited.
    /// </summary>
    internal class ErrorMultipleExternBody : Error, IErrorMultipleExternBody
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMultipleExternBody"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorMultipleExternBody(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Class has more than one parent with extern bodies and cannot be translated."; } }
        #endregion
    }
}
