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
                new OnceReferenceSourceTemplate<IBinaryOperatorExpression, IResultType>(nameof(IBinaryOperatorExpression.LeftExpression) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceSourceTemplate<IBinaryOperatorExpression, IResultType>(nameof(IBinaryOperatorExpression.RightExpression) + Dot + nameof(IExpression.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IBinaryOperatorExpression, IResultType>(nameof(IBinaryOperatorExpression.ResolvedResult)),
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

            Success &= BinaryOperatorExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out IFunctionFeature SelectedFeature, out IQueryOverload SelectedOverload);
            if (Success)
                data = new Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, SelectedFeature, SelectedOverload);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IBinaryOperatorExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>)data).Item4;
            IFunctionFeature SelectedFeature = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>)data).Item5;
            IQueryOverload SelectedOverload = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload>)data).Item6;

            node.ResolvedResult.Item = ResolvedResult;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.SelectedFeature.Item = SelectedFeature;
            node.SelectedOverload.Item = SelectedOverload;
        }
        #endregion
    }
}
