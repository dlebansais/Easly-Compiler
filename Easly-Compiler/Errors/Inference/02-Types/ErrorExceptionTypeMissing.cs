namespace EaslyCompiler
{
    /// <summary>
    /// The type 'Exception' is not imported.
    /// </summary>
    public interface IErrorExceptionTypeMissing : IError
    {
    }

    /// <summary>
    /// The type 'Exception' is not imported.
    /// </summary>
    internal class ErrorExceptionTypeMissing : Error, IErrorExceptionTypeMissing
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorExceptionTypeMissing"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorExceptionTypeMissing(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type 'Exception' not imported."; } }
        #endregion
    }
}
