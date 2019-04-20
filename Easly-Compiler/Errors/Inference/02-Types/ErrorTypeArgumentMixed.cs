namespace EaslyCompiler
{
    /// <summary>
    /// Positional and assignment type arguments mixed.
    /// </summary>
    public interface IErrorTypeArgumentMixed : IError
    {
    }

    /// <summary>
    /// Positional and assignment type arguments mixed.
    /// </summary>
    internal class ErrorTypeArgumentMixed : Error, IErrorTypeArgumentMixed
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorTypeArgumentMixed"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorTypeArgumentMixed(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Mixing assignment and positional type arguments not allowed."; } }
        #endregion
    }
}
