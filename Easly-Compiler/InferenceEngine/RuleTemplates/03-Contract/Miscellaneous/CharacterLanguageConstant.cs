namespace EaslyCompiler
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a character constant.
    /// </summary>
    public interface ICharacterLanguageConstant : ILanguageConstant
    {
        /// <summary>
        /// The constant value, if known.
        /// </summary>
        char? Value { get; }
    }

    /// <summary>
    /// Represents a character constant.
    /// </summary>
    public class CharacterLanguageConstant : LanguageConstant, ICharacterLanguageConstant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterLanguageConstant"/> class.
        /// </summary>
        public CharacterLanguageConstant()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterLanguageConstant"/> class.
        /// </summary>
        /// <param name="value">The constant value.</param>
        public CharacterLanguageConstant(char value)
        {
            Value = value;
        }

        /// <summary>
        /// True if the constant value is known.
        /// </summary>
        public override bool IsValueKnown { get { return Value.HasValue; } }

        /// <summary>
        /// The constant value, if known.
        /// </summary>
        public char? Value { get; }

        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsCompatibleWith(ILanguageConstant other)
        {
            return other is ICharacterLanguageConstant AsCharacterLanguageConstant && IsValueKnown && AsCharacterLanguageConstant.IsValueKnown;
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsConstantEqual(ILanguageConstant other)
        {
            return IsConstantEqual(other as ICharacterLanguageConstant);
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        protected virtual bool IsConstantEqual(ICharacterLanguageConstant other)
        {
            Debug.Assert(other != null && Value.HasValue && other.Value.HasValue);

            return Value.Value == other.Value.Value;
        }
    }
}
