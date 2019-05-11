namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// Represents a discrete constant.
    /// </summary>
    public interface IDiscreteLanguageConstant : ILanguageConstant
    {
        /// <summary>
        /// The discrete value.
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
        /// <param name="discrete">The discrete value.</param>
        public DiscreteLanguageConstant(IDiscrete discrete)
        {
            Discrete = discrete;
        }

        /// <summary>
        /// The discrete value.
        /// </summary>
        public IDiscrete Discrete { get; }

        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsCompatibleWith(ILanguageConstant other)
        {
            bool Result = false;

            if (other is IDiscreteLanguageConstant AsDiscreteLanguageConstant)
            {
                IDiscrete OtherDiscrete = AsDiscreteLanguageConstant.Discrete;
                Result = Discrete.EmbeddingClass == OtherDiscrete.EmbeddingClass;
            }

            return Result;
        }

        /// <summary>
        /// Checks if another constant is greater than this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsGreater(ILanguageConstant other)
        {
            Debug.Assert(IsCompatibleWith(other));

            IDiscreteLanguageConstant AsDiscreteLanguageConstant = other as IDiscreteLanguageConstant;
            Debug.Assert(AsDiscreteLanguageConstant != null);

            IDiscrete OtherDiscrete = AsDiscreteLanguageConstant.Discrete;
            IClass EmbeddingClass = Discrete.EmbeddingClass;

            Debug.Assert(EmbeddingClass != null);
            Debug.Assert(EmbeddingClass == OtherDiscrete.EmbeddingClass);

            int ThisDiscreteIndex = EmbeddingClass.DiscreteList.IndexOf(Discrete);
            int OtherDiscreteIndex = EmbeddingClass.DiscreteList.IndexOf(OtherDiscrete);

            Debug.Assert(ThisDiscreteIndex != -1 && OtherDiscreteIndex != -1);

            return ThisDiscreteIndex > OtherDiscreteIndex;
        }
    }
}
