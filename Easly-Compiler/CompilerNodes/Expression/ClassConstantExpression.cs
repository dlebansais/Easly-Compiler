namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IClassConstantExpression.
    /// </summary>
    public interface IClassConstantExpression : BaseNode.IClassConstantExpression, IExpression
    {
        /// <summary>
        /// The class type name.
        /// </summary>
        OnceReference<ITypeName> ResolvedClassTypeName { get; }

        /// <summary>
        /// The class type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedClassType { get; }
    }

    /// <summary>
    /// Compiler IClassConstantExpression.
    /// </summary>
    public class ClassConstantExpression : BaseNode.ClassConstantExpression, IClassConstantExpression
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
                ResolvedExceptions = new OnceReference<IList<IIdentifier>>();
                ConstantSourceList = new ListTableEx<IExpression>();
                ExpressionConstant = new OnceReference<ILanguageConstant>();
                ResolvedClassTypeName = new OnceReference<ITypeName>();
                ResolvedClassType = new OnceReference<ICompiledType>();
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
                IsResolved = ResolvedResult.IsAssigned && ResolvedExceptions.IsAssigned;

                Debug.Assert(ExpressionConstant.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedClassTypeName.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedClassType.IsAssigned || !IsResolved);

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
        /// List of exceptions the expression can throw.
        /// </summary>
        public OnceReference<IList<IIdentifier>> ResolvedExceptions { get; private set; } = new OnceReference<IList<IIdentifier>>();

        /// <summary>
        /// The list of sources for a constant, if any.
        /// </summary>
        public ListTableEx<IExpression> ConstantSourceList { get; private set; } = new ListTableEx<IExpression>();

        /// <summary>
        /// Specific constant number.
        /// </summary>
        public OnceReference<ILanguageConstant> ExpressionConstant { get; private set; } = new OnceReference<ILanguageConstant>();

        /// <summary>
        /// Sets the <see cref="IExpression.ExpressionConstant"/> property.
        /// </summary>
        /// <param name="expressionConstant">The expression constant.</param>
        public void SetExpressionConstant(ILanguageConstant expressionConstant)
        {
            Debug.Assert(!ExpressionConstant.IsAssigned);
            Debug.Assert(expressionConstant != null);

            ExpressionConstant.Item = expressionConstant;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// The class type name.
        /// </summary>
        public OnceReference<ITypeName> ResolvedClassTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The class type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedClassType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="expression1">The first expression.</param>
        /// <param name="expression2">The second expression.</param>
        public static bool IsExpressionEqual(IClassConstantExpression expression1, IClassConstantExpression expression2)
        {
            bool Result = true;

            Result &= expression1.ClassIdentifier.Text == expression2.ClassIdentifier.Text;
            Result &= expression1.ConstantIdentifier.Text == expression2.ConstantIdentifier.Text;

            return Result;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString { get { return $"{{{ClassIdentifier.Text}}}.{ConstantIdentifier.Text}"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Class Constant Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
