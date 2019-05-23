namespace EaslyCompiler
{
    /// <summary>
    /// Invalid attachment.
    /// </summary>
    public interface IErrorInvalidAttachment : IError
    {
    }

    /// <summary>
    /// Invalid attachment.
    /// </summary>
    internal class ErrorInvalidAttachment : Error, IErrorInvalidAttachment
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidAttachment"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorInvalidAttachment(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Invalid attachment."; } }
        #endregion
    }
}
