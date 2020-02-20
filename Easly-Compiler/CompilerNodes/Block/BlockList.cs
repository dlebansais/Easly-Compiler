namespace CompilerNode
{
    /// <summary>
    /// Compiler IBlockList.
    /// </summary>
    /// <typeparam name="TINode">Items interface type.</typeparam>
    /// <typeparam name="TNode">Items type.</typeparam>
    public interface IBlockList<TINode, TNode> : BaseNode.IBlockList<TINode, TNode>
        where TINode : class, BaseNode.INode
        where TNode : BaseNode.Node, TINode
    {
    }

    /// <summary>
    /// Compiler IBlockList.
    /// </summary>
    /// <typeparam name="TINode">Items interface type.</typeparam>
    /// <typeparam name="TNode">Items type.</typeparam>
    public class BlockList<TINode, TNode> : BaseNode.BlockList<TINode, TNode>, IBlockList<TINode, TNode>
        where TINode : class, BaseNode.INode
        where TNode : BaseNode.Node, TINode
    {
    }
}
