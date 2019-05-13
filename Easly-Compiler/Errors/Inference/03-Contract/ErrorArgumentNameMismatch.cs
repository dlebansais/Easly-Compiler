namespace EaslyCompiler
{
    /// <summary>
    /// Bad argument.
    /// </summary>
    public interface IErrorArgumentNameMismatch : IError
    {
        /// <summary>
        /// The argument name.
        /// </summary>
        string ArgumentName { get; }
    }

    /// <summary>
    /// Bad argument.
    /// </summary>
    internal class ErrorArgumentNameMismatch : Error, IErrorArgumentNameMismatch
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorArgumentNameMismatch"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="argumentName">The argument name.</param>
        public ErrorArgumentNameMismatch(ISource source, string argumentName)
            : base(source)
        {
            ArgumentName = argumentName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The argument name.
        /// </summary>
        public string ArgumentName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Argument with name '{ArgumentName}' does not match any parameter."; } }
        #endregion
    }
}
