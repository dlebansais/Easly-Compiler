namespace EaslyCompiler
{
    using System.Diagnostics;
    using BaseNodeHelper;

    /// <summary>
    /// Represents a number constant.
    /// </summary>
    public interface INumberLanguageConstant : IOrderedLanguageConstant
    {
        /// <summary>
        /// The constant value, if known.
        /// </summary>
        ICanonicalNumber Value { get; }
    }

    /// <summary>
    /// Represents a number constant.
    /// </summary>
    public class NumberLanguageConstant : LanguageConstant, INumberLanguageConstant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumberLanguageConstant"/> class.
        /// </summary>
        public NumberLanguageConstant()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberLanguageConstant"/> class.
        /// </summary>
        /// <param name="value">The constant value.</param>
        public NumberLanguageConstant(ICanonicalNumber value)
        {
            Value = value;
        }

        /// <summary>
        /// True if the constant value is known.
        /// </summary>
        public override bool IsValueKnown { get { return Value != null; } }

        /// <summary>
        /// The constant value, if known.
        /// </summary>
        public ICanonicalNumber Value { get; }

        /// <summary>
        /// The value as an ordered integer.
        /// </summary>
        /*public int OrdinalValue
        {
            get
            {
                int Result = 0;
                bool IsParsed = Value != null && Value.TryParseInt(out Result);

                Debug.Assert(IsParsed);
                return Result;
            }
        }*/

        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsCompatibleWith(ILanguageConstant other)
        {
            return other is INumberLanguageConstant AsNumberLanguageConstant && IsValueKnown && AsNumberLanguageConstant.IsValueKnown;
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsConstantEqual(ILanguageConstant other)
        {
            return IsConstantEqual(other as INumberLanguageConstant);
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        protected virtual bool IsConstantEqual(INumberLanguageConstant other)
        {
            Debug.Assert(other != null && Value != null && other.Value != null);

            return Value.IsEqual(other.Value);
        }

        /// <summary>
        /// Checks if this instance is greater than another constant.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /*public virtual bool IsConstantGreater(IOrderedLanguageConstant other)
        {
            return IsConstantGreater(other as INumberLanguageConstant);
        }*/

        /// <summary>
        /// Checks if another constant is greater than this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /*protected virtual bool IsConstantGreater(INumberLanguageConstant other)
        {
            Debug.Assert(other != null && Value != null && other.Value != null);

            return Value.IsGreater(other.Value);
        }*/
    }
}
