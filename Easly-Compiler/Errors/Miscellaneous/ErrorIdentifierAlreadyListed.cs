namespace EaslyCompiler
{
    /// <summary>
    /// Identifier already listed.
    /// </summary>
    public interface IErrorIdentifierAlreadyListed : IError
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        string Identifier { get; }
    }

    /// <summary>
    /// Identifier already listed.
    /// </summary>
    internal class ErrorIdentifierAlreadyListed : Error, IErrorIdentifierAlreadyListed
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorIdentifierAlreadyListed"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="identifier">The identifier.</param>
        public ErrorIdentifierAlreadyListed(ISource source, string identifier)
            : base(source)
        {
            Identifier = identifier;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The identifier.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Identifier '{Identifier}' already listed."; } }
        #endregion
    }
}
