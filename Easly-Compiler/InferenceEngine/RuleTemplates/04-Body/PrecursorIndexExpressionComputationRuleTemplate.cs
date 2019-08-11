namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexExpression"/>.
    /// </summary>
    public interface IPrecursorIndexExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexExpression"/>.
    /// </summary>
    public class PrecursorIndexExpressionComputationRuleTemplate : RuleTemplate<IPrecursorIndexExpression, PrecursorIndexExpressionComputationRuleTemplate>, IPrecursorIndexExpressionComputationRuleTemplate
    {
        #region Init
        static PrecursorIndexExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IPrecursorIndexExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IPrecursorIndexExpression>.Default),
                new OnceReferenceCollectionSourceTemplate<IPrecursorIndexExpression, IArgument, IResultException>(nameof(IPrecursorIndexExpression.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorIndexExpression, IResultException>(nameof(IPrecursorIndexExpression.ResolvedException)),
                new OnceReferenceDestinationTemplate<IPrecursorIndexExpression, IFeatureCall>(nameof(IPrecursorIndexExpression.FeatureCall)),
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
        public override bool CheckConsistency(IPrecursorIndexExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= PrecursorIndexExpression.ResolveCompilerReferences(node, ErrorList, out ResolvedExpression ResolvedExpression);

            if (Success)
                data = ResolvedExpression;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IPrecursorIndexExpression node, object data)
        {
            ResolvedExpression ResolvedExpression = (ResolvedExpression)data;

            node.ResolvedException.Item = ResolvedExpression.ResolvedException;

            Debug.Assert(ResolvedExpression.ResolvedFinalFeature is IIndexerFeature);
            node.ResolvedIndexer.Item = ResolvedExpression.ResolvedFinalFeature as IIndexerFeature;

            node.FeatureCall.Item = ResolvedExpression.FeatureCall;

            IFeature EmbeddingFeature = node.EmbeddingFeature;
            IFeatureWithPrecursor ResolvedFeature = EmbeddingFeature.ResolvedFeature.Item as IFeatureWithPrecursor;
            Debug.Assert(ResolvedFeature != null);
            ResolvedFeature.MarkAsCallingPrecursor();
        }
        #endregion
    }
}
