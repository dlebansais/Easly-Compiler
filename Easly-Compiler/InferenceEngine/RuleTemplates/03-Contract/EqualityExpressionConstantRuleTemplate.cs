namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IEqualityExpression"/>.
    /// </summary>
    public interface IEqualityExpressionConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IEqualityExpression"/>.
    /// </summary>
    public class EqualityExpressionConstantRuleTemplate : RuleTemplate<IEqualityExpression, EqualityExpressionConstantRuleTemplate>, IEqualityExpressionConstantRuleTemplate
    {
        #region Init
        static EqualityExpressionConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IEqualityExpression, IExpression>(nameof(IEqualityExpression.ConstantSourceList)),
                new OnceReferenceCollectionSourceTemplate<IEqualityExpression, IExpression, ILanguageConstant>(nameof(IEqualityExpression.ConstantSourceList), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IEqualityExpression, ILanguageConstant>(nameof(IEqualityExpression.ExpressionConstant)),
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
        public override bool CheckConsistency(IEqualityExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IEqualityExpression node, object data)
        {
            ILanguageConstant ExpressionConstant = NeutralLanguageConstant.NotConstant;

            Debug.Assert(node.ConstantSourceList.Count == 2);

            IExpression LeftConstantSource = node.ConstantSourceList[0];
            Debug.Assert(LeftConstantSource == node.LeftExpression);

            IExpression RightConstantSource = node.ConstantSourceList[1];
            Debug.Assert(RightConstantSource == node.RightExpression);

            Debug.Assert(LeftConstantSource.ExpressionConstant.IsAssigned);
            ILanguageConstant LeftExpressionConstant = LeftConstantSource.ExpressionConstant.Item;

            Debug.Assert(RightConstantSource.ExpressionConstant.IsAssigned);
            ILanguageConstant RightExpressionConstant = RightConstantSource.ExpressionConstant.Item;

            if (node.Comparison == BaseNode.ComparisonType.Different)
            {

            }

            if (LeftExpressionConstant != NeutralLanguageConstant.NotConstant && RightExpressionConstant != NeutralLanguageConstant.NotConstant)
            {
                if (LeftExpressionConstant.IsCompatibleWith(RightExpressionConstant) && LeftExpressionConstant.IsValueKnown && RightExpressionConstant.IsValueKnown)
                {
                    switch (node.Comparison)
                    {
                        case BaseNode.ComparisonType.Equal:
                            ExpressionConstant = new BooleanLanguageConstant(LeftExpressionConstant.IsConstantEqual(RightExpressionConstant));
                            break;

                        case BaseNode.ComparisonType.Different:
                            ExpressionConstant = new BooleanLanguageConstant(!LeftExpressionConstant.IsConstantEqual(RightExpressionConstant));
                            break;
                    }

                    IBooleanLanguageConstant BooleanLanguageConstant = ExpressionConstant as IBooleanLanguageConstant;
                    Debug.Assert(BooleanLanguageConstant != null);
                    Debug.Assert(BooleanLanguageConstant.IsValueKnown);
                }
                else
                    ExpressionConstant = new BooleanLanguageConstant();
            }

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
