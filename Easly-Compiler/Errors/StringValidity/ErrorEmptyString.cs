namespace EaslyCompiler
{
    /// <summary>
    /// Invalid empty string.
    /// </summary>
    public interface IErrorEmptyString : IErrorStringValidity
    {
    }

    /// <summary>
    /// Invalid empty string.
    /// </summary>
    internal class ErrorEmptyString : ErrorStringValidity, IErrorEmptyString
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEmptyString"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorEmptyString(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Empty identifier or name not allowed."; } }
        #endregion
    }
}
