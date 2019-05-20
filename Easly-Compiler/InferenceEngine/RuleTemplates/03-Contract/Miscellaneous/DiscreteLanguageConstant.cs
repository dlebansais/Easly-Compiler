namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// Represents a discrete constant.
    /// </summary>
    public interface IDiscreteLanguageConstant : IOrderedLanguageConstant
    {
        /// <summary>
        /// The discrete constant, if known.
        /// </summary>
        IDiscrete Discrete { get; }
    }

    /// <summary>
    /// Represents a discrete constant.
    /// </summary>
    public class DiscreteLanguageConstant : LanguageConstant, IDiscreteLanguageConstant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscreteLanguageConstant"/> class.
        /// </summary>
        /// <param name="discrete">The discrete constant.</param>
        public DiscreteLanguageConstant(IDiscrete discrete)
        {
            Discrete = discrete;
        }

        /// <summary>
        /// True if the constant value is known.
        /// </summary>
        public override bool IsValueKnown { get { return Discrete != null; } }

        /// <summary>
        /// The discrete constant, if known.
        /// </summary>
        public IDiscrete Discrete { get; }

        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsCompatibleWith(ILanguageConstant other)
        {
            bool Result = false;

            if (other is IDiscreteLanguageConstant AsDiscreteLanguageConstant && IsValueKnown && AsDiscreteLanguageConstant.IsValueKnown)
            {
                IDiscrete OtherDiscrete = AsDiscreteLanguageConstant.Discrete;
                Result = Discrete.EmbeddingClass == OtherDiscrete.EmbeddingClass;
            }

            return Result;
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsConstantEqual(ILanguageConstant other)
        {
            return IsConstantEqual(other as IDiscreteLanguageConstant);
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        protected virtual bool IsConstantEqual(IDiscreteLanguageConstant other)
        {
            Debug.Assert(other != null && Discrete != null && other.Discrete != null);

            IDiscrete OtherDiscrete = other.Discrete;
            IClass EmbeddingClass = Discrete.EmbeddingClass;

            Debug.Assert(EmbeddingClass != null);
            Debug.Assert(EmbeddingClass == OtherDiscrete.EmbeddingClass);

            int ThisDiscreteIndex = EmbeddingClass.DiscreteList.IndexOf(Discrete);
            int OtherDiscreteIndex = EmbeddingClass.DiscreteList.IndexOf(OtherDiscrete);

            Debug.Assert(ThisDiscreteIndex != -1 && OtherDiscreteIndex != -1);

            return ThisDiscreteIndex == OtherDiscreteIndex;
        }

        /// <summary>
        /// Checks if another constant is greater than this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /*public virtual bool IsConstantGreater(IOrderedLanguageConstant other)
        {
            return IsConstantGreater(other as IDiscreteLanguageConstant);
        }*/

        /// <summary>
        /// Checks if another constant is greater than this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /*protected virtual bool IsConstantGreater(IDiscreteLanguageConstant other)
        {
            Debug.Assert(other != null && Discrete != null && other.Discrete != null);

            IDiscrete OtherDiscrete = other.Discrete;
            IClass EmbeddingClass = Discrete.EmbeddingClass;

            Debug.Assert(EmbeddingClass != null);
            Debug.Assert(EmbeddingClass == OtherDiscrete.EmbeddingClass);

            int ThisDiscreteIndex = EmbeddingClass.DiscreteList.IndexOf(Discrete);
            int OtherDiscreteIndex = EmbeddingClass.DiscreteList.IndexOf(OtherDiscrete);

            Debug.Assert(ThisDiscreteIndex != -1 && OtherDiscreteIndex != -1);

            return ThisDiscreteIndex > OtherDiscreteIndex;
        }*/
    }
}
