namespace CompilerNode
{
    /// <summary>
    /// Compiler IBlockList.
    /// </summary>
    /// <typeparam name="IN">Items interface type.</typeparam>
    /// <typeparam name="N">Items type.</typeparam>
    public interface IBlockList<IN, N> : BaseNode.IBlockList<IN, N>
        where IN : class, BaseNode.INode
        where N : BaseNode.Node, IN
    {
    }

    /// <summary>
    /// Compiler IBlockList.
    /// </summary>
    /// <typeparam name="IN">Items interface type.</typeparam>
    /// <typeparam name="N">Items type.</typeparam>
    public class BlockList<IN, N> : BaseNode.BlockList<IN, N>, IBlockList<IN, N>
        where IN : class, BaseNode.INode
        where N : BaseNode.Node, IN
    {
    }
}
