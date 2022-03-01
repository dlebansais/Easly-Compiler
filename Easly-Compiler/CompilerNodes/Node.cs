namespace CompilerNode
{
    /// <summary>
    /// Compiler INode.
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Gets or sets the node documentation.
        /// </summary>
        BaseNode.Document Documentation { get; }
    }
}
