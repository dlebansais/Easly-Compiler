namespace CompilerNode
{
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IBinaryConditionalExpression.
    /// </summary>
    public interface IBinaryConditionalExpression : BaseNode.IBinaryConditionalExpression, IExpression, IComparableExpression
    {
        /// <summary>
        /// True if the condition is on events.
        /// </summary>
        bool IsEventExpression { get; }

        /// <summary>
        /// Sets the <see cref="IsEventExpression"/> property.
        /// </summary>
        void SetIsEventExpression(bool isEventExpression);
    }

    /// <summary>
    /// Compiler IBinaryConditionalExpression.
    /// </summary>
    public class BinaryConditionalExpression : BaseNode.BinaryConditionalExpression, IBinaryConditionalExpression
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
                ConstantSourceList = new SealableList<IExpression>();
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
        public SealableList<IExpression> ConstantSourceList { get; private set; } = new SealableList<IExpression>();

        /// <summary>
        /// Specific constant number.
        /// </summary>
        public OnceReference<ILanguageConstant> ExpressionConstant { get; private set; } = new OnceReference<ILanguageConstant>();
        #endregion

        #region Compiler
        /// <summary>
        /// True if the condition is on events.
        /// </summary>
        public bool IsEventExpression { get; private set; }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        public bool IsExpressionEqual(IComparableExpression other)
        {
            return IsExpressionEqual(other as IBinaryConditionalExpression);
        }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        protected bool IsExpressionEqual(IBinaryConditionalExpression other)
        {
            Debug.Assert(other != null);

            bool Result = true;

            Result &= Expression.IsExpressionEqual((IExpression)LeftExpression, (IExpression)other.LeftExpression);
            Result &= Conditional == other.Conditional;
            Result &= Expression.IsExpressionEqual((IExpression)RightExpression, (IExpression)other.RightExpression);

            return Result;
        }

        /// <summary>
        /// Sets the <see cref="IsEventExpression"/> property.
        /// </summary>
        public void SetIsEventExpression(bool isEventExpression)
        {
            IsEventExpression = isEventExpression;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IBinaryConditionalExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The constant value upon return, if any.</param>
        public static bool ResolveCompilerReferences(IBinaryConditionalExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out SealableList<IExpression> constantSourceList, out ILanguageConstant expressionConstant)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new SealableList<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;

            IExpression LeftExpression = (IExpression)node.LeftExpression;
            BaseNode.ConditionalTypes Conditional = node.Conditional;
            IExpression RightExpression = (IExpression)node.RightExpression;
            IClass EmbeddingClass = node.EmbeddingClass;

            bool IsLeftClassType = Expression.GetClassTypeOfExpression(LeftExpression, errorList, out IClassType LeftExpressionClassType);
            bool IsRightClassType = Expression.GetClassTypeOfExpression(RightExpression, errorList, out IClassType RightExpressionClassType);
            if (!IsLeftClassType || !IsRightClassType)
                return false;

            Expression.IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, node, out ITypeName BooleanTypeName, out ICompiledType BooleanType);
            Expression.IsLanguageTypeAvailable(LanguageClasses.Event.Guid, node, out ITypeName EventTypeName, out ICompiledType EventType);

            Debug.Assert(LeftExpressionClassType == BooleanType || LeftExpressionClassType == EventType);
            Debug.Assert(RightExpressionClassType == BooleanType || RightExpressionClassType == EventType);

            if (LeftExpressionClassType != RightExpressionClassType)
            {
                errorList.AddError(new ErrorInvalidExpression(LeftExpression));
                return false;
            }

            node.SetIsEventExpression(LeftExpressionClassType != BooleanType);

            constantSourceList.Add(LeftExpression);
            constantSourceList.Add(RightExpression);

            resolvedResult = new ResultType(BooleanTypeName, BooleanType, string.Empty);

            resolvedException = new ResultException();
            ResultException.Merge(resolvedException, LeftExpression.ResolvedException);
            ResultException.Merge(resolvedException, RightExpression.ResolvedException);

            return true;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString { get { return $"({((IExpression)LeftExpression).ExpressionToString}) {Conditional.ToString().ToLower()} ({((IExpression)RightExpression).ExpressionToString})"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Binary Conditional Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
