namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IConditional.
    /// </summary>
    public interface IConditional : INode, ISource, IScopeHolder
    {
        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        BaseNode.Expression BooleanExpression { get; }

        /// <summary>
        /// Gets or sets instructions to execute if the condition is true.
        /// </summary>
        BaseNode.Scope Instructions { get; }

        /// <summary>
        /// Types of results of the conditional.
        /// </summary>
        OnceReference<IResultType> ResolvedResult { get; }

        /// <summary>
        /// List of exceptions the conditional can throw.
        /// </summary>
        OnceReference<IResultException> ResolvedException { get; }

        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        void RestartNumberType(ref bool isChanged);

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        void CheckNumberType(ref bool isChanged);

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        void ValidateNumberType(IErrorList errorList);
    }

    /// <summary>
    /// Compiler IConditional.
    /// </summary>
    public class Conditional : BaseNode.Conditional, IConditional
    {
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
                InnerScopes = new List<IScopeHolder>();
                FullScope = new SealableDictionary<string, IScopeAttributeFeature>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                AdditionalScope = new SealableDictionary<string, IScopeAttributeFeature>();
                AdditionalScope.Seal();
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

        #region Compiler
        /// <summary>
        /// Types of results of the conditional.
        /// </summary>
        public OnceReference<IResultType> ResolvedResult { get; private set; } = new OnceReference<IResultType>();

        /// <summary>
        /// List of exceptions the conditional can throw.
        /// </summary>
        public OnceReference<IResultException> ResolvedException { get; private set; } = new OnceReference<IResultException>();
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

        #region Numbers
        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
            ((IExpression)BooleanExpression).RestartNumberType(ref isChanged);
            ((IScope)Instructions).RestartNumberType(ref isChanged);
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            ((IExpression)BooleanExpression).CheckNumberType(ref isChanged);
            ((IScope)Instructions).CheckNumberType(ref isChanged);
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
            ((IExpression)BooleanExpression).ValidateNumberType(errorList);
            ((IScope)Instructions).ValidateNumberType(errorList);
        }
        #endregion
    }
}
