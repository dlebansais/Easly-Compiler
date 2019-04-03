namespace EaslyCompiler
{
    using System;
    using System.Diagnostics;
    using System.Text;

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
        public ErrorInvalidCharacter(ISource source, char character)
            : base(source)
        {
            Character = character;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The invalid character.
        /// </summary>
        public char Character { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message
        {
            get
            {
                byte[] Bytes = Encoding.UTF32.GetBytes(new char[] { Character });
                Debug.Assert(Bytes.Length == sizeof(int));
                int Code = BitConverter.ToInt32(Bytes, 0);
                Debug.Assert(Code >= 0);

                return $"Invalid character with code U+{Code.ToString("X4")} found.";
            }
        }
        #endregion
    }
}
