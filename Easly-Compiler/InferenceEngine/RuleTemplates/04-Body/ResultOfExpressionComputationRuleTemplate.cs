namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IResultOfExpression"/>.
    /// </summary>
    public interface IResultOfExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IResultOfExpression"/>.
    /// </summary>
    public class ResultOfExpressionComputationRuleTemplate : RuleTemplate<IResultOfExpression, ResultOfExpressionComputationRuleTemplate>, IResultOfExpressionComputationRuleTemplate
    {
        #region Init
        static ResultOfExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IResultOfExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IResultOfExpression>.Default),
                new OnceReferenceSourceTemplate<IResultOfExpression, IResultException>(nameof(IResultOfExpression.Source) + Dot + nameof(IExpression.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IResultOfExpression, IResultException>(nameof(IResultOfExpression.ResolvedException)),
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
        public override bool CheckConsistency(IResultOfExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= ResultOfExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ISealableList<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant);

            if (Success)
                data = new Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IResultOfExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant>)data).Item2;
            ISealableList<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant>)data).Item4;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
