namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IPropertyFeature.
    /// </summary>
    public interface IPropertyFeature : BaseNode.IPropertyFeature, IFeatureWithName, INodeWithReplicatedBlocks, ICompiledFeature, IScopeHolder
    {
    }

    /// <summary>
    /// Compiler IPropertyFeature.
    /// </summary>
    public class PropertyFeature : BaseNode.PropertyFeature, IPropertyFeature
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.PropertyFeature.ModifiedQueryBlocks"/>.
        /// </summary>
        public IList<IIdentifier> ModifiedQueryList { get; } = new List<IIdentifier>();

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

        #region Implementation of ICompiledFeature
        /// <summary>
        /// Indicates if the feature is deferred in another class.
        /// </summary>
        public bool IsDeferredFeature { get { return (GetterBody.IsAssigned && ((ICompiledBody)GetterBody.Item).IsDeferredBody) || (SetterBody.IsAssigned && ((ICompiledBody)SetterBody.Item).IsDeferredBody); } }

        /// <summary>
        /// True if the feature contains extern bodies in its overloads.
        /// </summary>
        public bool HasExternBody { get { return (GetterBody.IsAssigned && GetterBody.Item is IExternBody) || (SetterBody.IsAssigned && SetterBody.Item is IExternBody); } }

        /// <summary>
        /// True if the feature contains precursor bodies in its overloads.
        /// </summary>
        public bool HasPrecursorBody { get { return (GetterBody.IsAssigned && GetterBody.Item is IPrecursorBody) || (SetterBody.IsAssigned && SetterBody.Item is IPrecursorBody); } }
        #endregion
    }
}
