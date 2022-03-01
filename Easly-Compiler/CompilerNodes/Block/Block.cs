namespace CompilerNode
{
    /// <summary>
    /// Compiler IBlock.
    /// </summary>
    /// <typeparam name="TNode">Items type.</typeparam>
    public interface IBlock<TNode> : BaseNode.IBlock<TNode>
        where TNode : BaseNode.Node
    {
    }

    /// <summary>
    /// Compiler IBlock.
    /// </summary>
    /// <typeparam name="TNode">Items type.</typeparam>
    public class Block<TNode> : BaseNode.Block<TNode>, IBlock<TNode>
        where TNode : BaseNode.Node
    {
    }
}
