namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IOldExpression"/>.
    /// </summary>
    public interface IOldExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IOldExpression"/>.
    /// </summary>
    public class OldExpressionComputationRuleTemplate : RuleTemplate<IOldExpression, OldExpressionComputationRuleTemplate>, IOldExpressionComputationRuleTemplate
    {
        #region Init
        static OldExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IOldExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IOldExpression>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IOldExpression, IResultException>(nameof(IOldExpression.ResolvedException)),
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
        public override bool CheckConsistency(IOldExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= OldExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ISealableList<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ICompiledFeature ResolvedFinalFeature);

            if (Success)
                data = new Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, ResolvedFinalFeature);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IOldExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item2;
            ISealableList<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item4;
            ICompiledFeature ResolvedFinalFeature = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item5;

            node.ResolvedException.Item = ResolvedException;
            node.ResolvedFinalFeature.Item = ResolvedFinalFeature;
        }
        #endregion
    }
}
