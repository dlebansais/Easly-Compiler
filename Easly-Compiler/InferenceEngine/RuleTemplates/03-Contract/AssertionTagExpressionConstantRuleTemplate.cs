namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IAssertionTagExpression"/>.
    /// </summary>
    public interface IAssertionTagExpressionConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAssertionTagExpression"/>.
    /// </summary>
    public class AssertionTagExpressionConstantRuleTemplate : RuleTemplate<IAssertionTagExpression, AssertionTagExpressionConstantRuleTemplate>, IAssertionTagExpressionConstantRuleTemplate
    {
        #region Init
        static AssertionTagExpressionConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IAssertionTagExpression, IExpression>(nameof(IAssertionTagExpression.ConstantSourceList)),
                new OnceReferenceCollectionSourceTemplate<IAssertionTagExpression, IExpression, ILanguageConstant>(nameof(IAssertionTagExpression.ConstantSourceList), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAssertionTagExpression, ILanguageConstant>(nameof(IAssertionTagExpression.ExpressionConstant)),
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
        public override bool CheckConsistency(IAssertionTagExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IAssertionTagExpression node, object data)
        {
            Debug.Assert(node.ConstantSourceList.Count == 1);
            IExpression ConstantSource = node.ConstantSourceList[0];

            Debug.Assert(ConstantSource.ExpressionConstant.IsAssigned);
            ILanguageConstant ExpressionConstant = ConstantSource.ExpressionConstant.Item;

            Debug.Assert(ExpressionConstant == NeutralLanguageConstant.NotConstant || ExpressionConstant is IBooleanLanguageConstant);

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
