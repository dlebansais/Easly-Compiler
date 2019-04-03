﻿namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IIndexerType.
    /// </summary>
    public interface IIndexerType : BaseNode.IIndexerType, IObjectType, INodeWithReplicatedBlocks, ICompiledType
    {
    }

    /// <summary>
    /// Compiler IIndexerType.
    /// </summary>
    public class IndexerType : BaseNode.IndexerType, IIndexerType
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.IndexParameterBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> IndexParameterList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.GetRequireBlocks"/>.
        /// </summary>
        public IList<IAssertion> GetRequireList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.GetEnsureBlocks"/>.
        /// </summary>
        public IList<IAssertion> GetEnsureList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.GetExceptionIdentifierBlocks"/>.
        /// </summary>
        public IList<IIdentifier> GetExceptionIdentifierList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.SetRequireBlocks"/>.
        /// </summary>
        public IList<IAssertion> SetRequireList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.SetEnsureBlocks"/>.
        /// </summary>
        public IList<IAssertion> SetEnsureList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.SetExceptionIdentifierBlocks"/>.
        /// </summary>
        public IList<IIdentifier> SetExceptionIdentifierList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyTypeArgument">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyTypeArgument, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyTypeArgument)
            {
                case nameof(IndexParameterBlocks):
                    TargetList = (IList)IndexParameterList;
                    break;

                case nameof(GetRequireBlocks):
                    TargetList = (IList)GetRequireList;
                    break;

                case nameof(GetEnsureBlocks):
                    TargetList = (IList)GetEnsureList;
                    break;

                case nameof(GetExceptionIdentifierBlocks):
                    TargetList = (IList)GetExceptionIdentifierList;
                    break;

                case nameof(SetRequireBlocks):
                    TargetList = (IList)SetRequireList;
                    break;

                case nameof(SetEnsureBlocks):
                    TargetList = (IList)SetEnsureList;
                    break;

                case nameof(SetExceptionIdentifierBlocks):
                    TargetList = (IList)SetExceptionIdentifierList;
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
    }
}
