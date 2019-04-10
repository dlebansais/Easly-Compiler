namespace EaslyCompiler
{
    /// <summary>
    /// Invalid character.
    /// </summary>
    public interface IErrorInvalidManifestChraracter : IError
    {
        /// <summary>
        /// The Invalid character.
        /// </summary>
        string Character { get; }
    }

    /// <summary>
    /// Invalid character.
    /// </summary>
    internal class ErrorInvalidManifestChraracter : Error, IErrorInvalidManifestChraracter
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidManifestChraracter"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="character">The Invalid character.</param>
        public ErrorInvalidManifestChraracter(ISource source, string character)
            : base(source)
        {
            Character = character;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Invalid character.
        /// </summary>
        public string Character { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Invalid Character '{Character}'."; } }
        #endregion
    }
}
