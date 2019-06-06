namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IPrecursorExpression"/>.
    /// </summary>
    public interface IPrecursorExpressionConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorExpression"/>.
    /// </summary>
    public class PrecursorExpressionConstantRuleTemplate : RuleTemplate<IPrecursorExpression, PrecursorExpressionConstantRuleTemplate>, IPrecursorExpressionConstantRuleTemplate
    {
        #region Init
        static PrecursorExpressionConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IPrecursorExpression, IExpression>(nameof(IPrecursorExpression.ConstantSourceList)),
                new OnceReferenceCollectionSourceTemplate<IPrecursorExpression, IExpression, ILanguageConstant>(nameof(IPrecursorExpression.ConstantSourceList), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorExpression, ILanguageConstant>(nameof(IPrecursorExpression.ExpressionConstant)),
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

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IPrecursorExpression node, object data)
        {
            ILanguageConstant ExpressionConstant = NeutralLanguageConstant.NotConstant;

            Debug.Assert(node.ConstantSourceList.Count > 0);

            bool IsConstant = true;
            foreach (IExpression ConstantSource in node.ConstantSourceList)
                IsConstant &= ConstantSource.ExpressionConstant.Item != NeutralLanguageConstant.NotConstant;

            if (IsConstant)
            {
                ExpressionConstant = new ObjectLanguageConstant();

                Debug.Assert(node.ResolvedPrecursor.IsAssigned);
                ICompiledFeature FinalFeature = node.ResolvedPrecursor.Item.Feature;

                if (FinalFeature is IConstantFeature AsConstantFeature)
                {
                    Debug.Assert(node.ConstantSourceList.Count == 1);
                    Debug.Assert(node.ConstantSourceList[0].ExpressionConstant.IsAssigned);

                    ExpressionConstant = node.ConstantSourceList[0].ExpressionConstant.Item;
                }
                else
                    ExpressionConstant = Expression.GetDefaultConstant(node, node.ResolvedResult.Item);
            }

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
