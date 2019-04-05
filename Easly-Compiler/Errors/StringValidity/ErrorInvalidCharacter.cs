namespace EaslyCompiler
{
    /// <summary>
    /// Invalid character in a string.
    /// </summary>
    public class ErrorInvalidCharacter : ErrorStringValidity
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidCharacter"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="character">The invalid character.</param>
        public ErrorInvalidCharacter(ISource source, int character)
            : base(source)
        {
            Character = character;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The invalid character.
        /// </summary>
        public int Character { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message
        {
            get
            {
                return $"Invalid character with code U+{Character.ToString("X8")} found.";
            }
        }
        #endregion
    }
}
