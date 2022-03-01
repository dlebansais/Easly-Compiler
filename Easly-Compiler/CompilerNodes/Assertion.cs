namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IAssertion.
    /// </summary>
    public interface IAssertion : INode, ISource, IScopeHolder
    {
        /// <summary>
        /// Gets or sets the assertion tag.
        /// </summary>
        IOptionalReference<BaseNode.Name> Tag { get; }

        /// <summary>
        /// Gets or sets the assertion expression.
        /// </summary>
        BaseNode.Expression BooleanExpression { get; }

        /// <summary>
        /// The resolved contract with the associated tag.
        /// </summary>
        OnceReference<ITaggedContract> ResolvedContract { get; }

        /// <summary>
        /// List of exceptions the assertion can throw.
        /// </summary>
        OnceReference<IResultException> ResolvedException { get; }
    }

    /// <summary>
    /// Compiler IAssertion.
    /// </summary>
    public class Assertion : BaseNode.Assertion, IAssertion
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
                AdditionalScope = new SealableDictionary<string, IScopeAttributeFeature>();
                AdditionalScope.Seal();
                InnerScopes = new List<IScopeHolder>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ResolvedContract = new OnceReference<ITaggedContract>();
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ResolvedContract.IsAssigned;
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
        public ISealableDictionary<string, IScopeAttributeFeature> LocalScope { get { return ((IScopeHolder)ParentSource).LocalScope; } }

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
        public ISealableDictionary<string, IScopeAttributeFeature> FullScope
        {
            // In the case of the full scope of an assertion, also include entities declared in the parent.
            // Typically, indexers of a IOverLoopInstruction.
            get
            {
                ISealableDictionary<string, IScopeAttributeFeature> Result = new SealableDictionary<string, IScopeAttributeFeature>();
                ISealableDictionary<string, IScopeAttributeFeature> ParentFullScope = ((IScopeHolder)ParentSource).FullScope;
                ISealableDictionary<string, IScopeAttributeFeature> ParentAdditionalScope = ((IScopeHolder)ParentSource).AdditionalScope;

                Result.Merge(ParentFullScope);
                Result.Merge(ParentAdditionalScope);

                if (ParentFullScope.IsSealed && ParentAdditionalScope.IsSealed)
                    Result.Seal();

                return Result;
            }
        }
        #endregion

        #region Compiler
        /// <summary>
        /// The resolved contract with the associated tag.
        /// </summary>
        public OnceReference<ITaggedContract> ResolvedContract { get; private set; } = new OnceReference<ITaggedContract>();

        /// <summary>
        /// List of exceptions the assertion can throw.
        /// </summary>
        public OnceReference<IResultException> ResolvedException { get; private set; } = new OnceReference<IResultException>();

        /// <summary>
        /// Checks that two lists of assertions are equal.
        /// </summary>
        /// <param name="list1">The first list.</param>
        /// <param name="list2">The second list.</param>
        public static bool IsAssertionListEqual(IList<IAssertion> list1, IList<IAssertion> list2)
        {
            bool Result = true;

            Result &= list1.Count == list2.Count;

            foreach (IAssertion Assertion1 in list1)
            {
                bool Found = false;
                foreach (IAssertion Assertion2 in list2)
                    Found |= IsAssertionEqual(Assertion1, Assertion2);

                Result &= Found;
            }

            return Result;
        }

        /// <summary>
        /// Checks that two assertions are equal.
        /// </summary>
        /// <param name="assertion1">The first assertion.</param>
        /// <param name="assertion2">The second assertion.</param>
        private static bool IsAssertionEqual(IAssertion assertion1, IAssertion assertion2)
        {
            return Expression.IsExpressionEqual((IExpression)assertion1.BooleanExpression, (IExpression)assertion2.BooleanExpression);
        }
        #endregion
    }
}
