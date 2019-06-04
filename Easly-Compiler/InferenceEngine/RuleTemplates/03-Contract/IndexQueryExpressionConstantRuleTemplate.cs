namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IIndexQueryExpression"/>.
    /// </summary>
    public interface IIndexQueryExpressionConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIndexQueryExpression"/>.
    /// </summary>
    public class IndexQueryExpressionConstantRuleTemplate : RuleTemplate<IIndexQueryExpression, IndexQueryExpressionConstantRuleTemplate>, IIndexQueryExpressionConstantRuleTemplate
    {
        #region Init
        static IndexQueryExpressionConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IIndexQueryExpression, IExpression>(nameof(IIndexQueryExpression.ConstantSourceList)),
                new OnceReferenceCollectionSourceTemplate<IIndexQueryExpression, IExpression, ILanguageConstant>(nameof(IIndexQueryExpression.ConstantSourceList), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIndexQueryExpression, ILanguageConstant>(nameof(IIndexQueryExpression.ExpressionConstant)),
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

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IIndexQueryExpression node, object data)
        {
            ILanguageConstant ExpressionConstant = NeutralLanguageConstant.NotConstant;

            Debug.Assert(node.ConstantSourceList.Count > 0);

            bool IsConstant = true;
            foreach (IExpression ConstantSource in node.ConstantSourceList)
                IsConstant &= ConstantSource.ExpressionConstant.Item != NeutralLanguageConstant.NotConstant;

            if (IsConstant)
                ExpressionConstant = Expression.GetDefaultConstant(node);

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
