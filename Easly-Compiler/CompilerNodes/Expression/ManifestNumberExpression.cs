namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IManifestNumberExpression.
    /// </summary>
    public interface IManifestNumberExpression : BaseNode.IManifestNumberExpression, IExpression
    {
        /// <summary>
        /// The valid value of <see cref="BaseNode.IManifestNumberExpression.Text"/>.
        /// </summary>
        OnceReference<string> ValidText { get; }
    }

    /// <summary>
    /// Compiler IManifestNumberExpression.
    /// </summary>
    public class ManifestNumberExpression : BaseNode.ManifestNumberExpression, IManifestNumberExpression
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestNumberExpression"/> class.
        /// This constructor is needed to allow deserialization of objects.
        /// </summary>
        public ManifestNumberExpression()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestNumberExpression"/> class.
        /// </summary>
        /// <param name="value">Initial value.</param>
        public ManifestNumberExpression(int value)
        {
            Documentation = BaseNodeHelper.NodeHelper.CreateEmptyDocumentation();
            Text = value.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestNumberExpression"/> class.
        /// </summary>
        /// <param name="text">Initial value.</param>
        public ManifestNumberExpression(string text)
        {
            Documentation = BaseNodeHelper.NodeHelper.CreateEmptyDocumentation();
            Text = text;
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
                ValidText = new OnceReference<string>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ResolvedResult = new OnceReference<IResultType>();
                ConstantSourceList = new ListTableEx<IExpression>();
                ExpressionConstant = new OnceReference<ILanguageConstant>();
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
                IsResolved = ValidText.IsAssigned;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ExpressionConstant.IsAssigned;

                Debug.Assert(ResolvedResult.IsAssigned || !IsResolved);

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
        /// The valid value of <see cref="BaseNode.IManifestNumberExpression.Text"/>.
        /// </summary>
        public OnceReference<string> ValidText { get; private set; } = new OnceReference<string>();

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="expression1">The first expression.</param>
        /// <param name="expression2">The second expression.</param>
        public static bool IsExpressionEqual(IManifestNumberExpression expression1, IManifestNumberExpression expression2)
        {
            bool Result = true;

            Result &= expression1.ValidText.Item == expression2.ValidText.Item;

            return Result;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IManifestNumberExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The constant type upon return.</param>
        public static bool ResolveCompilerReferences(IManifestNumberExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;

            IClass EmbeddingClass = node.EmbeddingClass;
            string NumberText = node.ValidText.Item;
            IHashtableEx<ITypeName, ICompiledType> TypeTable = EmbeddingClass.TypeTable;

            if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, out ITypeName NumberTypeName, out ICompiledType NumberType))
            {
                errorList.AddError(new ErrorNumberTypeMissing(node));
                return false;
            }

            resolvedResult = new ResultType(NumberTypeName, NumberType, string.Empty);
            resolvedException = new ResultException();

            BaseNodeHelper.IFormattedNumber FormattedNumber = BaseNodeHelper.FormattedNumber.Parse(NumberText, false);
            Debug.Assert(string.IsNullOrEmpty(FormattedNumber.InvalidText));

            expressionConstant = new NumberLanguageConstant(FormattedNumber.Canonical);

            return true;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString { get { return Text; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Manifest Number '{ExpressionToString}'";
        }
        #endregion
    }
}
