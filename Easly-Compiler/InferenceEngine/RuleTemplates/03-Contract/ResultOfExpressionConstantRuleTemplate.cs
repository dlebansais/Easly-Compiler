namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IResultOfExpression"/>.
    /// </summary>
    public interface IResultOfExpressionConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IResultOfExpression"/>.
    /// </summary>
    public class ResultOfExpressionConstantRuleTemplate : RuleTemplate<IResultOfExpression, ResultOfExpressionConstantRuleTemplate>, IResultOfExpressionConstantRuleTemplate
    {
        #region Init
        static ResultOfExpressionConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IResultOfExpression, IExpression>(nameof(IResultOfExpression.ConstantSourceList)),
                new OnceReferenceCollectionSourceTemplate<IResultOfExpression, IExpression, ILanguageConstant>(nameof(IResultOfExpression.ConstantSourceList), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IResultOfExpression, ILanguageConstant>(nameof(IResultOfExpression.ExpressionConstant)),
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
        public override bool CheckConsistency(IResultOfExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IResultOfExpression node, object data)
        {
            ILanguageConstant ExpressionConstant = NeutralLanguageConstant.NotConstant;

            Debug.Assert(node.ConstantSourceList.Count == 1);
            IExpression ResultSource = node.ConstantSourceList[0];

            Debug.Assert(ResultSource.ExpressionConstant.IsAssigned);
            ExpressionConstant = ResultSource.ExpressionConstant.Item;

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
