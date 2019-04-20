namespace EaslyCompiler
{
    /// <summary>
    /// Can't use Result.
    /// </summary>
    public interface IErrorResultNotReturned : IError
    {
    }

    /// <summary>
    /// Can't use Result.
    /// </summary>
    internal class ErrorResultNotReturned : Error, IErrorResultNotReturned
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResultNotReturned"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorResultNotReturned(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Result is not returned by this query."; } }
        #endregion
    }
}
