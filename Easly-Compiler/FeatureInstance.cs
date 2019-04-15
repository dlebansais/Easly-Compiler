namespace EaslyCompiler
{
    using System.Collections.Generic;
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

        /// <summary>
        /// Inherited with a forget clause.
        /// </summary>
        bool IsForgotten { get; }

        /// <summary>
        /// Inherited with a keep clause.
        /// </summary>
        bool IsKept { get; }

        /// <summary>
        /// Inherited with a discontinue clause.
        /// </summary>
        bool IsDiscontinued { get; }

        /// <summary>
        /// Inherited from an effective body.
        /// </summary>
        bool InheritBySideAttribute { get; }

        /// <summary>
        /// List of precursors.
        /// </summary>
        IList<IPrecursorInstance> PrecursorList { get; }

        /// <summary>
        /// The first precursor in the inheritance tree.
        /// </summary>
        OnceReference<IPrecursorInstance> OriginalPrecursor { get; }
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
            : this(owner, feature, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureInstance"/> class.
        /// </summary>
        /// <param name="owner">The class with the instance. Can be different than the class that defines the feature.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="isKept">Inherited with a keep clause.</param>
        /// <param name="isDiscontinued">Inherited with a discontinue clause.</param>
        public FeatureInstance(IClass owner, ICompiledFeature feature, bool isKept, bool isDiscontinued)
        {
            // TODO: check if oncereference is really needed.
            Owner.Item = owner;
            Feature.Item = feature;
            IsForgotten = feature.IsDeferredFeature;
            IsKept = isKept;
            IsDiscontinued = isDiscontinued;
            InheritBySideAttribute = false;
        }

        /// <summary>
        /// The class with the instance. Can be different than the class that defines the feature.
        /// </summary>
        public OnceReference<IClass> Owner { get; } = new OnceReference<IClass>();

        /// <summary>
        /// The feature.
        /// </summary>
        public OnceReference<ICompiledFeature> Feature { get; } = new OnceReference<ICompiledFeature>();

        /// <summary>
        /// Inherited with a forget clause.
        /// </summary>
        public bool IsForgotten { get; private set; }

        /// <summary>
        /// Inherited with a keep clause.
        /// </summary>
        public bool IsKept { get; private set; }

        /// <summary>
        /// Inherited with a discontinue clause.
        /// </summary>
        public bool IsDiscontinued { get; private set; }

        /// <summary>
        /// Inherited from an effective body.
        /// </summary>
        public bool InheritBySideAttribute { get; private set; }

        /// <summary>
        /// List of precursors.
        /// </summary>
        public IList<IPrecursorInstance> PrecursorList { get; } = new List<IPrecursorInstance>();

        /// <summary>
        /// The first precursor in the inheritance tree.
        /// </summary>
        public OnceReference<IPrecursorInstance> OriginalPrecursor { get; private set; }
    }
}
