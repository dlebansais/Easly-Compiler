namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IAttachmentInstruction"/>.
    /// </summary>
    public interface IAttachmentInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAttachmentInstruction"/>.
    /// </summary>
    public class AttachmentInstructionContractRuleTemplate : RuleTemplate<IAttachmentInstruction, AttachmentInstructionContractRuleTemplate>, IAttachmentInstructionContractRuleTemplate
    {
        #region Init
        static AttachmentInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IAttachmentInstruction, IResultType>(nameof(IAttachmentInstruction.ResolvedInitResult)),
                new OnceReferenceCollectionSourceTemplate<IAttachmentInstruction, IAttachment, IResultType>(nameof(IAttachmentInstruction.AttachmentList), nameof(IAttachment.ResolvedResult)),
                new ConditionallyAssignedReferenceSourceTemplate<IAttachmentInstruction, IScope, IResultType>(nameof(IAttachmentInstruction.ElseInstructions), nameof(IScope.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAttachmentInstruction, IResultType>(nameof(IAttachmentInstruction.ResolvedResult)),
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
        public override bool CheckConsistency(IAttachmentInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IAttachmentInstruction node, object data)
        {
            node.ResolvedResult.Item = ResultType.Empty;
        }
        #endregion
    }
}
