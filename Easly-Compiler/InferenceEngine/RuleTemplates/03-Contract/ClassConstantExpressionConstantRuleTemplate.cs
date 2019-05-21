namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IClassConstantExpression"/>.
    /// </summary>
    public interface IClassConstantExpressionConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClassConstantExpression"/>.
    /// </summary>
    public class ClassConstantExpressionConstantRuleTemplate : RuleTemplate<IClassConstantExpression, ClassConstantExpressionConstantRuleTemplate>, IClassConstantExpressionConstantRuleTemplate
    {
        #region Init
        static ClassConstantExpressionConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IClassConstantExpression, IExpression>(nameof(IClassConstantExpression.ConstantSourceList)),
                new OnceReferenceCollectionSourceTemplate<IClassConstantExpression, IExpression, ILanguageConstant>(nameof(IClassConstantExpression.ConstantSourceList), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IClassConstantExpression, ILanguageConstant>(nameof(IClassConstantExpression.ExpressionConstant)),
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
        public override bool CheckConsistency(IClassConstantExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IClassConstantExpression node, object data)
        {
            ILanguageConstant ExpressionConstant = NeutralLanguageConstant.NotConstant;

            Debug.Assert(node.ConstantSourceList.Count == 1);
            IExpression ConstantSource = node.ConstantSourceList[0];

            Debug.Assert(ConstantSource.ExpressionConstant.IsAssigned);

            if (node.ResolvedFinalDiscrete.IsAssigned)
                ExpressionConstant = new DiscreteLanguageConstant(node.ResolvedFinalDiscrete.Item);
            else
                ExpressionConstant = ConstantSource.ExpressionConstant.Item;

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
