namespace EaslyCompiler
{
    /// <summary>
    /// The type 'String' is not imported.
    /// </summary>
    public interface IErrorStringTypeMissing : IError
    {
    }

    /// <summary>
    /// The type 'String' is not imported.
    /// </summary>
    internal class ErrorStringTypeMissing : Error, IErrorStringTypeMissing
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorStringTypeMissing"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorStringTypeMissing(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type 'String' not imported."; } }
        #endregion
    }
}
