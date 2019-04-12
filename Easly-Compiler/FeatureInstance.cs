namespace EaslyCompiler
{
    using CompilerNode;
    using Easly;

    /// <summary>
    /// An instance of a feature in a class (direct or inherited).
    /// </summary>
    public interface IFeatureInstance
    {
        /// <summary>
        /// The class with the instance. Can be different than the class that defines the feature.
        /// </summary>
        OnceReference<IClass> Owner { get; }

        /// <summary>
        /// The feature.
        /// </summary>
        OnceReference<ICompiledFeature> Feature { get; }
    }

    /// <summary>
    /// An instance of a feature in a class (direct or inherited).
    /// </summary>
    public class FeatureInstance : IFeatureInstance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureInstance"/> class.
        /// </summary>
        /// <param name="owner">The class with the instance. Can be different than the class that defines the feature.</param>
        /// <param name="feature">The feature.</param>
        public FeatureInstance(IClass owner, ICompiledFeature feature)
        {
            // TODO: check if oncereference is really needed.
            Owner.Item = owner;
            Feature.Item = feature;
        }

        /// <summary>
        /// The class with the instance. Can be different than the class that defines the feature.
        /// </summary>
        public OnceReference<IClass> Owner { get; } = new OnceReference<IClass>();

        /// <summary>
        /// The feature.
        /// </summary>
        public OnceReference<ICompiledFeature> Feature { get; } = new OnceReference<ICompiledFeature>();
    }
}
