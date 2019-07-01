namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IBinaryConditionalExpression"/>.
    /// </summary>
    public interface IBinaryConditionalExpressionConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBinaryConditionalExpression"/>.
    /// </summary>
    public class BinaryConditionalExpressionConstantRuleTemplate : RuleTemplate<IBinaryConditionalExpression, BinaryConditionalExpressionConstantRuleTemplate>, IBinaryConditionalExpressionConstantRuleTemplate
    {
        #region Init
        static BinaryConditionalExpressionConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IBinaryConditionalExpression, IExpression>(nameof(IBinaryConditionalExpression.ConstantSourceList)),
                new OnceReferenceCollectionSourceTemplate<IBinaryConditionalExpression, IExpression, ILanguageConstant>(nameof(IBinaryConditionalExpression.ConstantSourceList), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IBinaryConditionalExpression, ILanguageConstant>(nameof(IBinaryConditionalExpression.ExpressionConstant)),
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
        public override bool CheckConsistency(IBinaryConditionalExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IBinaryConditionalExpression node, object data)
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

            if (LeftExpressionConstant != NeutralLanguageConstant.NotConstant && RightExpressionConstant != NeutralLanguageConstant.NotConstant)
            {
                IBooleanLanguageConstant LeftConstant = LeftExpressionConstant as IBooleanLanguageConstant;
                Debug.Assert(LeftConstant != null);
                IBooleanLanguageConstant RightConstant = RightExpressionConstant as IBooleanLanguageConstant;
                Debug.Assert(RightConstant != null);

                bool? LeftConstantValue = LeftConstant.Value;
                bool? RightConstantValue = RightConstant.Value;

                if (LeftConstantValue.HasValue && RightConstantValue.HasValue)
                {
                    switch (node.Conditional)
                    {
                        case BaseNode.ConditionalTypes.And:
                            ExpressionConstant = new BooleanLanguageConstant(LeftConstantValue.Value && RightConstantValue.Value);
                            break;

                        case BaseNode.ConditionalTypes.Or:
                            ExpressionConstant = new BooleanLanguageConstant(LeftConstantValue.Value || RightConstantValue.Value);
                            break;

                        case BaseNode.ConditionalTypes.Xor:
                            ExpressionConstant = new BooleanLanguageConstant(LeftConstantValue.Value ^ RightConstantValue.Value);
                            break;

                        case BaseNode.ConditionalTypes.Implies:
                            ExpressionConstant = new BooleanLanguageConstant(!LeftConstantValue.Value || RightConstantValue.Value);
                            break;
                    }

                    Debug.Assert(ExpressionConstant is IBooleanLanguageConstant);
                }
                else
                    ExpressionConstant = new BooleanLanguageConstant();
            }

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
