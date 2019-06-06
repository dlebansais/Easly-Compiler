namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexExpression"/>.
    /// </summary>
    public interface IPrecursorIndexExpressionConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexExpression"/>.
    /// </summary>
    public class PrecursorIndexExpressionConstantRuleTemplate : RuleTemplate<IPrecursorIndexExpression, PrecursorIndexExpressionConstantRuleTemplate>, IPrecursorIndexExpressionConstantRuleTemplate
    {
        #region Init
        static PrecursorIndexExpressionConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IPrecursorIndexExpression, IExpression>(nameof(IPrecursorIndexExpression.ConstantSourceList)),
                new OnceReferenceCollectionSourceTemplate<IPrecursorIndexExpression, IExpression, ILanguageConstant>(nameof(IPrecursorIndexExpression.ConstantSourceList), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorIndexExpression, ILanguageConstant>(nameof(IPrecursorIndexExpression.ExpressionConstant)),
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

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IPrecursorIndexExpression node, object data)
        {
            ILanguageConstant ExpressionConstant = NeutralLanguageConstant.NotConstant;

            Debug.Assert(node.ConstantSourceList.Count > 0);

            bool IsConstant = true;
            foreach (IExpression ConstantSource in node.ConstantSourceList)
                IsConstant &= ConstantSource.ExpressionConstant.Item != NeutralLanguageConstant.NotConstant;

            if (IsConstant)
                ExpressionConstant = Expression.GetDefaultConstant(node, node.ResolvedResult.Item);

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
