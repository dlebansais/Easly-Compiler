namespace EaslyCompiler
{
    /// <summary>
    /// The type 'Entity' is not imported.
    /// </summary>
    public interface IErrorEntityTypeMissing : IError
    {
    }

    /// <summary>
    /// The type 'Entity' is not imported.
    /// </summary>
    internal class ErrorEntityTypeMissing : Error, IErrorEntityTypeMissing
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEntityTypeMissing"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorEntityTypeMissing(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type 'Entity' not imported."; } }
        #endregion
    }
}
