namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IUnaryNotExpression"/>.
    /// </summary>
    public interface IUnaryNotExpressionConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IUnaryNotExpression"/>.
    /// </summary>
    public class UnaryNotExpressionConstantRuleTemplate : RuleTemplate<IUnaryNotExpression, UnaryNotExpressionConstantRuleTemplate>, IUnaryNotExpressionConstantRuleTemplate
    {
        #region Init
        static UnaryNotExpressionConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IUnaryNotExpression, IExpression>(nameof(IUnaryNotExpression.ConstantSourceList)),
                new OnceReferenceCollectionSourceTemplate<IUnaryNotExpression, IExpression, ILanguageConstant>(nameof(IUnaryNotExpression.ConstantSourceList), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IUnaryNotExpression, ILanguageConstant>(nameof(IUnaryNotExpression.ExpressionConstant)),
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
        public override bool CheckConsistency(IUnaryNotExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IUnaryNotExpression node, object data)
        {
            ILanguageConstant ExpressionConstant = NeutralLanguageConstant.NotConstant;

            Debug.Assert(node.ConstantSourceList.Count == 1);

            IExpression RightConstantSource = node.ConstantSourceList[0];
            Debug.Assert(RightConstantSource == node.RightExpression);

            Debug.Assert(RightConstantSource.ExpressionConstant.IsAssigned);
            ILanguageConstant RightExpressionConstant = RightConstantSource.ExpressionConstant.Item;

            if (RightExpressionConstant != NeutralLanguageConstant.NotConstant)
            {
                IBooleanLanguageConstant RightConstant = RightExpressionConstant as IBooleanLanguageConstant;
                Debug.Assert(RightConstant != null);

                bool? RightConstantValue = RightConstant.Value;

                if (RightConstantValue.HasValue)
                    ExpressionConstant = new BooleanLanguageConstant(!RightConstantValue.Value);
                else
                    ExpressionConstant = new BooleanLanguageConstant();
            }

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
