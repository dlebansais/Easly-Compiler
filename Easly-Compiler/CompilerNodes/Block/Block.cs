namespace CompilerNode
{
    /// <summary>
    /// Compiler IBlock.
    /// </summary>
    /// <typeparam name="TINode">Items interface type.</typeparam>
    /// <typeparam name="TNode">Items type.</typeparam>
    public interface IBlock<TINode, TNode> : BaseNode.IBlock<TINode, TNode>
        where TINode : class, BaseNode.INode
        where TNode : BaseNode.Node, TINode
    {
    }

    /// <summary>
    /// Compiler IBlock.
    /// </summary>
    /// <typeparam name="TINode">Items interface type.</typeparam>
    /// <typeparam name="TNode">Items type.</typeparam>
    public class Block<TINode, TNode> : BaseNode.Block<TINode, TNode>, IBlock<TINode, TNode>
        where TINode : class, BaseNode.INode
        where TNode : BaseNode.Node, TINode
    {
    }
}
