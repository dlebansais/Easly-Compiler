namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using EaslyCompiler;

    /// <summary>
    /// Compiler ICommandOverload.
    /// </summary>
    public interface ICommandOverload : BaseNode.ICommandOverload, INode, INodeWithReplicatedBlocks, ISource, IScopeHolder
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
    /// Compiler ICommandOverload.
    /// </summary>
    public class CommandOverload : BaseNode.CommandOverload, ICommandOverload
    {
        #region Compiler
        /// <summary>
        /// Indicates if the overload is deferred in another class.
        /// </summary>
        public bool IsDeferredOverload { get { return ((ICompiledBody)CommandBody).IsDeferredBody; } }

        /// <summary>
        /// True if the overload contains an extern body.
        /// </summary>
        public bool HasExternBody { get { return CommandBody is IExternBody; } }

        /// <summary>
        /// True if the overload contains a precursor body.
        /// </summary>
        public bool HasPrecursorBody { get { return CommandBody is IPrecursorBody; } }
        #endregion

        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.CommandOverload.ParameterBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> ParameterList { get; } = new List<IEntityDeclaration>();

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
                case nameof(ParameterBlocks):
                    TargetList = (IList)ParameterList;
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
