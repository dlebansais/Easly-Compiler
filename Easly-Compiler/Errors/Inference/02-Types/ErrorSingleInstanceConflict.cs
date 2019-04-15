namespace EaslyCompiler
{
    /// <summary>
    /// Multiple declaration of a singleton.
    /// </summary>
    public interface IErrorSingleInstanceConflict : IError
    {
        /// <summary>
        /// The entity name.
        /// </summary>
        string EntityName { get; }
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
        /// <param name="entityName">The entity name</param>
        public ErrorSingleInstanceConflict(ISource source, string entityName)
            : base(source)
        {
            EntityName = entityName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The entity name.
        /// </summary>
        public string EntityName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Entity '{EntityName}' declared but another instance of a related class already exists."; } }
        #endregion
    }
}
