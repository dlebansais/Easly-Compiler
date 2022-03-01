namespace CompilerNode
{
    /// <summary>
    /// Compiler IBlockList.
    /// </summary>
    /// <typeparam name="TNode">Items type.</typeparam>
    public interface IBlockList<TNode> : BaseNode.IBlockList<TNode>
        where TNode : BaseNode.Node
    {
    }

    /// <summary>
    /// Compiler IBlockList.
    /// </summary>
    /// <typeparam name="TNode">Items type.</typeparam>
    public class BlockList<TNode> : BaseNode.BlockList<TNode>, IBlockList<TNode>
        where TNode : BaseNode.Node
    {
    }
}
