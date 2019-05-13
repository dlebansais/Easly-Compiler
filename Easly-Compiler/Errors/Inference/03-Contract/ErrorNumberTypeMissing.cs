namespace EaslyCompiler
{
    /// <summary>
    /// The type 'Number' is not imported.
    /// </summary>
    public interface IErrorNumberTypeMissing : IError
    {
    }

    /// <summary>
    /// The type 'Number' is not imported.
    /// </summary>
    internal class ErrorNumberTypeMissing : Error, IErrorNumberTypeMissing
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorNumberTypeMissing"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorNumberTypeMissing(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type 'Number' not imported."; } }
        #endregion
    }
}
