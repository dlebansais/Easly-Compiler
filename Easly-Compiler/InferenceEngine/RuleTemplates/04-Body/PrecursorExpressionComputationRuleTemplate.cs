namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IPrecursorExpression"/>.
    /// </summary>
    public interface IPrecursorExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorExpression"/>.
    /// </summary>
    public class PrecursorExpressionComputationRuleTemplate : RuleTemplate<IPrecursorExpression, PrecursorExpressionComputationRuleTemplate>, IPrecursorExpressionComputationRuleTemplate
    {
        #region Init
        static PrecursorExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IPrecursorExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IPrecursorExpression>.Default),
                new OnceReferenceCollectionSourceTemplate<IPrecursorExpression, IArgument, IResultException>(nameof(IPrecursorExpression.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorExpression, IResultException>(nameof(IPrecursorExpression.ResolvedException)),
                new OnceReferenceDestinationTemplate<IPrecursorExpression, IFeatureCall>(nameof(IPrecursorExpression.FeatureCall)),
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
        public override bool CheckConsistency(IPrecursorExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= PrecursorExpression.ResolveCompilerReferences(node, ErrorList, out ResolvedExpression ResolvedExpression);

            if (Success)
                data = ResolvedExpression;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IPrecursorExpression node, object data)
        {
            ResolvedExpression ResolvedExpression = (ResolvedExpression)data;

            node.ResolvedException.Item = ResolvedExpression.ResolvedException;

            if (ResolvedExpression.SelectedOverload != null)
                node.SelectedOverload.Item = ResolvedExpression.SelectedOverload;

            if (ResolvedExpression.SelectedOverloadType != null)
                node.SelectedOverloadType.Item = ResolvedExpression.SelectedOverloadType;

            node.FeatureCall.Item = ResolvedExpression.FeatureCall;

            IFeature EmbeddingFeature = node.EmbeddingFeature;
            IFeatureWithPrecursor ResolvedFeature = EmbeddingFeature.ResolvedFeature.Item as IFeatureWithPrecursor;
            Debug.Assert(ResolvedFeature != null);
            ResolvedFeature.MarkAsCallingPrecursor();
        }
        #endregion
    }
}
