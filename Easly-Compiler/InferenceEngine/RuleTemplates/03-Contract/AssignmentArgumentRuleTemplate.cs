namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IAssignmentArgument"/>.
    /// </summary>
    public interface IAssignmentArgumentRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAssignmentArgument"/>.
    /// </summary>
    public class AssignmentArgumentRuleTemplate : RuleTemplate<IAssignmentArgument, AssignmentArgumentRuleTemplate>, IAssignmentArgumentRuleTemplate
    {
        #region Init
        static AssignmentArgumentRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IAssignmentArgument, IList<IExpressionType>>(nameof(IAssignmentArgument.Source) + Dot + nameof(IExpression.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAssignmentArgument, IList<IExpressionType>>(nameof(IAssignmentArgument.ResolvedResult)),
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
            IExpression Source = (IExpression)node.Source;
            node.ResolvedResult.Item = Source.ResolvedResult.Item;
        }
        #endregion
    }
}
