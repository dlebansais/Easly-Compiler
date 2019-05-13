namespace EaslyCompiler
{
    /// <summary>
    /// The type 'Event' is not imported.
    /// </summary>
    public interface IErrorEventTypeMissing : IError
    {
    }

    /// <summary>
    /// The type 'Event' is not imported.
    /// </summary>
    internal class ErrorEventTypeMissing : Error, IErrorEventTypeMissing
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEventTypeMissing"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorEventTypeMissing(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type 'Event' not imported."; } }
        #endregion
    }
}
