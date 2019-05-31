namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# constant.
    /// </summary>
    public interface ICSharpConstantFeature : ICSharpFeature<IConstantFeature>, ICSharpFeatureWithName
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        new IConstantFeature Source { get; }

        /// <summary>
        /// The class where the feature is declared.
        /// </summary>
        new ICSharpClass Owner { get; }

        /// <summary>
        /// The source feature instance.
        /// </summary>
        new IFeatureInstance Instance { get; }

        /// <summary>
        /// True if this feature as an override of a virtual parent.
        /// </summary>
        new bool IsOverride { get; }
    }

    /// <summary>
    /// A C# constant.
    /// </summary>
    public class CSharpConstantFeature : CSharpFeature<IConstantFeature>, ICSharpConstantFeature
    {
        #region Init
        /// <summary>
        /// Create a new C# constant.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        public static ICSharpConstantFeature Create(ICSharpClass owner, IFeatureInstance instance, IConstantFeature source)
        {
            return new CSharpConstantFeature(owner, instance, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpConstantFeature"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpConstantFeature(ICSharpClass owner, IFeatureInstance instance, IConstantFeature source)
            : base(owner, instance, source)
        {
            Name = Source.ValidFeatureName.Item.Name;
        }
        #endregion

        #region Properties
        ICompiledFeature ICSharpFeature.Source { get { return Source; } }
        ICSharpClass ICSharpFeature.Owner { get { return Owner; } }
        IFeatureInstance ICSharpFeature.Instance { get { return Instance; } }
        bool ICSharpFeature.IsOverride { get { return IsOverride; } }

        /// <summary>
        /// The feature name.
        /// </summary>
        public string Name { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Initializes the feature.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void Init(ICSharpContext context)
        {
        }
        #endregion
    }
}
