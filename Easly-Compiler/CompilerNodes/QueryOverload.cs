namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IQueryOverload.
    /// </summary>
    public interface IQueryOverload : BaseNode.IQueryOverload, INode, INodeWithReplicatedBlocks, ISource, IScopeHolder
    {
        #region Compiler
        /// <summary>
        /// Indicates if the overload is deferred in another class.
        /// </summary>
        bool IsDeferredOverload { get; }

        /// <summary>
        /// True if the overload contains an extern body.
        /// </summary>
        bool HasExternBody { get; }

        /// <summary>
        /// True if the overload contains a precursor body.
        /// </summary>
        bool HasPrecursorBody { get; }
        #endregion
    }

    /// <summary>
    /// Compiler IQueryOverload.
    /// </summary>
    public class QueryOverload : BaseNode.QueryOverload, IQueryOverload
    {
        #region Compiler
        /// <summary>
        /// Indicates if the overload is deferred in another class.
        /// </summary>
        public bool IsDeferredOverload { get { return ((ICompiledBody)QueryBody).IsDeferredBody; } }

        /// <summary>
        /// True if the overload contains an extern body.
        /// </summary>
        public bool HasExternBody { get { return QueryBody is IExternBody; } }

        /// <summary>
        /// True if the overload contains a precursor body.
        /// </summary>
        public bool HasPrecursorBody { get { return QueryBody is IPrecursorBody; } }
        #endregion

        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverload.ParameterBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> ParameterList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverload.ResultBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> ResultList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverload.ModifiedQueryBlocks"/>.
        /// </summary>
        public IList<IIdentifier> ModifiedQueryList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyEntityDeclaration">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyEntityDeclaration, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyEntityDeclaration)
            {
                case nameof(ParameterBlocks):
                    TargetList = (IList)ParameterList;
                    break;

                case nameof(ResultBlocks):
                    TargetList = (IList)ResultList;
                    break;

                case nameof(ModifiedQueryBlocks):
                    TargetList = (IList)ModifiedQueryList;
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
