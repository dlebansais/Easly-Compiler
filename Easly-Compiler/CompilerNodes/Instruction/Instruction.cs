namespace CompilerNode
{
    using System.Collections.Generic;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IInstruction.
    /// </summary>
    public interface IInstruction : BaseNode.IInstruction, INode, ISource, IScopeHolder
    {
        /// <summary>
        /// Types of results of the instruction.
        /// </summary>
        OnceReference<IList<IExpressionType>> ResolvedResult { get; }

        /// <summary>
        /// List of exceptions the instruction can throw.
        /// </summary>
        OnceReference<IList<IIdentifier>> ResolvedExceptions { get; }

        /// <summary>
        /// Gets a string representation of the instruction.
        /// </summary>
        string InstructionToString { get; }
    }
}
