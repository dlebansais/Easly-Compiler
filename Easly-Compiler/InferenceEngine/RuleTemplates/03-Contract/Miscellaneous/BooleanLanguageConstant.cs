namespace EaslyCompiler
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a boolean constant.
    /// </summary>
    public interface IBooleanLanguageConstant : ILanguageConstant
    {
        /// <summary>
        /// The constant value, if known.
        /// </summary>
        bool? Value { get; }
    }

    /// <summary>
    /// Represents a boolean constant.
    /// </summary>
    public class BooleanLanguageConstant : LanguageConstant, IBooleanLanguageConstant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanLanguageConstant"/> class.
        /// </summary>
        public BooleanLanguageConstant()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanLanguageConstant"/> class.
        /// </summary>
        /// <param name="value">The constant value.</param>
        public BooleanLanguageConstant(bool value)
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
        public bool? Value { get; }

        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsCompatibleWith(ILanguageConstant other)
        {
            return other is IBooleanLanguageConstant AsBooleanLanguageConstant && IsValueKnown && AsBooleanLanguageConstant.IsValueKnown;
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsConstantEqual(ILanguageConstant other)
        {
            return IsConstantEqual(other as IBooleanLanguageConstant);
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        protected virtual bool IsConstantEqual(IBooleanLanguageConstant other)
        {
            Debug.Assert(other != null && Value.HasValue && other.Value.HasValue);

            return Value.Value == other.Value.Value;
        }
    }
}
