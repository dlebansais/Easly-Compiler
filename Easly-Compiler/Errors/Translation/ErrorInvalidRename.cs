namespace EaslyCompiler
{
    /// <summary>
    /// Invalid rename in C#.
    /// </summary>
    public interface IErrorInvalidRename : IError
    {
    }

    /// <summary>
    /// Invalid rename in C#.
    /// </summary>
    internal class ErrorInvalidRename : Error, IErrorInvalidRename
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidRename"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorInvalidRename(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Renaming features is not allowed in this context."; } }
        #endregion
    }
}
