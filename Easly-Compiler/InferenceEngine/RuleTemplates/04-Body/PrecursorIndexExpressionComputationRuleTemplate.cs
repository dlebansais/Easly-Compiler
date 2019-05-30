namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexExpression"/>.
    /// </summary>
    public interface IPrecursorIndexExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexExpression"/>.
    /// </summary>
    public class PrecursorIndexExpressionComputationRuleTemplate : RuleTemplate<IPrecursorIndexExpression, PrecursorIndexExpressionComputationRuleTemplate>, IPrecursorIndexExpressionComputationRuleTemplate
    {
        #region Init
        static PrecursorIndexExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IPrecursorIndexExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IPrecursorIndexExpression>.Default),
                new OnceReferenceCollectionSourceTemplate<IPrecursorIndexExpression, IArgument, IResultException>(nameof(IPrecursorIndexExpression.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorIndexExpression, IResultException>(nameof(IPrecursorIndexExpression.ResolvedException)),
                new UnsealedListDestinationTemplate<IPrecursorIndexExpression, IParameter>(nameof(IPrecursorIndexExpression.SelectedParameterList)),
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
        public override bool CheckConsistency(IPrecursorIndexExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= PrecursorIndexExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ListTableEx<IParameter> SelectedParameterList, out List<IExpressionType> ResolvedArgumentList, out TypeArgumentStyles ArgumentStyle);

            if (Success)
                data = new Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, SelectedParameterList, ResolvedArgumentList, ArgumentStyle);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IPrecursorIndexExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles>)data).Item4;
            ListTableEx<IParameter> SelectedParameterList = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles>)data).Item5;
            List<IExpressionType> ResolvedArgumentList = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles>)data).Item6;
            TypeArgumentStyles ArgumentStyle = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles>)data).Item7;

            node.ResolvedException.Item = ResolvedException;
            node.SelectedParameterList.AddRange(SelectedParameterList);
            node.SelectedParameterList.Seal();
            node.ResolvedArgumentList.Item = ResolvedArgumentList;
            node.ArgumentStyle = ArgumentStyle;

            IFeature EmbeddingFeature = node.EmbeddingFeature;
            IFeatureWithPrecursor ResolvedFeature = EmbeddingFeature.ResolvedFeature.Item as IFeatureWithPrecursor;
            Debug.Assert(ResolvedFeature != null);
            ResolvedFeature.MarkAsCallingPrecursor();
        }
        #endregion
    }
}
