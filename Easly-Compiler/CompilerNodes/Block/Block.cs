namespace CompilerNode
{
    /// <summary>
    /// Compiler IBlock.
    /// </summary>
    /// <typeparam name="IN">Items interface type.</typeparam>
    /// <typeparam name="N">Items type.</typeparam>
    public interface IBlock<IN, N> : BaseNode.IBlock<IN, N>
        where IN : class, BaseNode.INode
        where N : BaseNode.Node, IN
    {
    }

    /// <summary>
    /// Compiler IBlock.
    /// </summary>
    /// <typeparam name="IN">Items interface type.</typeparam>
    /// <typeparam name="N">Items type.</typeparam>
    public class Block<IN, N> : BaseNode.Block<IN, N>, IBlock<IN, N>
        where IN : class, BaseNode.INode
        where N : BaseNode.Node, IN
    {
    }
}
