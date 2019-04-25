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
    public interface IPropertyFeature : BaseNode.IPropertyFeature, IFeature, IFeatureWithName, INodeWithReplicatedBlocks, ICompiledFeature, IGetterSetterScopeHolder, INodeWithResult
    {
        /// <summary>
        /// The name of the resolved property type.
        /// </summary>
        OnceReference<ITypeName> ResolvedEntityTypeName { get; }

        /// <summary>
        /// The resolved property type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedEntityType { get; }
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
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to reset.</param>
        public void Reset(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                ResolvedFeatureTypeName = new OnceReference<ITypeName>();
                ResolvedFeatureType = new OnceReference<ICompiledType>();
                ValidFeatureName = new OnceReference<IFeatureName>();
                ResolvedFeature = new OnceReference<ICompiledFeature>();
                ResolvedEntityTypeName = new OnceReference<ITypeName>();
                ResolvedEntityType = new OnceReference<ICompiledType>();
                LocalScope = new HashtableEx<string, IScopeAttributeFeature>();
                LocalGetScope = new HashtableEx<string, IScopeAttributeFeature>();
                LocalSetScope = new HashtableEx<string, IScopeAttributeFeature>();
                InnerScopes = new List<IScopeHolder>();
                InnerGetScopes = new List<IScopeHolder>();
                InnerSetScopes = new List<IScopeHolder>();
                FullScope = new HashtableEx<string, IScopeAttributeFeature>();
                FullGetScope = new HashtableEx<string, IScopeAttributeFeature>();
                FullSetScope = new HashtableEx<string, IScopeAttributeFeature>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsResolved = false;

            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                Debug.Assert(ResolvedEntityTypeName.IsAssigned == ResolvedEntityType.IsAssigned);
                Debug.Assert(ResolvedFeatureTypeName.IsAssigned == ResolvedFeatureType.IsAssigned);
                IsResolved = ResolvedFeature.IsAssigned;
                Debug.Assert(ResolvedEntityType.IsAssigned == IsResolved);
                Debug.Assert(ResolvedFeatureType.IsAssigned == IsResolved);
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IFeature
        /// <summary>
        /// The resolved feature name.
        /// </summary>
        public OnceReference<IFeatureName> ValidFeatureName { get; private set; } = new OnceReference<IFeatureName>();

        /// <summary>
        /// The resolved feature.
        /// </summary>
        public OnceReference<ICompiledFeature> ResolvedFeature { get; private set; } = new OnceReference<ICompiledFeature>();
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

        /// <summary>
        /// Name of the associated type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedFeatureTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// Associated type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedFeatureType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion

        #region Implementation of IGetterSetterScopeHolder
        /// <summary>
        /// Entities local to a scope.
        /// </summary>
        public IHashtableEx<string, IScopeAttributeFeature> LocalScope { get; private set; } = new HashtableEx<string, IScopeAttributeFeature>();

        /// <summary>
        /// Entities local to a scope, getter only.
        /// </summary>
        public IHashtableEx<string, IScopeAttributeFeature> LocalGetScope { get; private set; } = new HashtableEx<string, IScopeAttributeFeature>();

        /// <summary>
        /// Entities local to a scope, setter only.
        /// </summary>
        public IHashtableEx<string, IScopeAttributeFeature> LocalSetScope { get; private set; } = new HashtableEx<string, IScopeAttributeFeature>();

        /// <summary>
        /// List of scopes containing the current instance.
        /// </summary>
        public IList<IScopeHolder> InnerScopes { get; private set; } = new List<IScopeHolder>();

        /// <summary>
        /// List of scopes containing the current instance, getter only.
        /// </summary>
        public IList<IScopeHolder> InnerGetScopes { get; private set; } = new List<IScopeHolder>();

        /// <summary>
        /// List of scopes containing the current instance, setter only.
        /// </summary>
        public IList<IScopeHolder> InnerSetScopes { get; private set; } = new List<IScopeHolder>();

        /// <summary>
        /// All reachable entities.
        /// </summary>
        public IHashtableEx<string, IScopeAttributeFeature> FullScope { get; private set; } = new HashtableEx<string, IScopeAttributeFeature>();

        /// <summary>
        /// All reachable entities, getter only.
        /// </summary>
        public IHashtableEx<string, IScopeAttributeFeature> FullGetScope { get; private set; } = new HashtableEx<string, IScopeAttributeFeature>();

        /// <summary>
        /// All reachable entities, setter only.
        /// </summary>
        public IHashtableEx<string, IScopeAttributeFeature> FullSetScope { get; private set; } = new HashtableEx<string, IScopeAttributeFeature>();
        #endregion

        #region Compiler
        /// <summary>
        /// The name of the resolved property type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedEntityTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved property type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedEntityType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// The name of the resolved result type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedResultTypeName { get { return ResolvedEntityTypeName; } }

        /// <summary>
        /// The resolved result type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedResultType { get { return ResolvedEntityType; } }
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return $"Property '{EntityName.Text}'";
        }
        #endregion
    }
}
