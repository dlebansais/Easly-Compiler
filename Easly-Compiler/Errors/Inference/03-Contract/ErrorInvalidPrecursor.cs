namespace EaslyCompiler
{
    /// <summary>
    /// Invalid precursor.
    /// </summary>
    public interface IErrorInvalidPrecursor : IError
    {
    }

    /// <summary>
    /// Invalid precursor.
    /// </summary>
    internal class ErrorInvalidPrecursor : Error, IErrorInvalidPrecursor
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidPrecursor"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorInvalidPrecursor(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Invalid precursor."; } }
        #endregion
    }
}
