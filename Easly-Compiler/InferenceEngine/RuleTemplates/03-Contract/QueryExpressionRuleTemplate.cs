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
    public interface IQueryExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQueryExpression"/>.
    /// </summary>
    public class QueryExpressionRuleTemplate : RuleTemplate<IQueryExpression, QueryExpressionRuleTemplate>, IQueryExpressionRuleTemplate
    {
        #region Init
        static QueryExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IQueryExpression, string, IScopeAttributeFeature>(nameof(IScope.FullScope), TemplateScopeStart<IQueryExpression>.Default),
                new OnceReferenceTableSourceTemplate<IQueryExpression, string, IScopeAttributeFeature, ITypeName>(nameof(IScope.FullScope), nameof(IScopeAttributeFeature.ResolvedEffectiveTypeName), TemplateScopeStart<IQueryExpression>.Default),
                new OnceReferenceTableSourceTemplate<IQueryExpression, string, IScopeAttributeFeature, ICompiledType>(nameof(IScope.FullScope), nameof(IScopeAttributeFeature.ResolvedEffectiveType), TemplateScopeStart<IQueryExpression>.Default),
                new OnceReferenceSourceTemplate<IQueryExpression, IList<IExpressionType>>(nameof(IQueryExpression.Query) + Dot + nameof(IQualifiedName.ValidResultTypePath)),
                new OnceReferenceCollectionSourceTemplate<IQueryExpression, IArgument, IResultType>(nameof(IQueryExpression.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IQueryExpression, IResultType>(nameof(IQueryExpression.ResolvedResult)),
                new UnsealedListDestinationTemplate<IQueryExpression, IExpression>(nameof(IQueryExpression.ConstantSourceList)),
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

            node.ResolvedResult.Item = ResolvedExpression.ResolvedResult;

            node.ConstantSourceList.AddRange(ResolvedExpression.ConstantSourceList);
            node.ConstantSourceList.Seal();

            if (node.ConstantSourceList.Count == 0)
                node.ExpressionConstant.Item = ResolvedExpression.ExpressionConstant;

            Debug.Assert(ResolvedExpression.ResolvedFinalFeature != null || ResolvedExpression.ResolvedFinalDiscrete != null);

            if (ResolvedExpression.ResolvedFinalFeature != null)
                node.ResolvedFinalFeature.Item = ResolvedExpression.ResolvedFinalFeature;

            if (ResolvedExpression.ResolvedFinalDiscrete != null)
                node.ResolvedFinalDiscrete.Item = ResolvedExpression.ResolvedFinalDiscrete;
        }
        #endregion
    }
}
