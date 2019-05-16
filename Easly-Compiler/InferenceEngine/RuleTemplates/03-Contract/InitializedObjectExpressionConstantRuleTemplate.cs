namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IInitializedObjectExpression"/>.
    /// </summary>
    public interface IInitializedObjectExpressionConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IInitializedObjectExpression"/>.
    /// </summary>
    public class InitializedObjectExpressionConstantRuleTemplate : RuleTemplate<IInitializedObjectExpression, InitializedObjectExpressionConstantRuleTemplate>, IInitializedObjectExpressionConstantRuleTemplate
    {
        #region Init
        static InitializedObjectExpressionConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IInitializedObjectExpression, IExpression>(nameof(IInitializedObjectExpression.ConstantSourceList)),
                new OnceReferenceCollectionSourceTemplate<IInitializedObjectExpression, IAssignmentArgument, ILanguageConstant>(nameof(IInitializedObjectExpression.AssignmentList), nameof(IAssignmentArgument.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IInitializedObjectExpression, ILanguageConstant>(nameof(IInitializedObjectExpression.ExpressionConstant)),
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
        public override bool CheckConsistency(IInitializedObjectExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            foreach (IAssignmentArgument AssignmentItem in node.AssignmentList)
            {
                Debug.Assert(AssignmentItem.ExpressionConstant.IsAssigned);
                ILanguageConstant ExpressionConstant = AssignmentItem.ExpressionConstant.Item;

                if (ExpressionConstant == NeutralLanguageConstant.NotConstant)
                {
                    AddSourceError(new ErrorConstantExpected(AssignmentItem));
                    Success = false;
                }
            }

            if (Success)
            {
                //TODO: create the constant
                data = NeutralLanguageConstant.NotConstant;
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IInitializedObjectExpression node, object data)
        {
            ILanguageConstant ExpressionConstant = (ILanguageConstant)data;

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
