namespace EaslyCompiler
{
    /// <summary>
    /// Invalid range.
    /// </summary>
    public interface IErrorInvalidRange : IError
    {
    }

    /// <summary>
    /// Invalid range.
    /// </summary>
    internal class ErrorInvalidRange : Error, IErrorInvalidRange
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidRange"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorInvalidRange(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Invalid range."; } }
        #endregion
    }
}
