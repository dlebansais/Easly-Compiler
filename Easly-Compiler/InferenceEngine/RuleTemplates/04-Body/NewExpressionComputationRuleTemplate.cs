namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="INewExpression"/>.
    /// </summary>
    public interface INewExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="INewExpression"/>.
    /// </summary>
    public class NewExpressionComputationRuleTemplate : RuleTemplate<INewExpression, NewExpressionComputationRuleTemplate>, INewExpressionComputationRuleTemplate
    {
        #region Init
        static NewExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<INewExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<INewExpression>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<INewExpression, IResultException>(nameof(INewExpression.ResolvedException)),
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
        public override bool CheckConsistency(INewExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= NewExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ICompiledFeature ResolvedFinalFeature);

            if (Success)
                data = new Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, ResolvedFinalFeature);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(INewExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item4;
            ICompiledFeature ResolvedFinalFeature = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item5;

            node.ResolvedException.Item = ResolvedException;
            node.ResolvedFinalFeature.Item = ResolvedFinalFeature;
        }
        #endregion
    }
}
