namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IAssignmentInstruction"/>.
    /// </summary>
    public interface IAssignmentInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAssignmentInstruction"/>.
    /// </summary>
    public class AssignmentInstructionContractRuleTemplate : RuleTemplate<IAssignmentInstruction, AssignmentInstructionContractRuleTemplate>, IAssignmentInstructionContractRuleTemplate
    {
        #region Init
        static AssignmentInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IAssignmentInstruction, IList<IExpressionType>>(nameof(IAssignmentInstruction.Source) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceSourceTemplate<IAssignmentInstruction, IList<IIdentifier>>(nameof(IAssignmentInstruction.Source) + Dot + nameof(IExpression.ResolvedExceptions)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAssignmentInstruction, IList<IExpressionType>>(nameof(IAssignmentInstruction.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IAssignmentInstruction, IList<IIdentifier>>(nameof(IAssignmentInstruction.ResolvedExceptions)),
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
        public override bool CheckConsistency(IAssignmentInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IAssignmentInstruction node, object data)
        {
            IExpression Source = (IExpression)node.Source;
            node.ResolvedResult.Item = Source.ResolvedResult.Item;
            node.ResolvedExceptions.Item = Source.ResolvedExceptions.Item;
        }
        #endregion
    }
}
