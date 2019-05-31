namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# local attribute.
    /// </summary>
    public interface ICSharpScopeAttributeFeature : ICSharpFeature<IScopeAttributeFeature>, ICSharpFeatureWithName
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        new IScopeAttributeFeature Source { get; }

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

        /// <summary>
        /// The default value. Can be null.
        /// </summary>
        ICSharpExpression DefaultValue { get; }

        /// <summary>
        /// The attribute type.
        /// </summary>
        ICSharpType Type { get; }
    }

    /// <summary>
    /// A C# local attribute.
    /// </summary>
    public class CSharpScopeAttributeFeature : CSharpFeature<IScopeAttributeFeature>, ICSharpScopeAttributeFeature
    {
        #region Init
        /// <summary>
        /// Create a new C# local attribute.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="source">The source Easly feature.</param>
        public static ICSharpScopeAttributeFeature Create(ICSharpClass owner, IScopeAttributeFeature source)
        {
            return new CSharpScopeAttributeFeature(owner, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpScopeAttributeFeature"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpScopeAttributeFeature(ICSharpClass owner, IScopeAttributeFeature source)
            : base(owner, source)
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

        /// <summary>
        /// The default value. Can be null.
        /// </summary>
        public ICSharpExpression DefaultValue { get; private set; }

        /// <summary>
        /// The attribute type.
        /// </summary>
        public ICSharpType Type { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Initializes the feature.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void Init(ICSharpContext context)
        {
            Type = CSharpType.Create(context, Source.ResolvedFeatureType.Item);

            if (Source.DefaultValue.IsAssigned)
                DefaultValue = CSharpExpression.Create(context, Source.DefaultValue.Item);
        }
        #endregion
    }
}
