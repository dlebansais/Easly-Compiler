namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IUnaryOperatorExpression.
    /// </summary>
    public interface IUnaryOperatorExpression : BaseNode.IUnaryOperatorExpression, IExpression
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
        /// Type of the resolved operator feature overload.
        /// </summary>
        OnceReference<IQueryOverloadType> SelectedOverloadType { get; }
    }

    /// <summary>
    /// Compiler IUnaryOperatorExpression.
    /// </summary>
    public class UnaryOperatorExpression : BaseNode.UnaryOperatorExpression, IUnaryOperatorExpression
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
                SelectedOverloadType = new OnceReference<IQueryOverloadType>();
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
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ExpressionConstant.IsAssigned;

                Debug.Assert(ResolvedResult.IsAssigned || !IsResolved);
                Debug.Assert(SelectedFeature.IsAssigned == ResolvedResult.IsAssigned);
                Debug.Assert(SelectedOverload.IsAssigned == ResolvedResult.IsAssigned);
                Debug.Assert(SelectedOverloadType.IsAssigned == ResolvedResult.IsAssigned);

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
        /// Type of the resolved operator feature overload.
        /// </summary>
        public OnceReference<IQueryOverloadType> SelectedOverloadType { get; private set; } = new OnceReference<IQueryOverloadType>();

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="expression1">The first expression.</param>
        /// <param name="expression2">The second expression.</param>
        public static bool IsExpressionEqual(IUnaryOperatorExpression expression1, IUnaryOperatorExpression expression2)
        {
            bool Result = true;

            Result &= Expression.IsExpressionEqual((IExpression)expression1.RightExpression, (IExpression)expression2.RightExpression);
            Result &= expression1.Operator.Text == expression2.Operator.Text;

            return Result;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IUnaryOperatorExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The constant value upon return, if any.</param>
        /// <param name="selectedFeature">The matching feature upon return.</param>
        /// <param name="selectedOverload">The matching overload in <paramref name="selectedFeature"/> upon return.</param>
        /// <param name="selectedOverloadType">The matching overload type upon return.</param>
        public static bool ResolveCompilerReferences(IUnaryOperatorExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out IFunctionFeature selectedFeature, out IQueryOverload selectedOverload, out IQueryOverloadType selectedOverloadType)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            selectedFeature = null;
            selectedOverload = null;
            selectedOverloadType = null;

            IIdentifier Operator = (IIdentifier)node.Operator;
            string ValidText = Operator.ValidText.Item;
            IExpression RightExpression = (IExpression)node.RightExpression;
            IResultType RightResult = RightExpression.ResolvedResult.Item;

            if (!RightResult.TryGetResult(out ICompiledType RightExpressionType))
            {
                errorList.AddError(new ErrorInvalidExpression(RightExpression));
                return false;
            }

            if (RightExpressionType is IClassType AsClassType)
            {
                IClass RightBaseClass = AsClassType.BaseClass;
                IHashtableEx<IFeatureName, IFeatureInstance> RightFeatureTable = RightBaseClass.FeatureTable;

                if (!FeatureName.TableContain(RightFeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Value))
                {
                    errorList.AddError(new ErrorUnknownIdentifier(RightExpression, ValidText));
                    return false;
                }

                ICompiledFeature OperatorFeature = Value.Feature.Item;
                ICompiledType OperatorType = OperatorFeature.ResolvedFeatureType.Item;

                if (OperatorFeature is IFunctionFeature AsFunctionFeature && OperatorType is IFunctionType AsFunctionType)
                {
                    IList<IQueryOverloadType> OperatorOverloadList = AsFunctionType.OverloadList;

                    int SelectedOperatorIndex = -1;
                    for (int i = 0; i < OperatorOverloadList.Count; i++)
                    {
                        IQueryOverloadType Overload = OperatorOverloadList[i];
                        if (Overload.ParameterList.Count == 0 && Overload.ResultList.Count == 1)
                        {
                            SelectedOperatorIndex = i;
                            break;
                        }
                    }

                    if (SelectedOperatorIndex < 0)
                    {
                        errorList.AddError(new ErrorInvalidOperator(Operator, ValidText));
                        return false;
                    }

                    resolvedResult = Feature.CommonResultType(AsFunctionType.OverloadList);
                    selectedFeature = AsFunctionFeature;
                    selectedOverload = AsFunctionFeature.OverloadList[SelectedOperatorIndex];
                    selectedOverloadType = OperatorOverloadList[SelectedOperatorIndex];
                    resolvedException = new ResultException(selectedOverloadType.ExceptionIdentifierList);

                    constantSourceList.Add(RightExpression);
                }
                else
                {
                    errorList.AddError(new ErrorInvalidOperator(Operator, ValidText));
                    return false;
                }
            }
            else
            {
                errorList.AddError(new ErrorInvalidExpression(RightExpression));
                return false;
            }

            return true;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString { get { return $"{Operator.Text}({((IExpression)RightExpression).ExpressionToString})"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Unary Operator Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
