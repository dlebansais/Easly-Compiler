namespace EaslyCompiler
{
    /// <summary>
    /// Multiple declaration of a singleton.
    /// </summary>
    public interface IErrorSingleInstanceConflict : IError
    {
        /// <summary>
        /// The class name.
        /// </summary>
        string ClassName { get; }
    }

    /// <summary>
    /// Multiple declaration of a singleton.
    /// </summary>
    internal class ErrorSingleInstanceConflict : Error, IErrorSingleInstanceConflict
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorSingleInstanceConflict"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="className">The class name.</param>
        public ErrorSingleInstanceConflict(ISource source, string className)
            : base(source)
        {
            ClassName = className;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The class name.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Class '{ClassName}' used but another instance of a related class already exists."; } }
        #endregion
    }
}
