namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IDeferredBody.
    /// </summary>
    public interface IDeferredBody : BaseNode.IDeferredBody, IBody, ICompiledBody
    {
    }

    /// <summary>
    /// Compiler IDeferredBody.
    /// </summary>
    public class DeferredBody : BaseNode.DeferredBody, IDeferredBody
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Body.RequireBlocks"/>.
        /// </summary>
        public IList<IAssertion> RequireList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Body.EnsureBlocks"/>.
        /// </summary>
        public IList<IAssertion> EnsureList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Body.ExceptionIdentifierBlocks"/>.
        /// </summary>
        public IList<IIdentifier> ExceptionIdentifierList { get; } = new List<IIdentifier>();

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
                case nameof(RequireBlocks):
                    TargetList = (IList)RequireList;
                    break;

                case nameof(EnsureBlocks):
                    TargetList = (IList)EnsureList;
                    break;

                case nameof(ExceptionIdentifierBlocks):
                    TargetList = (IList)ExceptionIdentifierList;
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

        #region Implementation of ICompiledBody
        /// <summary>
        /// Indicates if the body is deferred in another class.
        /// </summary>
        public bool IsDeferredBody { get { return true; } }
        #endregion
    }
}
