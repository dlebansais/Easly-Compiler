namespace EaslyCompiler
{
    using System.Collections.Generic;
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

        /// <summary>
        /// Reset some intermediate results.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to reset.</param>
        void Reset(IList<IRuleTemplate> ruleTemplateList);

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        bool IsResolved(IList<IRuleTemplate> ruleTemplateList);
    }
}
