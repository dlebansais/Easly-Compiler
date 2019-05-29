namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# indexer.
    /// </summary>
    public interface ICSharpIndexerFeature : ICSharpFeature<IIndexerFeature>, ICSharpFeature
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        new IIndexerFeature Source { get; }

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
        /// True if this feature must be both read and write.
        /// </summary>
        bool IsForcedReadWrite { get; }

        /// <summary>
        /// Mark this feature as both read and write.
        /// </summary>
        void MarkAsForcedReadWrite();
    }

    /// <summary>
    /// A C# indexer.
    /// </summary>
    public class CSharpIndexerFeature : CSharpFeature<IIndexerFeature>, ICSharpIndexerFeature
    {
        #region Init
        /// <summary>
        /// Create a new C# indexer.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        public static ICSharpIndexerFeature Create(ICSharpClass owner, IFeatureInstance instance, IIndexerFeature source)
        {
            return new CSharpIndexerFeature(owner, instance, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpIndexerFeature"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpIndexerFeature(ICSharpClass owner, IFeatureInstance instance, IIndexerFeature source)
            : base(owner, instance, source)
        {
        }
        #endregion

        #region Properties
        ICompiledFeature ICSharpFeature.Source { get { return Source; } }
        ICSharpClass ICSharpFeature.Owner { get { return Owner; } }
        IFeatureInstance ICSharpFeature.Instance { get { return Instance; } }
        bool ICSharpFeature.IsOverride { get { return IsOverride; } }

        /// <summary>
        /// True if this feature must be both read and write.
        /// </summary>
        public bool IsForcedReadWrite { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Mark this feature as both read and write.
        /// </summary>
        public void MarkAsForcedReadWrite()
        {
            Debug.Assert(!IsForcedReadWrite);

            IsForcedReadWrite = true;
        }
        #endregion
    }
}
