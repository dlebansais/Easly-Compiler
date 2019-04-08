namespace EaslyCompiler
{
    /// <summary>
    /// Whitespace not allowed in string.
    /// </summary>
    public interface IErrorWhiteSpaceNotAllowed : IErrorStringValidity
    {
    }

    /// <summary>
    /// Whitespace not allowed in string.
    /// </summary>
    internal class ErrorWhiteSpaceNotAllowed : ErrorStringValidity, IErrorWhiteSpaceNotAllowed
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorWhiteSpaceNotAllowed"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorWhiteSpaceNotAllowed(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "White space not allowed at the begining or the end of an identifier or name."; } }
        #endregion
    }
}
