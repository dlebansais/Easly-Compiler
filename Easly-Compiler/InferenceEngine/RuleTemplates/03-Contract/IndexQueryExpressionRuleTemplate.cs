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
    public interface IIndexQueryExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIndexQueryExpression"/>.
    /// </summary>
    public class IndexQueryExpressionRuleTemplate : RuleTemplate<IIndexQueryExpression, IndexQueryExpressionRuleTemplate>, IIndexQueryExpressionRuleTemplate
    {
        #region Init
        static IndexQueryExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IIndexQueryExpression, IResultType>(nameof(IIndexQueryExpression.IndexedExpression) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceCollectionSourceTemplate<IIndexQueryExpression, IArgument, IResultType>(nameof(IIndexQueryExpression.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIndexQueryExpression, IResultType>(nameof(IIndexQueryExpression.ResolvedResult)),
                new UnsealedListDestinationTemplate<IIndexQueryExpression, IExpression>(nameof(IIndexQueryExpression.ConstantSourceList)),
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

            Debug.Assert(ResolvedExpression.ConstantSourceList.Count > 0);

            node.ResolvedResult.Item = ResolvedExpression.ResolvedResult;
            node.ConstantSourceList.AddRange(ResolvedExpression.ConstantSourceList);
            node.ConstantSourceList.Seal();
        }
        #endregion
    }
}
