namespace EaslyCompiler
{
    /// <summary>
    /// Invalid arguments.
    /// </summary>
    public interface IErrorArgumentMixed : IError
    {
    }

    /// <summary>
    /// Invalid arguments.
    /// </summary>
    internal class ErrorArgumentMixed : Error, IErrorArgumentMixed
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorArgumentMixed"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorArgumentMixed(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Mixing assignment and positional arguments not allowed."; } }
        #endregion
    }
}
