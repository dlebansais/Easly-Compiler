namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IProcedureType.
    /// </summary>
    public interface IProcedureType : BaseNode.IProcedureType, IObjectType, INodeWithReplicatedBlocks, ICompiledType
    {
    }

    /// <summary>
    /// Compiler IProcedureType.
    /// </summary>
    public class ProcedureType : BaseNode.ProcedureType, IProcedureType
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.ProcedureType.OverloadBlocks"/>.
        /// </summary>
        public IList<ICommandOverloadType> OverloadList { get; } = new List<ICommandOverloadType>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyOverload">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyOverload, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyOverload)
            {
                case nameof(OverloadBlocks):
                    TargetList = (IList)OverloadList;
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

        /// <summary>
        /// Reset some intermediate results.
        /// </summary>
        /// <param name="engine">The engine requesting reset.</param>
        public void Reset(InferenceEngine engine)
        {
            bool IsHandled = false;

            if (engine.RuleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }
        #endregion
    }
}
