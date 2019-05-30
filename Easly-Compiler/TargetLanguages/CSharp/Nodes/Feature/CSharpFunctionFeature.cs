namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# function.
    /// </summary>
    public interface ICSharpFunctionFeature : ICSharpFeature<IFunctionFeature>, ICSharpRoutineFeature
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        new IFunctionFeature Source { get; }

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
    /// A C# function.
    /// </summary>
    public class CSharpFunctionFeature : CSharpFeature<IFunctionFeature>, ICSharpFunctionFeature
    {
        #region Init
        /// <summary>
        /// Create a new C# function.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        public static ICSharpFunctionFeature Create(ICSharpClass owner, IFeatureInstance instance, IFunctionFeature source)
        {
            return new CSharpFunctionFeature(owner, instance, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFunctionFeature"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpFunctionFeature(ICSharpClass owner, IFeatureInstance instance, IFunctionFeature source)
            : base(owner, instance, source)
        {
            Name = Source.ValidFeatureName.Item.Name;

            foreach (IQueryOverload Overload in source.OverloadList)
            {
                ICSharpQueryOverload NewOverload = CSharpQueryOverload.Create(Overload, owner);
                OverloadList.Add(NewOverload);
            }
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
    }
}
