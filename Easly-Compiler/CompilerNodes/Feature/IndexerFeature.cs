namespace CompilerNode
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IIndexerFeature.
    /// </summary>
    public interface IIndexerFeature : IFeature, INodeWithReplicatedBlocks, ICompiledFeature, IFeatureWithPrecursor, IGetterSetterScopeHolder, INodeWithResult, IFeatureWithEntity, IFeatureWithNumberType
    {
        /// <summary>
        /// Gets or sets the indexed value type.
        /// </summary>
        BaseNode.ObjectType EntityType { get; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        BaseNode.IBlockList<BaseNode.EntityDeclaration> IndexParameterBlocks { get; }

        /// <summary>
        /// Gets or sets whether the index accepts extra parameters.
        /// </summary>
        BaseNode.ParameterEndStatus ParameterEnd { get; }

        /// <summary>
        /// Gets or sets the list of other features this indexer modifies.
        /// </summary>
        BaseNode.IBlockList<BaseNode.Identifier> ModifiedQueryBlocks { get; }

        /// <summary>
        /// Gets or sets the getter body.
        /// </summary>
        IOptionalReference<BaseNode.Body> GetterBody { get; }

        /// <summary>
        /// Gets or sets the setter body.
        /// </summary>
        IOptionalReference<BaseNode.Body> SetterBody { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerFeature.IndexParameterBlocks"/>.
        /// </summary>
        IList<IEntityDeclaration> IndexParameterList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerFeature.ModifiedQueryBlocks"/>.
        /// </summary>
        IList<IIdentifier> ModifiedQueryList { get; }

        /// <summary>
        /// The name of the resolved indexer type.
        /// </summary>
        OnceReference<ITypeName> ResolvedEntityTypeName { get; }

        /// <summary>
        /// The resolved indexer type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedEntityType { get; }

        /// <summary>
        /// Table of resolved parameters.
        /// </summary>
        ISealableList<IParameter> ParameterTable { get; }
    }

    /// <summary>
    /// Compiler IIndexerFeature.
    /// </summary>
    public class IndexerFeature : BaseNode.IndexerFeature, IIndexerFeature
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerFeature.IndexParameterBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> IndexParameterList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerFeature.ModifiedQueryBlocks"/>.
        /// </summary>
        public IList<IIdentifier> ModifiedQueryList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyName">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyName, List<BaseNode.Node> nodeList)
        {
            IList TargetList = null;

            switch (propertyName)
            {
                case nameof(IndexParameterBlocks):
                    TargetList = (IList)IndexParameterList;
                    break;

                case nameof(ModifiedQueryBlocks):
                    TargetList = (IList)ModifiedQueryList;
                    break;
            }

            Debug.Assert(TargetList != null);
            Debug.Assert(TargetList.Count == 0);

            foreach (BaseNode.Node Node in nodeList)
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
        public IOverload EmbeddingOverload { get; private set; }

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
            EmbeddingOverload = parentSource is IOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
            EmbeddingBody = parentSource is IBody AsBody ? AsBody : parentSource?.EmbeddingBody;
            EmbeddingAssertion = parentSource is IAssertion AsAssertion ? AsAssertion : parentSource?.EmbeddingAssertion;
        }

        /// <summary>
        /// Reset some intermediate results.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to reset.</param>
        public void Reset(IRuleTemplateList ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                ResolvedAgentTypeName = new OnceReference<ITypeName>();
                ResolvedAgentType = new OnceReference<ICompiledType>();
                ResolvedEffectiveTypeName = new OnceReference<ITypeName>();
                ResolvedEffectiveType = new OnceReference<ICompiledType>();
                ValidFeatureName = new OnceReference<IFeatureName>();
                ResolvedFeature = new OnceReference<ICompiledFeature>();
                ResolvedEntityTypeName = new OnceReference<ITypeName>();
                ResolvedEntityType = new OnceReference<ICompiledType>();
                ParameterTable = new SealableList<IParameter>();
                LocalScope = new SealableDictionary<string, IScopeAttributeFeature>();
                LocalGetScope = new SealableDictionary<string, IScopeAttributeFeature>();
                LocalSetScope = new SealableDictionary<string, IScopeAttributeFeature>();
                InnerScopes = new List<IScopeHolder>();
                InnerGetScopes = new List<IScopeHolder>();
                InnerSetScopes = new List<IScopeHolder>();
                FullScope = new SealableDictionary<string, IScopeAttributeFeature>();
                FullGetScope = new SealableDictionary<string, IScopeAttributeFeature>();
                FullSetScope = new SealableDictionary<string, IScopeAttributeFeature>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                AdditionalScope = new SealableDictionary<string, IScopeAttributeFeature>();
                AdditionalScope.Seal();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsCallingPrecursor = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IRuleTemplateList ruleTemplateList)
        {
            bool IsResolved = false;

            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Contract || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = ResolvedFeature.IsAssigned;
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
        /// Name of the agent type associated to the feature.
        /// </summary>
        public OnceReference<ITypeName> ResolvedAgentTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The agent type associated to the feature.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedAgentType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// The name of the type to use, as source or destination, for the purpose of path searching, assignment and query.
        /// </summary>
        public OnceReference<ITypeName> ResolvedEffectiveTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The type to use, as source or destination, for the purpose of path searching, assignment and query.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedEffectiveType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion

        #region Implementation of IFeatureWithPrecursor
        /// <summary>
        /// True if the feature is calling a precursor.
        /// </summary>
        public bool IsCallingPrecursor { get; private set; }

        /// <summary>
        /// Sets the <see cref="IsCallingPrecursor"/> property.
        /// </summary>
        public void MarkAsCallingPrecursor()
        {
            IsCallingPrecursor = true;
        }
        #endregion

        #region Implementation of IGetterSetterScopeHolder
        /// <summary>
        /// Entities local to a scope.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> LocalScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// Entities local to a scope, getter only.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> LocalGetScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// Entities local to a scope, setter only.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> LocalSetScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// Additional entities such as loop indexer.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> AdditionalScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

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
        public ISealableDictionary<string, IScopeAttributeFeature> FullScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// All reachable entities, getter only.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> FullGetScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// All reachable entities, setter only.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> FullSetScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();
        #endregion

        #region Implementation of IFeatureWithEntity
        /// <summary>
        /// Guid of the language type corresponding to the entity object for an instance of this class.
        /// </summary>
        public Guid EntityGuid { get { return LanguageClasses.IndexerEntity.Guid; } }

        /// <summary>
        /// The source node associated to this instance.
        /// </summary>
        public ISource Location { get { return this; } }
        #endregion

        #region Compiler
        /// <summary>
        /// The name of the resolved indexer type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedEntityTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved indexer type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedEntityType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// Table of resolved parameters.
        /// </summary>
        public ISealableList<IParameter> ParameterTable { get; private set; } = new SealableList<IParameter>();

        /// <summary>
        /// The name of the resolved result type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedResultTypeName { get { return ResolvedEntityTypeName; } }

        /// <summary>
        /// The resolved result type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedResultType { get { return ResolvedEntityType; } }
        #endregion

        #region Numbers
        /// <summary>
        /// The number kind if the constant type is a number.
        /// </summary>
        public NumberKinds NumberKind
        {
            get
            {
                if (ResolvedEntityType.Item is ICompiledNumberType AsNumberType)
                    return AsNumberType.GetDefaultNumberKind();
                else
                    return NumberKinds.NotApplicable;
            }
        }

        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
            if (GetterBody.IsAssigned)
                ((IBody)GetterBody.Item).RestartNumberType(ref isChanged);

            if (SetterBody.IsAssigned)
                ((IBody)SetterBody.Item).RestartNumberType(ref isChanged);
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            if (GetterBody.IsAssigned)
                ((IBody)GetterBody.Item).CheckNumberType(ref isChanged);

            if (SetterBody.IsAssigned)
                ((IBody)SetterBody.Item).CheckNumberType(ref isChanged);
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
            if (GetterBody.IsAssigned)
                ((IBody)GetterBody.Item).ValidateNumberType(errorList);

            if (SetterBody.IsAssigned)
                ((IBody)SetterBody.Item).ValidateNumberType(errorList);
        }
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return $"Indexer";
        }
        #endregion
    }
}
