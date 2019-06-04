namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IEntityExpression"/>.
    /// </summary>
    public interface IEntityExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IEntityExpression"/>.
    /// </summary>
    public class EntityExpressionComputationRuleTemplate : RuleTemplate<IEntityExpression, EntityExpressionComputationRuleTemplate>, IEntityExpressionComputationRuleTemplate
    {
        #region Init
        static EntityExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IEntityExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IEntityExpression>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IEntityExpression, IResultException>(nameof(IEntityExpression.ResolvedException)),
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
        public override bool CheckConsistency(IEntityExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= EntityExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ISealableList<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ICompiledFeature ResolvedFinalFeature, out IDiscrete ResolvedFinalDiscrete);

            if (Success)
                data = new Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, ResolvedFinalFeature, ResolvedFinalDiscrete);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IEntityExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>)data).Item2;
            ISealableList<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>)data).Item4;
            ICompiledFeature ResolvedFinalFeature = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>)data).Item5;
            IDiscrete ResolvedFinalDiscrete = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>)data).Item6;

            node.ResolvedException.Item = ResolvedException;

            if (ResolvedFinalFeature != null)
                node.ResolvedFinalFeature.Item = ResolvedFinalFeature;

            if (ResolvedFinalDiscrete != null)
                node.ResolvedFinalDiscrete.Item = ResolvedFinalDiscrete;
        }
        #endregion
    }
}
