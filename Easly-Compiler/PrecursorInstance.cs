namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// An instance of a precursor.
    /// </summary>
    public interface IPrecursorInstance
    {
        /// <summary>
        /// The ancestor type.
        /// </summary>
        IClassType Ancestor { get; }

        /// <summary>
        /// The precursor instance.
        /// </summary>
        IFeatureInstance Precursor { get; }
    }

    /// <summary>
    /// An instance of a precursor.
    /// </summary>
    public class PrecursorInstance : IPrecursorInstance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrecursorInstance"/> class.
        /// </summary>
        /// <param name="ancestor">The ancestor type.</param>
        /// <param name="precursor">The precursor instance.</param>
        public PrecursorInstance(IClassType ancestor, IFeatureInstance precursor)
        {
            Ancestor = ancestor;
            Precursor = precursor;
        }

        /// <summary>
        /// The ancestor type.
        /// </summary>
        public IClassType Ancestor { get; }

        /// <summary>
        /// The precursor instance.
        /// </summary>
        public IFeatureInstance Precursor { get; }
    }
}
