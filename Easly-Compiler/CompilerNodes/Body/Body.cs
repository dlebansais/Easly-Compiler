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
        ISealableDictionary<string, IScopeAttributeFeature> LocalScope { get; }

        /// <summary>
        /// Tags for tag expressions.
        /// </summary>
        ISealableDictionary<string, IExpression> ResolvedTagTable { get; }

        /// <summary>
        /// Types of results.
        /// </summary>
        OnceReference<IResultType> ResolvedResult { get; }

        /// <summary>
        /// Resolved list of require assertions.
        /// </summary>
        OnceReference<IList<IAssertion>> ResolvedRequireList { get; }

        /// <summary>
        /// Resolved list of ensure assertions.
        /// </summary>
        OnceReference<IList<IAssertion>> ResolvedEnsureList { get; }

        /// <summary>
        /// Resolved list of exceptions the body can throw.
        /// </summary>
        OnceReference<IList<IIdentifier>> ResolvedExceptionIdentifierList { get; }

        /// <summary>
        /// Resolved list of instructions in the body.
        /// </summary>
        OnceReference<IList<IInstruction>> ResolvedInstructionList { get; }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        void CheckNumberType(ref bool isChanged);

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        void ValidateNumberType(IErrorList errorList);
    }
}
