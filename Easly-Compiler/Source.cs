namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// References to embedding nodes.
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// The parent node, null if root.
        /// </summary>
        ISource ParentSource { get; }

        /// <summary>
        /// The parent class, null if none.
        /// </summary>
        IClass EmbeddingClass { get; }

        /// <summary>
        /// The parent feature, null if none.
        /// </summary>
        IFeature EmbeddingFeature { get; }

        /// <summary>
        /// The parent overload, null if none.
        /// </summary>
        IQueryOverload EmbeddingOverload { get; }

        /// <summary>
        /// The parent body, null if none.
        /// </summary>
        IBody EmbeddingBody { get; }

        /// <summary>
        /// The parent assertion, null if none.
        /// </summary>
        IAssertion EmbeddingAssertion { get; }

        /// <summary>
        /// Initializes parents based on the provided <paramref name="parentSource"/> node.
        /// </summary>
        /// <param name="parentSource">The parent node.</param>
        void InitializeSource(ISource parentSource);
    }
}
