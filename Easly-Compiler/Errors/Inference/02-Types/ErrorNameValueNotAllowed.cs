namespace EaslyCompiler
{
    /// <summary>
    /// The name 'Value' is not allowed.
    /// </summary>
    public interface IErrorNameValueNotAllowed : IError
    {
    }

    /// <summary>
    /// The name 'Value' is not allowed.
    /// </summary>
    internal class ErrorNameValueNotAllowed : Error, IErrorNameValueNotAllowed
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorNameValueNotAllowed"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorNameValueNotAllowed(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "The name Value is reserved to another use."; } }
        #endregion
    }
}
