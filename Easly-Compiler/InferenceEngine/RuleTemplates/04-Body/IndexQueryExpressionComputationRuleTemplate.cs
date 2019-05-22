namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IIndexQueryExpression"/>.
    /// </summary>
    public interface IIndexQueryExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIndexQueryExpression"/>.
    /// </summary>
    public class IndexQueryExpressionComputationRuleTemplate : RuleTemplate<IIndexQueryExpression, IndexQueryExpressionComputationRuleTemplate>, IIndexQueryExpressionComputationRuleTemplate
    {
        #region Init
        static IndexQueryExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IIndexQueryExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IIndexQueryExpression>.Default),
                new OnceReferenceSourceTemplate<IIndexQueryExpression, IResultException>(nameof(IIndexQueryExpression.IndexedExpression) + Dot + nameof(IExpression.ResolvedException)),
                new OnceReferenceCollectionSourceTemplate<IIndexQueryExpression, IArgument, IResultException>(nameof(IIndexQueryExpression.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIndexQueryExpression, IResultException>(nameof(IIndexQueryExpression.ResolvedException)),
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
        public override bool CheckConsistency(IIndexQueryExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= IndexQueryExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ListTableEx<IParameter> SelectedParameterList, out List<IExpressionType> ResolvedArgumentList);

            if (Success)
                data = new Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, SelectedParameterList, ResolvedArgumentList);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IIndexQueryExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>>)data).Item4;
            ListTableEx<IParameter> SelectedParameterList = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>>)data).Item5;
            List<IExpressionType> ResolvedArgumentList = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, List<IExpressionType>>)data).Item6;

            node.ResolvedException.Item = ResolvedException;
            node.SelectedParameterList.AddRange(SelectedParameterList);
            node.SelectedParameterList.Seal();
            node.ResolvedArgumentList.Item = ResolvedArgumentList;
        }
        #endregion
    }
}
