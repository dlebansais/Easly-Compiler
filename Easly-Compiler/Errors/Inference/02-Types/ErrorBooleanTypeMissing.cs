namespace EaslyCompiler
{
    /// <summary>
    /// The type 'Boolean' is not imported.
    /// </summary>
    public interface IErrorBooleanTypeMissing : IError
    {
    }

    /// <summary>
    /// The type 'Boolean' is not imported.
    /// </summary>
    internal class ErrorBooleanTypeMissing : Error, IErrorBooleanTypeMissing
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorBooleanTypeMissing"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorBooleanTypeMissing(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type 'Boolean' not imported."; } }
        #endregion
    }
}
