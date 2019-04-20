namespace EaslyCompiler
{
    /// <summary>
    /// Can't use Value.
    /// </summary>
    public interface IErrorUnavailableValue : IError
    {
    }

    /// <summary>
    /// Can't use Value.
    /// </summary>
    internal class ErrorUnavailableValue : Error, IErrorUnavailableValue
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorUnavailableValue"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorUnavailableValue(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "The keyword 'value' cannot be used in this context."; } }
        #endregion
    }
}
