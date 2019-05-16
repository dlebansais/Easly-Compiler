namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IIndexAssignmentInstruction"/>.
    /// </summary>
    public interface IIndexAssignmentInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIndexAssignmentInstruction"/>.
    /// </summary>
    public class IndexAssignmentInstructionContractRuleTemplate : RuleTemplate<IIndexAssignmentInstruction, IndexAssignmentInstructionContractRuleTemplate>, IIndexAssignmentInstructionContractRuleTemplate
    {
        #region Init
        static IndexAssignmentInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IIndexAssignmentInstruction, IArgument, IList<IExpressionType>>(nameof(IIndexAssignmentInstruction.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIndexAssignmentInstruction, IList<IExpressionType>>(nameof(IIndexAssignmentInstruction.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IIndexAssignmentInstruction, IList<IIdentifier>>(nameof(IIndexAssignmentInstruction.ResolvedExceptions)),
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
        public override bool CheckConsistency(IIndexAssignmentInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IIndexAssignmentInstruction node, object data)
        {
            node.ResolvedResult.Item = new List<IExpressionType>();
            node.ResolvedExceptions.Item = new List<IIdentifier>();
        }
        #endregion
    }
}
