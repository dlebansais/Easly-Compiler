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
    public interface IPrecursorExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorExpression"/>.
    /// </summary>
    public class PrecursorExpressionRuleTemplate : RuleTemplate<IPrecursorExpression, PrecursorExpressionRuleTemplate>, IPrecursorExpressionRuleTemplate
    {
        #region Init
        static PrecursorExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new ConditionallyAssignedReferenceSourceTemplate<IPrecursorExpression, IObjectType, ITypeName>(nameof(IPrecursorExpression.AncestorType), nameof(IObjectType.ResolvedTypeName)),
                new ConditionallyAssignedReferenceSourceTemplate<IPrecursorExpression, IObjectType, ICompiledType>(nameof(IPrecursorExpression.AncestorType), nameof(IObjectType.ResolvedType)),
                new OnceReferenceCollectionSourceTemplate<IPrecursorExpression, IArgument, IResultType>(nameof(IPrecursorExpression.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorExpression, IResultType>(nameof(IPrecursorExpression.ResolvedResult)),
                new UnsealedListDestinationTemplate<IPrecursorExpression, IExpression>(nameof(IPrecursorExpression.ConstantSourceList)),
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

            node.ResolvedResult.Item = ResolvedExpression.ResolvedResult;
            node.ConstantSourceList.AddRange(ResolvedExpression.ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.ResolvedPrecursor.Item = ResolvedExpression.SelectedPrecursor;

            if (node.ConstantSourceList.Count == 0)
                node.ExpressionConstant.Item = ResolvedExpression.ExpressionConstant;
        }
        #endregion
    }
}
