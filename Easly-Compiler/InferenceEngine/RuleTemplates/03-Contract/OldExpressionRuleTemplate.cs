namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IOldExpression"/>.
    /// </summary>
    public interface IOldExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IOldExpression"/>.
    /// </summary>
    public class OldExpressionRuleTemplate : RuleTemplate<IOldExpression, OldExpressionRuleTemplate>, IOldExpressionRuleTemplate
    {
        #region Init
        static OldExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceTableSourceTemplate<IOldExpression, string, IScopeAttributeFeature, ITypeName>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedEffectiveTypeName), TemplateScopeStart<IOldExpression>.Default),
                new OnceReferenceTableSourceTemplate<IOldExpression, string, IScopeAttributeFeature, ICompiledType>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedEffectiveType), TemplateScopeStart<IOldExpression>.Default),
                new OnceReferenceSourceTemplate<IOldExpression, IList<IExpressionType>>(nameof(IOldExpression.Query) + Dot + nameof(IQualifiedName.ValidResultTypePath)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IOldExpression, IResultType>(nameof(IOldExpression.ResolvedResult)),
                new UnsealedListDestinationTemplate<IOldExpression, IExpression>(nameof(IOldExpression.ConstantSourceList)),
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

            Success &= OldExpression.ResolveCompilerReferences(node, ErrorList, out ResolvedExpression ResolvedExpression);

            if (Success)
                data = ResolvedExpression;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IOldExpression node, object data)
        {
            ResolvedExpression ResolvedExpression = (ResolvedExpression)data;

            node.ResolvedResult.Item = ResolvedExpression.ResolvedResult;
            node.ConstantSourceList.AddRange(ResolvedExpression.ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.ExpressionConstant.Item = ResolvedExpression.ExpressionConstant;
        }
        #endregion
    }
}
