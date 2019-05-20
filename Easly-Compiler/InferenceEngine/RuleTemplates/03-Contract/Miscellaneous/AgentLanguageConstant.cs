namespace EaslyCompiler
{
    using System.Diagnostics;

    /// <summary>
    /// Represents an agent feature as some constant.
    /// </summary>
    public interface IAgentLanguageConstant : ILanguageConstant
    {
        /// <summary>
        /// The constant value, if known.
        /// </summary>
        ICompiledFeature Value { get; }
    }

    /// <summary>
    /// Represents an agent feature as some constant.
    /// </summary>
    public class AgentLanguageConstant : LanguageConstant, IAgentLanguageConstant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentLanguageConstant"/> class.
        /// </summary>
        /// <param name="value">The constant value.</param>
        public AgentLanguageConstant(ICompiledFeature value)
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
        public ICompiledFeature Value { get; }

        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsCompatibleWith(ILanguageConstant other)
        {
            return other is IAgentLanguageConstant AsAgentLanguageConstant && IsValueKnown && AsAgentLanguageConstant.IsValueKnown;
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsConstantEqual(ILanguageConstant other)
        {
            return IsConstantEqual(other as IAgentLanguageConstant);
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        protected virtual bool IsConstantEqual(IAgentLanguageConstant other)
        {
            Debug.Assert(other != null && Value != null && other.Value != null);

            return Value == other.Value;
        }
    }
}
