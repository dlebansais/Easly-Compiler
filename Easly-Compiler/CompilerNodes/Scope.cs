namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IScope.
    /// </summary>
    public interface IScope : BaseNode.IScope, INode, INodeWithReplicatedBlocks, ISource, IScopeHolder
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Scope.EntityDeclarationBlocks"/>.
        /// </summary>
        IList<IEntityDeclaration> EntityDeclarationList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Scope.InstructionBlocks"/>.
        /// </summary>
        IList<IInstruction> InstructionList { get; }

        /// <summary>
        /// Types of results of the scope.
        /// </summary>
        OnceReference<IResultType> ResolvedResult { get; }

        /// <summary>
        /// List of exceptions the scope can throw.
        /// </summary>
        OnceReference<IResultException> ResolvedException { get; }
    }

    /// <summary>
    /// Compiler IScope.
    /// </summary>
    public class Scope : BaseNode.Scope, IScope
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Scope.EntityDeclarationBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> EntityDeclarationList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Scope.InstructionBlocks"/>.
        /// </summary>
        public IList<IInstruction> InstructionList { get; } = new List<IInstruction>();

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
                case nameof(EntityDeclarationBlocks):
                    TargetList = (IList)EntityDeclarationList;
                    break;

                case nameof(InstructionBlocks):
                    TargetList = (IList)InstructionList;
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
        public void Reset(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                LocalScope = new HashtableEx<string, IScopeAttributeFeature>();
                InnerScopes = new List<IScopeHolder>();
                FullScope = new HashtableEx<string, IScopeAttributeFeature>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ResolvedResult = new OnceReference<IResultType>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                ResolvedException = new OnceReference<IResultException>();
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
                IsResolved = LocalScope.IsSealed;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ResolvedResult.IsAssigned;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = ResolvedException.IsAssigned;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IScopeHolder
        /// <summary>
        /// Entities local to a scope.
        /// </summary>
        public IHashtableEx<string, IScopeAttributeFeature> LocalScope { get; private set; } = new HashtableEx<string, IScopeAttributeFeature>();

        /// <summary>
        /// List of scopes containing the current instance.
        /// </summary>
        public IList<IScopeHolder> InnerScopes { get; private set; } = new List<IScopeHolder>();

        /// <summary>
        /// All reachable entities.
        /// </summary>
        public IHashtableEx<string, IScopeAttributeFeature> FullScope { get; private set; } = new HashtableEx<string, IScopeAttributeFeature>();
        #endregion

        #region Compiler
        /// <summary>
        /// Types of results of the scope.
        /// </summary>
        public OnceReference<IResultType> ResolvedResult { get; private set; } = new OnceReference<IResultType>();

        /// <summary>
        /// List of exceptions the scope can throw.
        /// </summary>
        public OnceReference<IResultException> ResolvedException { get; private set; } = new OnceReference<IResultException>();

        /// <summary>
        /// Gets the scope associated to a node.
        /// </summary>
        /// <param name="source">The node with a scope</param>
        public static IHashtableEx<string, IScopeAttributeFeature> CurrentScope(ISource source)
        {
            ISource ChildSource = source;
            ISource ParentSource = source.ParentSource;

            IHashtableEx<string, IScopeAttributeFeature> Result = null;
            bool IsHandled;

            do
            {
                IsHandled = false;

                switch (ParentSource)
                {
                    case IScope AsScope:
                        Result = AsScope.FullScope;
                        IsHandled = true;
                        break;

                    case IClass AsClass:
                        Result = AsClass.FullScope;
                        IsHandled = true;
                        break;

                    /* Can't possibly be in a continuation, it's either in the continuation scope or in one of the cleanup instructions.
                    case IContinuation AsContinuation:
                        Result = AsContinuation.FullScope;
                        IsHandled = true;
                        break;
                        */

                    case IConditional AsConditional:
                        Result = AsConditional.FullScope;
                        IsHandled = true;
                        break;

                    /* Can't possibly be in an attachment, it would be in the attachment scope.
                    case IAttachment AsAttachment:
                        Result = AsAttachment.FullScope;
                        IsHandled = true;
                        break;
                        */

                    case IInstruction AsInstruction:
                        Result = AsInstruction.FullScope;
                        IsHandled = true;
                        break;

                    case IEffectiveBody AsEffectiveBody:
                        Result = AsEffectiveBody.FullScope;
                        IsHandled = true;
                        break;

                    case ICommandOverload AsCommandOverload:
                        Result = AsCommandOverload.FullScope;
                        IsHandled = true;
                        break;

                    case IQueryOverload AsQueryOverload:
                        Result = AsQueryOverload.FullScope;
                        IsHandled = true;
                        break;

                    case IPropertyFeature AsPropertyFeature:
                        if (AsPropertyFeature.GetterBody.IsAssigned && AsPropertyFeature.GetterBody.Item == ChildSource)
                        {
                            Result = AsPropertyFeature.FullGetScope;
                            IsHandled = true;
                        }
                        else if (AsPropertyFeature.SetterBody.IsAssigned && AsPropertyFeature.SetterBody.Item == ChildSource)
                        {
                            Result = AsPropertyFeature.FullSetScope;
                            IsHandled = true;
                        }
                        break;

                    case IIndexerFeature AsIndexerFeature:
                        if (AsIndexerFeature.GetterBody.IsAssigned && AsIndexerFeature.GetterBody.Item == ChildSource)
                        {
                            Result = AsIndexerFeature.FullGetScope;
                            IsHandled = true;
                        }
                        else if (AsIndexerFeature.SetterBody.IsAssigned && AsIndexerFeature.SetterBody.Item == ChildSource)
                        {
                            Result = AsIndexerFeature.FullSetScope;
                            IsHandled = true;
                        }
                        break;

                    default:
                        ChildSource = ParentSource;
                        ParentSource = ChildSource.ParentSource;
                        IsHandled = ParentSource != null;
                        break;
                }

                Debug.Assert(IsHandled);
            }
            while (Result == null);

            Debug.Assert(Result != null);

            return Result;
        }
        #endregion
    }
}
