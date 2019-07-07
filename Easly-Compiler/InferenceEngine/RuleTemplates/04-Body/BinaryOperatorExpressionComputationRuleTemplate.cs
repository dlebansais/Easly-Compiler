namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IBinaryOperatorExpression"/>.
    /// </summary>
    public interface IBinaryOperatorExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBinaryOperatorExpression"/>.
    /// </summary>
    public class BinaryOperatorExpressionComputationRuleTemplate : RuleTemplate<IBinaryOperatorExpression, BinaryOperatorExpressionComputationRuleTemplate>, IBinaryOperatorExpressionComputationRuleTemplate
    {
        #region Init
        static BinaryOperatorExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IBinaryOperatorExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IBinaryOperatorExpression>.Default),
                new OnceReferenceSourceTemplate<IBinaryOperatorExpression, IResultException>(nameof(IBinaryOperatorExpression.LeftExpression) + Dot + nameof(IExpression.ResolvedException)),
                new OnceReferenceSourceTemplate<IBinaryOperatorExpression, IResultException>(nameof(IBinaryOperatorExpression.RightExpression) + Dot + nameof(IExpression.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IBinaryOperatorExpression, IResultException>(nameof(IBinaryOperatorExpression.ResolvedException)),
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

            Success &= BinaryOperatorExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ISealableList<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out IFunctionFeature SelectedFeature, out IQueryOverload SelectedOverload, out IFeatureCall FeatureCall);
            if (Success)
                data = new Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IFeatureCall>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, SelectedFeature, SelectedOverload, FeatureCall);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IBinaryOperatorExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IFeatureCall>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IFeatureCall>)data).Item2;
            ISealableList<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IFeatureCall>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IFeatureCall>)data).Item4;
            IFunctionFeature SelectedFeature = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IFeatureCall>)data).Item5;
            IQueryOverload SelectedOverload = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IFeatureCall>)data).Item6;
            IFeatureCall FeatureCall = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IFeatureCall>)data).Item7;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
