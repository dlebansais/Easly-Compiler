namespace EaslyCompiler
{
    /// <summary>
    /// Can't use Result.
    /// </summary>
    public interface IErrorResultUsedOutsideGetter : IError
    {
    }

    /// <summary>
    /// Can't use Result.
    /// </summary>
    internal class ErrorResultUsedOutsideGetter : Error, IErrorResultUsedOutsideGetter
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResultUsedOutsideGetter"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorResultUsedOutsideGetter(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Result only allowed within getter."; } }
        #endregion
    }
}
