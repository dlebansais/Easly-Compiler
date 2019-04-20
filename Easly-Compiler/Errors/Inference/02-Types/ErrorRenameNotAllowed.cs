namespace EaslyCompiler
{
    /// <summary>
    /// Rename not allowed.
    /// </summary>
    public interface IErrorRenameNotAllowed : IError
    {
    }

    /// <summary>
    /// Rename not allowed.
    /// </summary>
    internal class ErrorRenameNotAllowed : Error, IErrorRenameNotAllowed
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorRenameNotAllowed"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorRenameNotAllowed(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Renaming not allowed in this context."; } }
        #endregion
    }
}
