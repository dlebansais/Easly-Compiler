namespace EaslyCompiler
{
    using System.Diagnostics;
    using BaseNodeHelper;

    /// <summary>
    /// Represents a constant number with known value.
    /// </summary>
    public interface IManifestLanguageConstant : ILanguageConstant
    {
        /// <summary>
        /// The constant value.
        /// </summary>
        ICanonicalNumber Number { get; }
    }

    /// <summary>
    /// Represents a constant number with known value.
    /// </summary>
    public class ManifestLanguageConstant : LanguageConstant, IManifestLanguageConstant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestLanguageConstant"/> class.
        /// </summary>
        /// <param name="number">The constant value.</param>
        public ManifestLanguageConstant(ICanonicalNumber number)
        {
            Number = number;
        }

        /// <summary>
        /// The constant value.
        /// </summary>
        public ICanonicalNumber Number { get; }

        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsCompatibleWith(ILanguageConstant other)
        {
            return other is IManifestLanguageConstant;
        }

        /// <summary>
        /// Checks if this instance is greater than another constant.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsGreater(ILanguageConstant other)
        {
            Debug.Assert(IsCompatibleWith(other));

            IManifestLanguageConstant AsManifestLanguageConstant = other as IManifestLanguageConstant;
            Debug.Assert(AsManifestLanguageConstant != null);

            return Number.IsGreater(AsManifestLanguageConstant.Number);
        }
    }
}
