namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IIndexQueryExpression"/>.
    /// </summary>
    public interface IIndexQueryExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIndexQueryExpression"/>.
    /// </summary>
    public class IndexQueryExpressionComputationRuleTemplate : RuleTemplate<IIndexQueryExpression, IndexQueryExpressionComputationRuleTemplate>, IIndexQueryExpressionComputationRuleTemplate
    {
        #region Init
        static IndexQueryExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IIndexQueryExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IIndexQueryExpression>.Default),
                new OnceReferenceSourceTemplate<IIndexQueryExpression, IResultException>(nameof(IIndexQueryExpression.IndexedExpression) + Dot + nameof(IExpression.ResolvedException)),
                new OnceReferenceCollectionSourceTemplate<IIndexQueryExpression, IArgument, IResultException>(nameof(IIndexQueryExpression.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIndexQueryExpression, IResultException>(nameof(IIndexQueryExpression.ResolvedException)),
                new OnceReferenceDestinationTemplate<IIndexQueryExpression, IFeatureCall>(nameof(IIndexQueryExpression.FeatureCall)),
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
        public override bool CheckConsistency(IIndexQueryExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= IndexQueryExpression.ResolveCompilerReferences(node, ErrorList, out ResolvedExpression ResolvedExpression);

            if (Success)
                data = ResolvedExpression;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IIndexQueryExpression node, object data)
        {
            ResolvedExpression ResolvedExpression = (ResolvedExpression)data;

            node.ResolvedException.Item = ResolvedExpression.ResolvedException;

            Debug.Assert(ResolvedExpression.ResolvedFinalFeature is IIndexerFeature);
            node.ResolvedIndexer.Item = ResolvedExpression.ResolvedFinalFeature as IIndexerFeature;

            node.FeatureCall.Item = ResolvedExpression.FeatureCall;
        }
        #endregion
    }
}
