namespace EaslyCompiler
{
    using System.Diagnostics;

    /// <summary>
    /// A C# feature.
    /// </summary>
    public interface ICSharpFeature
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        ICompiledFeature Source { get; }

        /// <summary>
        /// The class where the feature is declared.
        /// </summary>
        ICSharpClass Owner { get; }

        /// <summary>
        /// The source feature instance.
        /// </summary>
        IFeatureInstance Instance { get; }

        /// <summary>
        /// True if this feature as an override of a virtual parent.
        /// </summary>
        bool IsOverride { get; }

        /// <summary>
        /// The name of the precursor if it's implemented as a separate feature in the same class. Can be null.
        /// </summary>
        string CoexistingPrecursorName { get; }

        /// <summary>
        /// Mark this feature as an override of a virtual parent.
        /// </summary>
        void MarkAsOverride();
    }

    /// <summary>
    /// A C# feature.
    /// </summary>
    /// <typeparam name="T">The corresponding compiler node.</typeparam>
    public interface ICSharpFeature<T> : ICSharpSource<T>
        where T : class, ICompiledFeature
    {
        /// <summary>
        /// The class where the feature is declared.
        /// </summary>
        ICSharpClass Owner { get; }

        /// <summary>
        /// The source feature instance.
        /// </summary>
        IFeatureInstance Instance { get; }

        /// <summary>
        /// True if this feature as an override of a virtual parent.
        /// </summary>
        bool IsOverride { get; }

        /// <summary>
        /// The name of the precursor if it's implemented as a separate feature in the same class. Can be null.
        /// </summary>
        string CoexistingPrecursorName { get; }

        /// <summary>
        /// Mark this feature as an override of a virtual parent.
        /// </summary>
        void MarkAsOverride();
    }

    /// <summary>
    /// A C# feature.
    /// </summary>
    /// <typeparam name="T">The corresponding compiler node.</typeparam>
    public class CSharpFeature<T> : CSharpSource<T>, ICSharpFeature<T>, ICSharpFeature
        where T : class, ICompiledFeature
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFeature{T}"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpFeature(ICSharpClass owner, IFeatureInstance instance, T source)
            : base(source)
        {
            Owner = owner;
            Instance = instance;

            Debug.Assert(Instance.Feature.Item == Source);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        ICompiledFeature ICSharpFeature.Source { get { return Source; } }

        /// <summary>
        /// The class where the feature is declared.
        /// </summary>
        public ICSharpClass Owner { get; }
        ICSharpClass ICSharpFeature.Owner { get { return Owner; } }

        /// <summary>
        /// The source feature instance.
        /// </summary>
        public IFeatureInstance Instance { get; }
        IFeatureInstance ICSharpFeature.Instance { get { return Instance ; } }

        /// <summary>
        /// True if this feature as an override of a virtual parent.
        /// </summary>
        public bool IsOverride { get; private set; }
        bool ICSharpFeature.IsOverride { get { return IsOverride; } }

        /// <summary>
        /// The name of the precursor if it's implemented as a separate feature in the same class. Can be null.
        /// </summary>
        public string CoexistingPrecursorName { get; }
        string ICSharpFeature.CoexistingPrecursorName { get { return CoexistingPrecursorName; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Mark this feature as an override of a virtual parent.
        /// </summary>
        public void MarkAsOverride()
        {
            Debug.Assert(!IsOverride);

            IsOverride = true;
        }
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return Source.ToString();
        }
        #endregion
    }
}
