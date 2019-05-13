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
                new OnceReferenceSourceTemplate<IBinaryOperatorExpression, IList<IExpressionType>>(nameof(IBinaryOperatorExpression.RightExpression) + Dot + nameof(IExpression.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IBinaryOperatorExpression, IList<IExpressionType>>(nameof(IBinaryOperatorExpression.ResolvedResult)),
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

            Success &= BinaryOperatorExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out bool IsResultConstant, out IFunctionFeature SelectedFeature, out IQueryOverload SelectedOverload, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions);
            if (Success)
                data = new Tuple<bool, IFunctionFeature, IQueryOverload, IList<IExpressionType>, IList<IIdentifier>>(IsResultConstant, SelectedFeature, SelectedOverload, ResolvedResult, ResolvedExceptions);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IBinaryOperatorExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="isResultConstant">True upon return if the result is a constant.</param>
        /// <param name="selectedFeature">The matching feature upon return.</param>
        /// <param name="selectedOverload">The matching overload in <paramref name="selectedFeature"/> upon return.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        public static bool ResolveCompilerReferences(IBinaryOperatorExpression node, IErrorList errorList, out bool isResultConstant, out IFunctionFeature selectedFeature, out IQueryOverload selectedOverload, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions)
        {
            isResultConstant = false;
            selectedFeature = null;
            selectedOverload = null;
            resolvedResult = null;
            resolvedExceptions = null;

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

                    if (OperatorType is FunctionType AsFunctionType)
                    {
                        if (OperatorFeature is IFunctionFeature AsFunctionFeature)
                        {
                            IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();
                            foreach (IQueryOverloadType Overload in AsFunctionType.OverloadList)
                                ParameterTableList.Add(Overload.ParameterTable);

                            IList<IExpressionType> RightResult = RightExpression.ResolvedResult.Item;
int SelectedIndex;
                            if (!Argument.ArgumentsConformToParameters(ParameterTableList, RightResult, TypeArgumentStyles.Positional, errorList, Operator, out SelectedIndex))
                                return false;

                            IQueryOverloadType SelectedOverloadType = AsFunctionType.OverloadList[SelectedIndex];
                            resolvedResult = SelectedOverloadType.Result;
                            selectedFeature = AsFunctionFeature;
                            selectedOverload = AsFunctionFeature.OverloadList[SelectedIndex];
                            resolvedExceptions = SelectedOverloadType.ExceptionIdentifierList;
                            isResultConstant = LeftExpression.IsConstant && RightExpression.IsConstant;
                            if (LeftExpression.NumberConstant.IsAssigned && RightExpression.NumberConstant.IsAssigned)
                            {
                                //TODO: evaluate the result
                                //ResultNumberConstant.Item = ??
                            }
                        }
                        else
                        {
                            errorList.AddError(new ErrorInvalidOperator(Operator, OperatorName));
                            return false;
                        }
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
            bool IsResultConstant = ((Tuple<bool, IFunctionFeature, IQueryOverload, IList<IExpressionType>, IList<IIdentifier>>)data).Item1;
            IFunctionFeature SelectedFeature = ((Tuple<bool, IFunctionFeature, IQueryOverload, IList<IExpressionType>, IList<IIdentifier>>)data).Item2;
            IQueryOverload SelectedOverload = ((Tuple<bool, IFunctionFeature, IQueryOverload, IList<IExpressionType>, IList<IIdentifier>>)data).Item3;
            IList<IExpressionType> ResolvedResult = ((Tuple<bool, IFunctionFeature, IQueryOverload, IList<IExpressionType>, IList<IIdentifier>>)data).Item4;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<bool, IFunctionFeature, IQueryOverload, IList<IExpressionType>, IList<IIdentifier>>)data).Item5;

            node.ResolvedResult.Item = ResolvedResult;
            node.SelectedFeature.Item = SelectedFeature;
            node.SelectedOverload.Item = SelectedOverload;

            //node.SetIsConstant(IsResultConstant, ResultNumberConstant);
            node.SetIsConstant(IsResultConstant);
        }
        #endregion
    }
}
