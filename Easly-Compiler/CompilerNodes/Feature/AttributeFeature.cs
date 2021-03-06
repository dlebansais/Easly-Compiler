namespace CompilerNode
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IAttributeFeature.
    /// </summary>
    public interface IAttributeFeature : BaseNode.IAttributeFeature, IFeature, IFeatureWithName, INodeWithReplicatedBlocks, ICompiledFeature, IFeatureWithEvents, IFeatureWithEntity
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.AttributeFeature.EnsureBlocks"/>.
        /// </summary>
        IList<IAssertion> EnsureList { get; }

        /// <summary>
        /// The name of the resolved attribute type.
        /// </summary>
        OnceReference<ITypeName> ResolvedEntityTypeName { get; }

        /// <summary>
        /// The resolved attribute type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedEntityType { get; }
    }

    /// <summary>
    /// Compiler IAttributeFeature.
    /// </summary>
    public class AttributeFeature : BaseNode.AttributeFeature, IAttributeFeature
    {
        #region Init
        private const BaseNode.EventType UninitializedEventType = (BaseNode.EventType)(-1);

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeFeature"/> class.
        /// </summary>
        public AttributeFeature()
        {
            ResolvedEventType = UninitializedEventType;
        }
        #endregion

        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.AttributeFeature.EnsureBlocks"/>.
        /// </summary>
        public IList<IAssertion> EnsureList { get; } = new List<IAssertion>();

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
                case nameof(EnsureBlocks):
                    TargetList = (IList)EnsureList;
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Contract)
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
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                ResolvedEventType = (BaseNode.EventType)(-1);
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
                Debug.Assert(ResolvedEntityTypeName.IsAssigned == ResolvedEntityType.IsAssigned);
                Debug.Assert(ResolvedAgentTypeName.IsAssigned == ResolvedAgentType.IsAssigned);
                Debug.Assert(ResolvedEffectiveTypeName.IsAssigned == ResolvedEffectiveType.IsAssigned);

                IsResolved = ResolvedFeature.IsAssigned;

                Debug.Assert(ResolvedEntityType.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedAgentType.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedEffectiveType.IsAssigned || !IsResolved);

                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of ICompiledFeature
        /// <summary>
        /// Indicates if the feature is deferred in another class.
        /// </summary>
        public bool IsDeferredFeature { get { return false; } }

        /// <summary>
        /// True if the feature contains extern bodies in its overloads.
        /// </summary>
        public bool HasExternBody { get { return false; } }

        /// <summary>
        /// True if the feature contains precursor bodies in its overloads.
        /// </summary>
        public bool HasPrecursorBody { get { return false; } }

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

        #region Implementation of IFeatureWithEntity
        /// <summary>
        /// Guid of the language type corresponding to the entity object for an instance of this class.
        /// </summary>
        public Guid EntityGuid { get { return LanguageClasses.AttributeEntity.Guid; } }

        /// <summary>
        /// The source node associated to this instance.
        /// </summary>
        public ISource Location { get { return this; } }
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

        #region Compiler
        /// <summary>
        /// The name of the resolved attribute type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedEntityTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved attribute type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedEntityType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// The resolved event type.
        /// </summary>
        public BaseNode.EventType ResolvedEventType { get; private set; }

        /// <summary>
        /// Sets the <see cref="ResolvedEventType"/> property.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="isConflicting">True upon return if <paramref name="eventType"/> is conflicting with a previous call.</param>
        public void SetEventType(BaseNode.EventType eventType, out bool isConflicting)
        {
            if (ResolvedEventType == UninitializedEventType || ResolvedEventType == eventType)
            {
                ResolvedEventType = eventType;
                isConflicting = false;
            }
            else
                isConflicting = true;
        }
        #endregion

        #region Numbers
        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
        }
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return $"Attribute '{EntityName.Text}'";
        }
        #endregion
    }
}
