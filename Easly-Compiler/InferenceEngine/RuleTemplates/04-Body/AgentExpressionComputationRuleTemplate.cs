﻿namespace EaslyCompiler
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
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAgentExpression, IResultType>(nameof(IAgentExpression.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IAgentExpression, IList<IIdentifier>>(nameof(IAgentExpression.ResolvedExceptions)),
                new OnceReferenceDestinationTemplate<IAgentExpression, ICompiledType>(nameof(IAgentExpression.ResolvedAncestorType)),
                new UnsealedListDestinationTemplate<IAgentExpression, IExpression>(nameof(IAgentExpression.ConstantSourceList)),
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

            Success &= AgentExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ICompiledFeature ResolvedFeature);

            if (Success)
                data = new Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant, ResolvedFeature);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAgentExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item4;
            ICompiledFeature ResolvedFeature = ((Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item5;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.ExpressionConstant.Item = ExpressionConstant;
            node.ResolvedAncestorTypeName.Item = ResolvedFeature.ResolvedFeatureTypeName.Item;
            node.ResolvedAncestorType.Item = ResolvedFeature.ResolvedFeatureType.Item;
            node.ResolvedFeature.Item = ResolvedFeature;
        }
        #endregion
    }
}
