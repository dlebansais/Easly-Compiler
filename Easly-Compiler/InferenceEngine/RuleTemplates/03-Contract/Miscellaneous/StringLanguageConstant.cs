namespace EaslyCompiler
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a string constant.
    /// </summary>
    public interface IStringLanguageConstant : ILanguageConstant
    {
        /// <summary>
        /// The constant value, if known.
        /// </summary>
        string Value { get; }
    }

    /// <summary>
    /// Represents a string constant.
    /// </summary>
    public class StringLanguageConstant : LanguageConstant, IStringLanguageConstant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringLanguageConstant"/> class.
        /// </summary>
        public StringLanguageConstant()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLanguageConstant"/> class.
        /// </summary>
        /// <param name="value">The constant value.</param>
        public StringLanguageConstant(string value)
        {
            Value = value;
        }

        /// <summary>
        /// The constant value, if known.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsCompatibleWith(ILanguageConstant other)
        {
            return other is IStringLanguageConstant AsStringLanguageConstant && Value != null && AsStringLanguageConstant.Value != null;
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsConstantEqual(ILanguageConstant other)
        {
            return IsConstantEqual(other as IStringLanguageConstant);
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        protected virtual bool IsConstantEqual(IStringLanguageConstant other)
        {
            Debug.Assert(other != null && Value != null && other.Value != null);

            return Value == other.Value;
        }
    }
}
