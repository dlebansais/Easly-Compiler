namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IAgentExpression.
    /// </summary>
    public interface IAgentExpression : BaseNode.IAgentExpression, IExpression, IComparableExpression
    {
        /// <summary>
        /// The resolved type name of the feature providing the expression result.
        /// </summary>
        OnceReference<ITypeName> ResolvedAncestorTypeName { get; }

        /// <summary>
        /// The resolved type of the feature providing the expression result.
        /// </summary>
        OnceReference<ICompiledType> ResolvedAncestorType { get; }

        /// <summary>
        /// The resolved feature providing the expression result.
        /// </summary>
        OnceReference<ICompiledFeature> ResolvedFeature { get; }
    }

    /// <summary>
    /// Compiler IAgentExpression.
    /// </summary>
    public class AgentExpression : BaseNode.AgentExpression, IAgentExpression
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Types)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ResolvedResult = new OnceReference<IResultType>();
                ConstantSourceList = new ListTableEx<IExpression>();
                ExpressionConstant = new OnceReference<ILanguageConstant>();
                ResolvedAncestorTypeName = new OnceReference<ITypeName>();
                ResolvedAncestorType = new OnceReference<ICompiledType>();
                ResolvedFeature = new OnceReference<ICompiledFeature>();
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
                IsResolved = ExpressionConstant.IsAssigned;

                Debug.Assert(ResolvedResult.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedAncestorTypeName.IsAssigned == ResolvedResult.IsAssigned);
                Debug.Assert(ResolvedAncestorType.IsAssigned == ResolvedResult.IsAssigned);
                Debug.Assert(ResolvedFeature.IsAssigned == ResolvedResult.IsAssigned);

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

        #region Implementation of IExpression
        /// <summary>
        /// Types of expression results.
        /// </summary>
        public OnceReference<IResultType> ResolvedResult { get; private set; } = new OnceReference<IResultType>();

        /// <summary>
        /// List of exceptions the expression can throw.
        /// </summary>
        public OnceReference<IResultException> ResolvedException { get; private set; } = new OnceReference<IResultException>();

        /// <summary>
        /// The list of sources for a constant, if any.
        /// </summary>
        public ListTableEx<IExpression> ConstantSourceList { get; private set; } = new ListTableEx<IExpression>();

        /// <summary>
        /// Specific constant number.
        /// </summary>
        public OnceReference<ILanguageConstant> ExpressionConstant { get; private set; } = new OnceReference<ILanguageConstant>();
        #endregion

        #region Compiler
        /// <summary>
        /// The resolved type name of the feature providing the expression result.
        /// </summary>
        public OnceReference<ITypeName> ResolvedAncestorTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved type of the feature providing the expression result.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedAncestorType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// The resolved feature providing the expression result.
        /// </summary>
        public OnceReference<ICompiledFeature> ResolvedFeature { get; private set; } = new OnceReference<ICompiledFeature>();

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        public bool IsExpressionEqual(IComparableExpression other)
        {
            return IsExpressionEqual(other as IAgentExpression);
        }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        protected bool IsExpressionEqual(IAgentExpression other)
        {
            Debug.Assert(other != null);

            bool Result = true;

            Result &= Delegated.Text == other.Delegated.Text;

            return Result;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IAgentExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The constant value upon return, if any.</param>
        /// <param name="resolvedFeature">The feature found upon return.</param>
        public static bool ResolveCompilerReferences(IAgentExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ICompiledFeature resolvedFeature)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            resolvedFeature = null;

            IIdentifier Delegated = (IIdentifier)node.Delegated;

            IClass EmbeddingClass = node.EmbeddingClass;
            IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable = EmbeddingClass.FeatureTable;
            Debug.Assert(Delegated.ValidText.IsAssigned);
            string ValidText = Delegated.ValidText.Item;

            if (!FeatureName.TableContain(FeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Value))
            {
                errorList.AddError(new ErrorUnknownIdentifier(node, ValidText));
                return false;
            }

            Debug.Assert(Value.Feature != null);
            resolvedFeature = Value.Feature;

            resolvedResult = new ResultType(resolvedFeature.ResolvedFeatureTypeName.Item, resolvedFeature.ResolvedFeatureType.Item, string.Empty);

            resolvedException = new ResultException();
            expressionConstant = new AgentLanguageConstant(resolvedFeature);

            return true;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString
        {
            get
            {
                string BaseTypeString = BaseType.IsAssigned ? $"{{{((IObjectType)BaseType.Item).TypeToString}}} " : string.Empty;
                return $"agent {BaseTypeString}{Delegated.Text}";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Agent Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
