namespace CompilerNode
{
    using EaslyCompiler;

    /// <summary>
    /// Compiler IInstruction.
    /// </summary>
    public interface IInstruction : BaseNode.IInstruction, INode, ISource, IScopeHolder
    {
    }
}
