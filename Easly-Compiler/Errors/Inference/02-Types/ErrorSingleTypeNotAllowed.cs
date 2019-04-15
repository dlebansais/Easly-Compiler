namespace EaslyCompiler
{
    /// <summary>
    /// Invalid declaration of a singleton.
    /// </summary>
    public interface IErrorSingleTypeNotAllowed : IError
    {
        /// <summary>
        /// The entity name.
        /// </summary>
        string EntityName { get; }
    }

    /// <summary>
    /// Invalid declaration of a singleton.
    /// </summary>
    internal class ErrorSingleTypeNotAllowed : Error, IErrorSingleTypeNotAllowed
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorSingleTypeNotAllowed"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="entityName">The entity name</param>
        public ErrorSingleTypeNotAllowed(ISource source, string entityName)
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
        public override string Message { get { return $"Entity '{EntityName}' cannot be declared with a default value, it already has one."; } }
        #endregion
    }
}
