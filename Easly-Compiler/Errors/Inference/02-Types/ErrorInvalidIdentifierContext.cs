namespace EaslyCompiler
{
    /// <summary>
    /// Use of a reserved name.
    /// </summary>
    public interface IErrorInvalidIdentifierContext : IError
    {
        /// <summary>
        /// The invalid identifier.
        /// </summary>
        string Identifier { get; }
    }

    /// <summary>
    /// Use of a reserved name.
    /// </summary>
    internal class ErrorInvalidIdentifierContext : Error, IErrorInvalidIdentifierContext
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidIdentifierContext"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="identifier">The invalid identifier.</param>
        public ErrorInvalidIdentifierContext(ISource source, string identifier)
            : base(source)
        {
            Identifier = identifier;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The invalid identifier.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Identifier '{Identifier}' is invalid in this context."; } }
        #endregion
    }
}
