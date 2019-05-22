namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IAssignmentArgument"/>.
    /// </summary>
    public interface IAssignmentArgumentComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAssignmentArgument"/>.
    /// </summary>
    public class AssignmentArgumentComputationRuleTemplate : RuleTemplate<IAssignmentArgument, AssignmentArgumentComputationRuleTemplate>, IAssignmentArgumentComputationRuleTemplate
    {
        #region Init
        static AssignmentArgumentComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IAssignmentArgument, IResultException>(nameof(IAssignmentArgument.Source) + Dot + nameof(IExpression.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAssignmentArgument, IResultException>(nameof(IAssignmentArgument.ResolvedException)),
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
            node.ResolvedException.Item = Source.ResolvedException.Item;
        }
        #endregion
    }
}
