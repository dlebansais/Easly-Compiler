namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IAssignmentArgument"/>.
    /// </summary>
    public interface IAssignmentArgumentConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAssignmentArgument"/>.
    /// </summary>
    public class AssignmentArgumentConstantRuleTemplate : RuleTemplate<IAssignmentArgument, AssignmentArgumentConstantRuleTemplate>, IAssignmentArgumentConstantRuleTemplate
    {
        #region Init
        static AssignmentArgumentConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IAssignmentArgument, IExpression, ILanguageConstant>(nameof(IAssignmentArgument.ConstantSourceList)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAssignmentArgument, IList<IExpressionType>>(nameof(IAssignmentArgument.ExpressionConstant)),
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
        public override bool CheckConsistency(IAssignmentArgument node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IAssignmentArgument node, object data)
        {
            IExpression SourceExpression = (IExpression)node.Source;

            Debug.Assert(SourceExpression.ExpressionConstant.IsAssigned);
            ILanguageConstant ExpressionConstant = SourceExpression.ExpressionConstant.Item;

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
