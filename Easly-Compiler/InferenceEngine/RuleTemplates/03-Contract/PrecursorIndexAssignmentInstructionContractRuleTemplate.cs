namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexAssignmentInstruction"/>.
    /// </summary>
    public interface IPrecursorIndexAssignmentInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexAssignmentInstruction"/>.
    /// </summary>
    public class PrecursorIndexAssignmentInstructionContractRuleTemplate : RuleTemplate<IPrecursorIndexAssignmentInstruction, PrecursorIndexAssignmentInstructionContractRuleTemplate>, IPrecursorIndexAssignmentInstructionContractRuleTemplate
    {
        #region Init
        static PrecursorIndexAssignmentInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IPrecursorIndexAssignmentInstruction, IArgument, IList<IExpressionType>>(nameof(IPrecursorIndexAssignmentInstruction.ArgumentList), nameof(IArgument.ResolvedResult)),
                new OnceReferenceSourceTemplate<IPrecursorIndexAssignmentInstruction, IList<IExpressionType>>(nameof(IPrecursorIndexAssignmentInstruction.Source) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceSourceTemplate<IPrecursorIndexAssignmentInstruction, IList<IIdentifier>>(nameof(IPrecursorIndexAssignmentInstruction.Source) + Dot + nameof(IExpression.ResolvedExceptions)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorIndexAssignmentInstruction, IList<IExpressionType>>(nameof(IPrecursorIndexAssignmentInstruction.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IPrecursorIndexAssignmentInstruction, IList<IIdentifier>>(nameof(IPrecursorIndexAssignmentInstruction.ResolvedExceptions)),
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
        public override bool CheckConsistency(IPrecursorIndexAssignmentInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IPrecursorIndexAssignmentInstruction node, object data)
        {
            node.ResolvedResult.Item = new List<IExpressionType>();
            node.ResolvedExceptions.Item = new List<IIdentifier>();
        }
        #endregion
    }
}
