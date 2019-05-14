namespace EaslyCompiler
{
    /// <summary>
    /// The type 'Character' is not imported.
    /// </summary>
    public interface IErrorCharacterTypeMissing : IError
    {
    }

    /// <summary>
    /// The type 'Character' is not imported.
    /// </summary>
    internal class ErrorCharacterTypeMissing : Error, IErrorCharacterTypeMissing
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCharacterTypeMissing"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorCharacterTypeMissing(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type 'Character' not imported."; } }
        #endregion
    }
}
