namespace EaslyCompiler
{
    /// <summary>
    /// The name 'Result' is not allowed.
    /// </summary>
    public interface IErrorNameResultNotAllowed : IError
    {
    }

    /// <summary>
    /// The name 'Result' is not allowed.
    /// </summary>
    internal class ErrorNameResultNotAllowed : Error, IErrorNameResultNotAllowed
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorNameResultNotAllowed"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorNameResultNotAllowed(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "The name Result is reserved to another use."; } }
        #endregion
    }
}
