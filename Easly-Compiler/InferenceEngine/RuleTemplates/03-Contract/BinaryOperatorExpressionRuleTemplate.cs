namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IBinaryOperatorExpression"/>.
    /// </summary>
    public interface IBinaryOperatorExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBinaryOperatorExpression"/>.
    /// </summary>
    public class BinaryOperatorExpressionRuleTemplate : RuleTemplate<IBinaryOperatorExpression, BinaryOperatorExpressionRuleTemplate>, IBinaryOperatorExpressionRuleTemplate
    {
        #region Init
        static BinaryOperatorExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IBinaryOperatorExpression, IList<IExpressionType>>(nameof(IBinaryOperatorExpression.LeftExpression) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceSourceTemplate<IBinaryOperatorExpression, IList<IIdentifier>>(nameof(IBinaryOperatorExpression.LeftExpression) + Dot + nameof(IExpression.ResolvedExceptions)),
                new OnceReferenceSourceTemplate<IBinaryOperatorExpression, IList<IExpressionType>>(nameof(IBinaryOperatorExpression.RightExpression) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceSourceTemplate<IBinaryOperatorExpression, IList<IIdentifier>>(nameof(IBinaryOperatorExpression.RightExpression) + Dot + nameof(IExpression.ResolvedExceptions)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IBinaryOperatorExpression, IList<IExpressionType>>(nameof(IBinaryOperatorExpression.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IBinaryOperatorExpression, IList<IIdentifier>>(nameof(IBinaryOperatorExpression.ResolvedExceptions)),
                new UnsealedListDestinationTemplate<IBinaryOperatorExpression, IExpression>(nameof(IBinaryOperatorExpression.ConstantSourceList)),
            };
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        /// <param name="dataList">Optional data collected during inspection of sources.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        public override bool CheckConsistency(IBinaryOperatorExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= BinaryOperatorExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out IFunctionFeature SelectedFeature, out IQueryOverload SelectedOverload);
            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant, SelectedFeature, SelectedOverload);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IBinaryOperatorExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The constant value upon return, if any.</param>
        /// <param name="selectedFeature">The matching feature upon return.</param>
        /// <param name="selectedOverload">The matching overload in <paramref name="selectedFeature"/> upon return.</param>
        public static bool ResolveCompilerReferences(IBinaryOperatorExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out IFunctionFeature selectedFeature, out IQueryOverload selectedOverload)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            selectedFeature = null;
            selectedOverload = null;

            IExpression LeftExpression = (IExpression)node.LeftExpression;
            IIdentifier Operator = (IIdentifier)node.Operator;
            IExpression RightExpression = (IExpression)node.RightExpression;
            OnceReference<ICompiledType> LeftExpressionType = new OnceReference<ICompiledType>();

            IList<IExpressionType> LeftResult = LeftExpression.ResolvedResult.Item;
            if (LeftResult.Count == 1)
                LeftExpressionType.Item = LeftResult[0].ValueType;
            else
                foreach (ExpressionType Item in LeftResult)
                    if (Item.Name == nameof(BaseNode.Keyword.Result))
                    {
                        LeftExpressionType.Item = Item.ValueType;
                        break;
                    }

            if (LeftExpressionType.IsAssigned)
            {
                if (LeftExpressionType.Item is IClassType AsClassType)
                {
                    string OperatorName = Operator.ValidText.Item;

                    IClass LeftBaseClass = AsClassType.BaseClass;
                    IHashtableEx<IFeatureName, IFeatureInstance> LeftFeatureTable = LeftBaseClass.FeatureTable;

                    if (!FeatureName.TableContain(LeftFeatureTable, OperatorName, out IFeatureName Key, out IFeatureInstance Value))
                    {
                        errorList.AddError(new ErrorUnknownIdentifier(Operator, OperatorName));
                        return false;
                    }

                    Debug.Assert(Value.Feature.IsAssigned);
                    ICompiledFeature OperatorFeature = Value.Feature.Item;
                    Debug.Assert(OperatorFeature.ResolvedFeatureType.IsAssigned);
                    ICompiledType OperatorType = OperatorFeature.ResolvedFeatureType.Item;

                    if (OperatorType is FunctionType AsFunctionType && OperatorFeature is IFunctionFeature AsFunctionFeature)
                    {
                        IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();
                        foreach (IQueryOverloadType Overload in AsFunctionType.OverloadList)
                            ParameterTableList.Add(Overload.ParameterTable);

                        IList<IExpressionType> RightResult = RightExpression.ResolvedResult.Item;
                        if (!Argument.ArgumentsConformToParameters(ParameterTableList, RightResult, TypeArgumentStyles.Positional, errorList, Operator, out int SelectedIndex))
                            return false;

                        IQueryOverloadType SelectedOverloadType = AsFunctionType.OverloadList[SelectedIndex];
                        resolvedResult = SelectedOverloadType.Result;
                        selectedFeature = AsFunctionFeature;
                        selectedOverload = AsFunctionFeature.OverloadList[SelectedIndex];
                        resolvedExceptions = SelectedOverloadType.ExceptionIdentifierList;

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

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IBinaryOperatorExpression node, object data)
        {
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>)data).Item4;
            IFunctionFeature SelectedFeature = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>)data).Item5;
            IQueryOverload SelectedOverload = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>)data).Item6;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.SelectedFeature.Item = SelectedFeature;
            node.SelectedOverload.Item = SelectedOverload;
        }
        #endregion
    }
}
