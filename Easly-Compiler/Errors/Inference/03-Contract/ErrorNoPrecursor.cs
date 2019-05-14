namespace EaslyCompiler
{
    /// <summary>
    /// Invalid precursor call.
    /// </summary>
    public interface IErrorNoPrecursor : IError
    {
    }

    /// <summary>
    /// Invalid precursor call.
    /// </summary>
    internal class ErrorNoPrecursor : Error, IErrorNoPrecursor
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorNoPrecursor"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorNoPrecursor(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Feature has no precursor."; } }
        #endregion
    }
}
