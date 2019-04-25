namespace CompilerNode
{
    using EaslyCompiler;

    /// <summary>
    /// Compiler IInstruction.
    /// </summary>
    public interface IInstruction : BaseNode.IInstruction, INode, ISource, IScopeHolder
    {
        /// <summary>
        /// Gets a string representation of the instruction.
        /// </summary>
        string InstructionToString { get; }
    }
}
