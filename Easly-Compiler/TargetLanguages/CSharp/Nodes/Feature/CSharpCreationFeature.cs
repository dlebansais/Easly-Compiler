namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# constructor.
    /// </summary>
    public interface ICSharpCreationFeature : ICSharpFeature<ICreationFeature>, ICSharpRoutineFeature
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        new ICreationFeature Source { get; }

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
    /// A C# constructor.
    /// </summary>
    public class CSharpCreationFeature : CSharpFeature<ICreationFeature>, ICSharpCreationFeature
    {
        #region Init
        /// <summary>
        /// Create a new C# constructor.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        public static ICSharpCreationFeature Create(ICSharpClass owner, IFeatureInstance instance, ICreationFeature source)
        {
            return new CSharpCreationFeature(owner, instance, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCreationFeature"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpCreationFeature(ICSharpClass owner, IFeatureInstance instance, ICreationFeature source)
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

        /// <summary>
        /// The list of overloads.
        /// </summary>
        public IList<ICSharpOverload> OverloadList { get; } = new List<ICSharpOverload>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Initializes the feature.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void Init(ICSharpContext context)
        {
            foreach (ICommandOverload Overload in Source.OverloadList)
            {
                ICSharpCommandOverload NewOverload = CSharpCommandOverload.Create(context, Overload, Owner);
                OverloadList.Add(NewOverload);
            }
        }
        #endregion
    }
}
