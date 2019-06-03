namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IBinaryOperatorExpression.
    /// </summary>
    public interface IBinaryOperatorExpression : BaseNode.IBinaryOperatorExpression, IExpression, IComparableExpression
    {
        /// <summary>
        /// The resolved operator feature.
        /// </summary>
        OnceReference<IFunctionFeature> SelectedFeature { get; }

        /// <summary>
        /// The resolved operator feature overload.
        /// </summary>
        OnceReference<IQueryOverload> SelectedOverload { get; }
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
                SelectedFeature = new OnceReference<IFunctionFeature>();
                SelectedOverload = new OnceReference<IQueryOverload>();
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
                Debug.Assert(SelectedFeature.IsAssigned == ResolvedResult.IsAssigned);
                Debug.Assert(SelectedOverload.IsAssigned == ResolvedResult.IsAssigned);

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
        /// <param name="other">The other expression.</param>
        public bool IsExpressionEqual(IComparableExpression other)
        {
            return IsExpressionEqual(other as IBinaryOperatorExpression);
        }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        protected bool IsExpressionEqual(IBinaryOperatorExpression other)
        {
            Debug.Assert(other != null);

            bool Result = true;

            Result &= Expression.IsExpressionEqual((IExpression)LeftExpression, (IExpression)other.LeftExpression);
            Result &= Operator.Text == other.Operator.Text;
            Result &= Expression.IsExpressionEqual((IExpression)RightExpression, (IExpression)other.RightExpression);

            return Result;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IBinaryOperatorExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The constant value upon return, if any.</param>
        /// <param name="selectedFeature">The matching feature upon return.</param>
        /// <param name="selectedOverload">The matching overload in <paramref name="selectedFeature"/> upon return.</param>
        public static bool ResolveCompilerReferences(IBinaryOperatorExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out IFunctionFeature selectedFeature, out IQueryOverload selectedOverload)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            selectedFeature = null;
            selectedOverload = null;

            IExpression LeftExpression = (IExpression)node.LeftExpression;
            IIdentifier Operator = (IIdentifier)node.Operator;
            IExpression RightExpression = (IExpression)node.RightExpression;
            IResultType LeftResult = LeftExpression.ResolvedResult.Item;

            if (LeftResult.TryGetResult(out ICompiledType LeftExpressionType))
            {
                if (LeftExpressionType is IClassType AsClassType)
                {
                    string OperatorName = Operator.ValidText.Item;

                    IClass LeftBaseClass = AsClassType.BaseClass;
                    IHashtableEx<IFeatureName, IFeatureInstance> LeftFeatureTable = LeftBaseClass.FeatureTable;

                    if (!FeatureName.TableContain(LeftFeatureTable, OperatorName, out IFeatureName Key, out IFeatureInstance Value))
                    {
                        errorList.AddError(new ErrorUnknownIdentifier(Operator, OperatorName));
                        return false;
                    }

                    Debug.Assert(Value.Feature != null);

                    ICompiledFeature OperatorFeature = Value.Feature;
                    Debug.Assert(OperatorFeature.ResolvedFeatureType.IsAssigned);
                    ICompiledType OperatorType = OperatorFeature.ResolvedFeatureType.Item;

                    if (OperatorType is FunctionType AsFunctionType && OperatorFeature is IFunctionFeature AsFunctionFeature)
                    {
                        IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();
                        foreach (IQueryOverloadType Overload in AsFunctionType.OverloadList)
                            ParameterTableList.Add(Overload.ParameterTable);

                        IResultType RightResult = RightExpression.ResolvedResult.Item;
                        if (!Argument.ArgumentsConformToParameters(ParameterTableList, RightResult.ToList(), TypeArgumentStyles.Positional, errorList, Operator, out int SelectedIndex))
                            return false;

                        IQueryOverloadType SelectedOverloadType = AsFunctionType.OverloadList[SelectedIndex];
                        resolvedResult = new ResultType(SelectedOverloadType.ResultTypeList);
                        selectedFeature = AsFunctionFeature;
                        selectedOverload = AsFunctionFeature.OverloadList[SelectedIndex];
                        resolvedException = new ResultException(SelectedOverloadType.ExceptionIdentifierList);

                        constantSourceList.Add(LeftExpression);
                        constantSourceList.Add(RightExpression);
                    }
                    else
                    {
                        errorList.AddError(new ErrorInvalidOperator(Operator, OperatorName));
                        return false;
                    }
                }
                else
                {
                    errorList.AddError(new ErrorInvalidExpression(LeftExpression));
                    return false;
                }
            }
            else
            {
                errorList.AddError(new ErrorInvalidExpression(LeftExpression));
                return false;
            }

            return true;
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
