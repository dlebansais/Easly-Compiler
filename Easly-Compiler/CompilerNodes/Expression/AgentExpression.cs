namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IAgentExpression.
    /// </summary>
    public interface IAgentExpression : BaseNode.IAgentExpression, IExpression
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
        public void Reset(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ResolvedResult = new OnceReference<IList<IExpressionType>>();
                NumberConstant = new OnceReference<ILanguageConstant>();
                ResolvedExceptions = new OnceReference<IList<IIdentifier>>();
                ResolvedAncestorTypeName = new OnceReference<ITypeName>();
                ResolvedAncestorType = new OnceReference<ICompiledType>();
                ResolvedFeature = new OnceReference<ICompiledFeature>();
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
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ResolvedResult.IsAssigned && NumberConstant.IsAssigned && ResolvedExceptions.IsAssigned;
                Debug.Assert(ResolvedAncestorTypeName.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedAncestorType.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedFeature.IsAssigned || !IsResolved);
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
        public OnceReference<IList<IExpressionType>> ResolvedResult { get; private set; } = new OnceReference<IList<IExpressionType>>();

        /// <summary>
        /// True if the expression is a constant.
        /// </summary>
        public bool IsConstant { get { return true; } }

        /// <summary>
        /// Specific constant number.
        /// </summary>
        public OnceReference<ILanguageConstant> NumberConstant { get; private set; } = new OnceReference<ILanguageConstant>();

        /// <summary>
        /// List of exceptions the expression can throw.
        /// </summary>
        public OnceReference<IList<IIdentifier>> ResolvedExceptions { get; private set; } = new OnceReference<IList<IIdentifier>>();
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
        /// <param name="expression1">The first expression.</param>
        /// <param name="expression2">The second expression.</param>
        public static bool IsExpressionEqual(IAgentExpression expression1, IAgentExpression expression2)
        {
            bool Result = true;

            Result &= expression1.Delegated.Text == expression2.Delegated.Text;

            return Result;
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
