namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IQueryExpression"/>.
    /// </summary>
    public interface IQueryExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQueryExpression"/>.
    /// </summary>
    public class QueryExpressionComputationRuleTemplate : RuleTemplate<IQueryExpression, QueryExpressionComputationRuleTemplate>, IQueryExpressionComputationRuleTemplate
    {
        #region Init
        static QueryExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IQueryExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IQueryExpression>.Default),
                new OnceReferenceCollectionSourceTemplate<IQueryExpression, IArgument, IResultException>(nameof(IQueryExpression.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IQueryExpression, IResultException>(nameof(IQueryExpression.ResolvedException)),
                new OnceReferenceDestinationTemplate<IQueryExpression, IFeatureCall>(nameof(IQueryExpression.FeatureCall)),
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
        public override bool CheckConsistency(IQueryExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= QueryExpression.ResolveCompilerReferences(node, ErrorList, out ResolvedExpression ResolvedExpression);

            if (Success)
                data = ResolvedExpression;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IQueryExpression node, object data)
        {
            ResolvedExpression ResolvedExpression = (ResolvedExpression)data;

            node.ResolvedException.Item = ResolvedExpression.ResolvedException;
            node.SelectedResultList.AddRange(ResolvedExpression.SelectedResultList);
            node.SelectedResultList.Seal();

            if (ResolvedExpression.SelectedOverload != null)
                node.SelectedOverload.Item = ResolvedExpression.SelectedOverload;

            if (ResolvedExpression.SelectedOverloadType != null)
                node.SelectedOverloadType.Item = ResolvedExpression.SelectedOverloadType;

            node.FeatureCall.Item = ResolvedExpression.FeatureCall;
            node.InheritBySideAttribute = ResolvedExpression.InheritBySideAttribute;
        }
        #endregion
    }
}
