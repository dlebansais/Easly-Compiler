namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// Represents an entity feature as some constant.
    /// </summary>
    public interface IEntityLanguageConstant : ILanguageConstant
    {
        /// <summary>
        /// The constant value, if known.
        /// </summary>
        ISource Value { get; }
    }

    /// <summary>
    /// Represents an entity feature as some constant.
    /// </summary>
    public class EntityLanguageConstant : LanguageConstant, IEntityLanguageConstant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLanguageConstant"/> class.
        /// </summary>
        /// <param name="value">The constant value.</param>
        public EntityLanguageConstant(IFeatureWithEntity value)
        {
            Value = value.Location;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLanguageConstant"/> class.
        /// </summary>
        /// <param name="value">The constant value.</param>
        public EntityLanguageConstant(IDiscrete value)
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
        public ISource Value { get; }

        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsCompatibleWith(ILanguageConstant other)
        {
            return other is IEntityLanguageConstant AsEntityLanguageConstant && IsValueKnown && AsEntityLanguageConstant.IsValueKnown;
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsConstantEqual(ILanguageConstant other)
        {
            return IsConstantEqual(other as IEntityLanguageConstant);
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        protected virtual bool IsConstantEqual(IEntityLanguageConstant other)
        {
            Debug.Assert(other != null && Value != null && other.Value != null);

            return Value == other.Value;
        }
    }
}
