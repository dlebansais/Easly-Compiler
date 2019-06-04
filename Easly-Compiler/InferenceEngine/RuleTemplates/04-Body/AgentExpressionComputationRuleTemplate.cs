namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IAgentExpression"/>.
    /// </summary>
    public interface IAgentExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAgentExpression"/>.
    /// </summary>
    public class AgentExpressionComputationRuleTemplate : RuleTemplate<IAgentExpression, AgentExpressionComputationRuleTemplate>, IAgentExpressionComputationRuleTemplate
    {
        #region Init
        static AgentExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IAgentExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IAgentExpression>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAgentExpression, IResultException>(nameof(IAgentExpression.ResolvedException)),
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
        public override bool CheckConsistency(IAgentExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= AgentExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ISealableList<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ICompiledFeature ResolvedFeature);

            if (Success)
                data = new Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, ResolvedFeature);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAgentExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item2;
            ISealableList<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item4;
            ICompiledFeature ResolvedFeature = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item5;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
