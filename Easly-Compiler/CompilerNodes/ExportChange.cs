namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IExportChange.
    /// </summary>
    public interface IExportChange : BaseNode.IExportChange, INode, INodeWithReplicatedBlocks, ISource
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.ExportChange.IdentifierBlocks"/>.
        /// </summary>
        IList<IIdentifier> IdentifierList { get; }

        /// <summary>
        /// Table of valid identifiers.
        /// </summary>
        IHashtableEx<string, IIdentifier> IdentifierTable { get; }

        /// <summary>
        /// Valid export identifier.
        /// </summary>
        OnceReference<string> ValidExportIdentifier { get; }
    }

    /// <summary>
    /// Compiler IExportChange.
    /// </summary>
    public class ExportChange : BaseNode.ExportChange, IExportChange
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.ExportChange.IdentifierBlocks"/>.
        /// </summary>
        public IList<IIdentifier> IdentifierList { get; } = new List<IIdentifier>();

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
                case nameof(IdentifierBlocks):
                    TargetList = (IList)IdentifierList;
                    break;
            }

            Debug.Assert(TargetList != null);
            Debug.Assert(TargetList.Count == 0);

            foreach (BaseNode.INode Node in nodeList)
                TargetList.Add(Node);
        }
        #endregion

        #region Implementation of ISource
        /// <summary>
        /// The parent node, null if root.
        /// </summary>
        public ISource ParentSource { get; private set; }

        /// <summary>
        /// The parent class, null if none.
        /// </summary>
        public IClass EmbeddingClass { get; private set; }

        /// <summary>
        /// The parent feature, null if none.
        /// </summary>
        public IFeature EmbeddingFeature { get; private set; }

        /// <summary>
        /// The parent overload, null if none.
        /// </summary>
        public IQueryOverload EmbeddingOverload { get; private set; }

        /// <summary>
        /// The parent body, null if none.
        /// </summary>
        public IBody EmbeddingBody { get; private set; }

        /// <summary>
        /// The parent assertion, null if none.
        /// </summary>
        public IAssertion EmbeddingAssertion { get; private set; }

        /// <summary>
        /// Initializes parents based on the provided <paramref name="parentSource"/> node.
        /// </summary>
        /// <param name="parentSource">The parent node.</param>
        public virtual void InitializeSource(ISource parentSource)
        {
            ParentSource = parentSource;

            EmbeddingClass = parentSource is IClass AsClass ? AsClass : parentSource?.EmbeddingClass;
            EmbeddingFeature = parentSource is IFeature AsFeature ? AsFeature : parentSource?.EmbeddingFeature;
            EmbeddingOverload = parentSource is IQueryOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
            EmbeddingBody = parentSource is IBody AsBody ? AsBody : parentSource?.EmbeddingBody;
            EmbeddingAssertion = parentSource is IAssertion AsAssertion ? AsAssertion : parentSource?.EmbeddingAssertion;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Table of valid identifiers.
        /// </summary>
        public IHashtableEx<string, IIdentifier> IdentifierTable { get; } = new HashtableEx<string, IIdentifier>();

        /// <summary>
        /// Valid export identifier.
        /// </summary>
        public OnceReference<string> ValidExportIdentifier { get; } = new OnceReference<string>();
        #endregion
    }
}
