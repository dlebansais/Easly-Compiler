namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IEqualityExpression"/>.
    /// </summary>
    public interface IEqualityExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IEqualityExpression"/>.
    /// </summary>
    public class EqualityExpressionComputationRuleTemplate : RuleTemplate<IEqualityExpression, EqualityExpressionComputationRuleTemplate>, IEqualityExpressionComputationRuleTemplate
    {
        #region Init
        static EqualityExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IEqualityExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IEqualityExpression>.Default),
                new OnceReferenceSourceTemplate<IEqualityExpression, IResultException>(nameof(IEqualityExpression.LeftExpression) + Dot + nameof(IExpression.ResolvedException)),
                new OnceReferenceSourceTemplate<IEqualityExpression, IResultException>(nameof(IEqualityExpression.RightExpression) + Dot + nameof(IExpression.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IEqualityExpression, IResultException>(nameof(IEqualityExpression.ResolvedException)),
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
        public override bool CheckConsistency(IEqualityExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= EqualityExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant);
            if (Success)
                data = new Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IEqualityExpression node, object data)
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
