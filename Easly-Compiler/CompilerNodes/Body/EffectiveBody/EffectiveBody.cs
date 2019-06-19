namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IEffectiveBody.
    /// </summary>
    public interface IEffectiveBody : BaseNode.IEffectiveBody, IBody, ICompiledBody, IScopeHolder
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.EffectiveBody.EntityDeclarationBlocks"/>.
        /// </summary>
        IList<IEntityDeclaration> EntityDeclarationList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.EffectiveBody.BodyInstructionBlocks"/>.
        /// </summary>
        IList<IInstruction> BodyInstructionList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.EffectiveBody.ExceptionHandlerBlocks"/>.
        /// </summary>
        IList<IExceptionHandler> ExceptionHandlerList { get; }
    }

    /// <summary>
    /// Compiler IEffectiveBody.
    /// </summary>
    public class EffectiveBody : BaseNode.EffectiveBody, IEffectiveBody
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
        /// Replicated list from <see cref="BaseNode.EffectiveBody.EntityDeclarationBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> EntityDeclarationList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.EffectiveBody.BodyInstructionBlocks"/>.
        /// </summary>
        public IList<IInstruction> BodyInstructionList { get; } = new List<IInstruction>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.EffectiveBody.ExceptionHandlerBlocks"/>.
        /// </summary>
        public IList<IExceptionHandler> ExceptionHandlerList { get; } = new List<IExceptionHandler>();

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

                case nameof(EntityDeclarationBlocks):
                    TargetList = (IList)EntityDeclarationList;
                    break;

                case nameof(BodyInstructionBlocks):
                    TargetList = (IList)BodyInstructionList;
                    break;

                case nameof(ExceptionHandlerBlocks):
                    TargetList = (IList)ExceptionHandlerList;
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                LocalScope = new SealableDictionary<string, IScopeAttributeFeature>();
                AdditionalScope = new SealableDictionary<string, IScopeAttributeFeature>();
                AdditionalScope.Seal();
                InnerScopes = new List<IScopeHolder>();
                FullScope = new SealableDictionary<string, IScopeAttributeFeature>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                if (!FullScope.IsSealed) FullScope.Seal();
                ResolvedTagTable = new SealableDictionary<string, IExpression>();
                ResolvedResult = new OnceReference<IResultType>();
                ResolvedRequireList = new OnceReference<IList<IAssertion>>();
                ResolvedEnsureList = new OnceReference<IList<IAssertion>>();
                ResolvedExceptionIdentifierList = new OnceReference<IList<IIdentifier>>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                ResolvedInstructionList = new OnceReference<IList<IInstruction>>();
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = LocalScope.IsSealed;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ResolvedResult.IsAssigned && ResolvedRequireList.IsAssigned && ResolvedEnsureList.IsAssigned && ResolvedExceptionIdentifierList.IsAssigned;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = ResolvedInstructionList.IsAssigned;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IBody
        /// <summary>
        /// Tags for tag expressions.
        /// </summary>
        public ISealableDictionary<string, IExpression> ResolvedTagTable { get; private set; } = new SealableDictionary<string, IExpression>();

        /// <summary>
        /// Types of results.
        /// </summary>
        public OnceReference<IResultType> ResolvedResult { get; private set; } = new OnceReference<IResultType>();

        /// <summary>
        /// Resolved list of require assertions.
        /// </summary>
        public OnceReference<IList<IAssertion>> ResolvedRequireList { get; private set; } = new OnceReference<IList<IAssertion>>();

        /// <summary>
        /// Resolved list of ensure assertions.
        /// </summary>
        public OnceReference<IList<IAssertion>> ResolvedEnsureList { get; private set; } = new OnceReference<IList<IAssertion>>();

        /// <summary>
        /// Resolved list of exceptions the body can throw.
        /// </summary>
        public OnceReference<IList<IIdentifier>> ResolvedExceptionIdentifierList { get; private set; } = new OnceReference<IList<IIdentifier>>();

        /// <summary>
        /// Resolved list of instructions in the body.
        /// </summary>
        public OnceReference<IList<IInstruction>> ResolvedInstructionList { get; private set; } = new OnceReference<IList<IInstruction>>();
        #endregion

        #region Implementation of ICompiledBody
        /// <summary>
        /// Indicates if the body is deferred in another class.
        /// </summary>
        public bool IsDeferredBody { get { return false; } }
        #endregion

        #region Implementation of IScopeHolder
        /// <summary>
        /// Entities local to a scope.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> LocalScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// Additional entities such as loop indexer.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> AdditionalScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// List of scopes containing the current instance.
        /// </summary>
        public IList<IScopeHolder> InnerScopes { get; private set; } = new List<IScopeHolder>();

        /// <summary>
        /// All reachable entities.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> FullScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();
        #endregion
    }
}
