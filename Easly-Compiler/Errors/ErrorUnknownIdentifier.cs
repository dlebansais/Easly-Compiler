namespace EaslyCompiler
{
    /// <summary>
    /// Unknown identifier.
    /// </summary>
    public class ErrorUnknownIdentifier : Error
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorUnknownIdentifier"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="identifier">The unknown identifier.</param>
        public ErrorUnknownIdentifier(ISource source, string identifier)
            : base(source)
        {
            Identifier = identifier;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The unknown identifier.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Unknown identifier: '{Identifier}'."; } }
        #endregion
    }
}
