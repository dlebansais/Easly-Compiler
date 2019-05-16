namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IUnaryOperatorExpression"/>.
    /// </summary>
    public interface IUnaryOperatorExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IUnaryOperatorExpression"/>.
    /// </summary>
    public class UnaryOperatorExpressionRuleTemplate : RuleTemplate<IUnaryOperatorExpression, UnaryOperatorExpressionRuleTemplate>, IUnaryOperatorExpressionRuleTemplate
    {
        #region Init
        static UnaryOperatorExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IUnaryOperatorExpression, IList<IExpressionType>>(nameof(IUnaryOperatorExpression.RightExpression) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceSourceTemplate<IUnaryOperatorExpression, IList<IIdentifier>>(nameof(IUnaryOperatorExpression.RightExpression) + Dot + nameof(IExpression.ResolvedExceptions)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IUnaryOperatorExpression, IList<IExpressionType>>(nameof(IUnaryOperatorExpression.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IUnaryOperatorExpression, IList<IIdentifier>>(nameof(IUnaryOperatorExpression.ResolvedExceptions)),
                new UnsealedListDestinationTemplate<IUnaryOperatorExpression, IExpression>(nameof(IUnaryOperatorExpression.ConstantSourceList)),
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
        public override bool CheckConsistency(IUnaryOperatorExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= UnaryOperatorExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out IFunctionFeature SelectedFeature, out IQueryOverload SelectedOverload, out IQueryOverloadType SelectedOverloadType);
            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant, SelectedFeature, SelectedOverload, SelectedOverloadType);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IUnaryOperatorExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The constant value upon return, if any.</param>
        /// <param name="selectedFeature">The matching feature upon return.</param>
        /// <param name="selectedOverload">The matching overload in <paramref name="selectedFeature"/> upon return.</param>
        /// <param name="selectedOverloadType">The matching overload type upon return.</param>
        public static bool ResolveCompilerReferences(IUnaryOperatorExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out IFunctionFeature selectedFeature, out IQueryOverload selectedOverload, out IQueryOverloadType selectedOverloadType)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            selectedFeature = null;
            selectedOverload = null;
            selectedOverloadType = null;

            IIdentifier Operator = (IIdentifier)node.Operator;
            string ValidText = Operator.ValidText.Item;
            IExpression RightExpression = (IExpression)node.RightExpression;
            IList<IExpressionType> RightResult = RightExpression.ResolvedResult.Item;

            OnceReference<ICompiledType> RightExpressionType = new OnceReference<ICompiledType>();
            if (RightResult.Count == 1)
                RightExpressionType.Item = RightResult[0].ValueType;
            else
                foreach (IExpressionType Item in RightResult)
                    if (Item.Name == nameof(BaseNode.Keyword.Result))
                    {
                        RightExpressionType.Item = Item.ValueType;
                        break;
                    }

            if (!RightExpressionType.IsAssigned)
            {
                errorList.AddError(new ErrorInvalidExpression(RightExpression));
                return false;
            }

            if (RightExpressionType.Item is IClassType AsClassType)
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

                if (OperatorType is IFunctionType AsFunctionType)
                {
                    if (OperatorFeature is IFunctionFeature AsFunctionFeature)
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
                        resolvedExceptions = selectedOverloadType.ExceptionIdentifierList;

                        //TODO: check that the result is a number constant
                        //ResultNumberConstant.Item = ??
                    }
                    else
                    {
                        errorList.AddError(new ErrorInvalidOperator(Operator, ValidText));
                        return false;
                    }
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

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IUnaryOperatorExpression node, object data)
        {
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item4;
            IFunctionFeature SelectedFeature = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item5;
            IQueryOverload SelectedOverload = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item6;
            IQueryOverloadType SelectedOverloadType = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item7;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.ExpressionConstant.Item = ExpressionConstant;
            node.SelectedFeature.Item = SelectedFeature;
            node.SelectedOverload.Item = SelectedOverload;
            node.SelectedOverloadType.Item = SelectedOverloadType;
        }
        #endregion
    }
}
