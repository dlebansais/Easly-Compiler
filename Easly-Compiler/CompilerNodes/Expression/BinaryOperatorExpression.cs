namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IBinaryOperatorExpression.
    /// </summary>
    public interface IBinaryOperatorExpression : BaseNode.IBinaryOperatorExpression, IExpression
    {
        /// <summary>
        /// The resolved operator feature.
        /// </summary>
        OnceReference<IFunctionFeature> SelectedFeature { get; }

        /// <summary>
        /// The resolved operator feature overload.
        /// </summary>
        OnceReference<IQueryOverload> SelectedOverload { get; }

        /// <summary>
        /// Sets the <see cref="IExpression.IsConstant"/> property.
        /// </summary>
        /// <param name="isConstant">The property value.</param>
        void SetIsConstant(bool isConstant);
    }

    /// <summary>
    /// Compiler IBinaryOperatorExpression.
    /// </summary>
    public class BinaryOperatorExpression : BaseNode.BinaryOperatorExpression, IBinaryOperatorExpression
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
                SelectedFeature = new OnceReference<IFunctionFeature>();
                SelectedOverload = new OnceReference<IQueryOverload>();
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
                Debug.Assert(SelectedFeature.IsAssigned == ResolvedResult.IsAssigned);
                Debug.Assert(SelectedOverload.IsAssigned == ResolvedResult.IsAssigned);

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
        public bool IsConstant { get; private set; }

        /// <summary>
        /// Specific constant number.
        /// </summary>
        public OnceReference<ILanguageConstant> NumberConstant { get; private set; } = new OnceReference<ILanguageConstant>();

        /// <summary>
        /// List of exceptions the expression can throw.
        /// </summary>
        public OnceReference<IList<IIdentifier>> ResolvedExceptions { get; private set; } = new OnceReference<IList<IIdentifier>>();

        /// <summary>
        /// Sets the <see cref="IsConstant"/> property.
        /// </summary>
        /// <param name="isConstant">The property value.</param>
        public void SetIsConstant(bool isConstant)
        {
            IsConstant = isConstant;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// The resolved operator feature.
        /// </summary>
        public OnceReference<IFunctionFeature> SelectedFeature { get; private set; } = new OnceReference<IFunctionFeature>();

        /// <summary>
        /// The resolved operator feature overload.
        /// </summary>
        public OnceReference<IQueryOverload> SelectedOverload { get; private set; } = new OnceReference<IQueryOverload>();

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="expression1">The first expression.</param>
        /// <param name="expression2">The second expression.</param>
        public static bool IsExpressionEqual(IBinaryOperatorExpression expression1, IBinaryOperatorExpression expression2)
        {
            bool Result = true;

            Result &= Expression.IsExpressionEqual((IExpression)expression1.LeftExpression, (IExpression)expression2.LeftExpression);
            Result &= expression1.Operator.Text == expression2.Operator.Text;
            Result &= Expression.IsExpressionEqual((IExpression)expression1.RightExpression, (IExpression)expression2.RightExpression);

            return Result;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString { get { return $"({((IExpression)LeftExpression).ExpressionToString}) {Operator.Text} ({((IExpression)RightExpression).ExpressionToString})"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Binary Operator Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
