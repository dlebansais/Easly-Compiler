namespace EaslyCompiler
{
    using System.Collections.Generic;

    /// <summary>
    /// INode contains block list properties.
    /// </summary>
    public interface INodeWithReplicatedBlocks
    {
        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyName">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        void FillReplicatedList(string propertyName, List<BaseNode.INode> nodeList);
    }
}
