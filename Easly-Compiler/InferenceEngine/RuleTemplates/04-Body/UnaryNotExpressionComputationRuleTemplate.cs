namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IUnaryNotExpression"/>.
    /// </summary>
    public interface IUnaryNotExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IUnaryNotExpression"/>.
    /// </summary>
    public class UnaryNotExpressionComputationRuleTemplate : RuleTemplate<IUnaryNotExpression, UnaryNotExpressionComputationRuleTemplate>, IUnaryNotExpressionComputationRuleTemplate
    {
        #region Init
        static UnaryNotExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IUnaryNotExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IUnaryNotExpression>.Default),
                new OnceReferenceSourceTemplate<IUnaryNotExpression, IResultException>(nameof(IUnaryNotExpression.RightExpression) + Dot + nameof(IExpression.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IUnaryNotExpression, IResultException>(nameof(IUnaryNotExpression.ResolvedException)),
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
        public override bool CheckConsistency(IUnaryNotExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= UnaryNotExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant);
            if (Success)
                data = new Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IUnaryNotExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant>)data).Item4;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
