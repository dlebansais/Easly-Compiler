namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IRoot.
    /// </summary>
    public interface IRoot : BaseNode.IRoot, INode, INodeWithReplicatedBlocks
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Root.ClassBlocks"/>.
        /// </summary>
        IList<IClass> ClassList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Root.LibraryBlocks"/>.
        /// </summary>
        IList<ILibrary> LibraryList { get; }
    }

    /// <summary>
    /// Compiler IRoot.
    /// </summary>
    public class Root : BaseNode.Root, IRoot
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Root.ClassBlocks"/>.
        /// </summary>
        public IList<IClass> ClassList { get; } = new List<IClass>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Root.LibraryBlocks"/>.
        /// </summary>
        public IList<ILibrary> LibraryList { get; } = new List<ILibrary>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyName">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyName, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyName)
            {
                case nameof(ClassBlocks):
                    TargetList = (IList)ClassList;
                    break;

                case nameof(LibraryBlocks):
                    TargetList = (IList)LibraryList;
                    break;
            }

            Debug.Assert(TargetList != null);
            Debug.Assert(TargetList.Count == 0);

            foreach (BaseNode.INode Node in nodeList)
                TargetList.Add(Node);
        }
        #endregion
    }
}
