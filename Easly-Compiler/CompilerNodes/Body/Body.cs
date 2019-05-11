namespace CompilerNode
{
    using System.Collections.Generic;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IBody.
    /// </summary>
    public interface IBody : BaseNode.IBody, INode, INodeWithReplicatedBlocks, ISource
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Body.RequireBlocks"/>.
        /// </summary>
        IList<IAssertion> RequireList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Body.EnsureBlocks"/>.
        /// </summary>
        IList<IAssertion> EnsureList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Body.ExceptionIdentifierBlocks"/>.
        /// </summary>
        IList<IIdentifier> ExceptionIdentifierList { get; }

        /// <summary>
        /// Entities local to a scope.
        /// </summary>
        IHashtableEx<string, IScopeAttributeFeature> LocalScope { get; }

        /// <summary>
        /// Tags for tag expressions.
        /// </summary>
        IHashtableEx<string, IExpression> ResolvedTagTable { get; }

        /// <summary>
        /// Types of results.
        /// </summary>
        OnceReference<IList<IExpressionType>> ResolvedResult { get; }

        /// <summary>
        /// Resolved list of require assertions.
        /// </summary>
        OnceReference<IList<IAssertion>> ResolvedRequireList { get; }

        /// <summary>
        /// Resolved list of ensure assertions.
        /// </summary>
        OnceReference<IList<IAssertion>> ResolvedEnsureList { get; }
    }
}
