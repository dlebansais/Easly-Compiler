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
                new OnceReferenceSourceTemplate<IUnaryOperatorExpression, IResultType>(nameof(IUnaryOperatorExpression.RightExpression) + Dot + nameof(IExpression.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IUnaryOperatorExpression, IResultType>(nameof(IUnaryOperatorExpression.ResolvedResult)),
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

            Success &= UnaryOperatorExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out IFunctionFeature SelectedFeature, out IQueryOverload SelectedOverload, out IQueryOverloadType SelectedOverloadType);
            if (Success)
                data = new Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, SelectedFeature, SelectedOverload, SelectedOverloadType);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IUnaryOperatorExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item4;
            IFunctionFeature SelectedFeature = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item5;
            IQueryOverload SelectedOverload = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item6;
            IQueryOverloadType SelectedOverloadType = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item7;

            node.ResolvedResult.Item = ResolvedResult;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.SelectedFeature.Item = SelectedFeature;
            node.SelectedOverload.Item = SelectedOverload;
            node.SelectedOverloadType.Item = SelectedOverloadType;
        }
        #endregion
    }
}
