namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IQueryExpression"/>.
    /// </summary>
    public interface IQueryExpressionConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQueryExpression"/>.
    /// </summary>
    public class QueryExpressionConstantRuleTemplate : RuleTemplate<IQueryExpression, QueryExpressionConstantRuleTemplate>, IQueryExpressionConstantRuleTemplate
    {
        #region Init
        static QueryExpressionConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IQueryExpression, IExpression>(nameof(IQueryExpression.ConstantSourceList)),
                new OnceReferenceCollectionSourceTemplate<IQueryExpression, IExpression, ILanguageConstant>(nameof(IQueryExpression.ConstantSourceList), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IQueryExpression, ILanguageConstant>(nameof(IQueryExpression.ExpressionConstant)),
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
        public override bool CheckConsistency(IQueryExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IQueryExpression node, object data)
        {
            ILanguageConstant ExpressionConstant = NeutralLanguageConstant.NotConstant;

            if (node.ConstantSourceList.Count > 0)
            {
                Debug.Assert(node.ConstantSourceList.Count == 1);
                IExpression ConstantSource = node.ConstantSourceList[0];

                Debug.Assert(ConstantSource.ExpressionConstant.IsAssigned);
                ExpressionConstant = ConstantSource.ExpressionConstant.Item;

                node.ExpressionConstant.Item = ExpressionConstant;
            }
            else
            {
                Debug.Assert(node.ExpressionConstant.IsAssigned);
                Debug.Assert(node.ExpressionConstant.Item is IDiscreteLanguageConstant AsDiscreteLanguageConstant);
            }
        }
        #endregion
    }
}
