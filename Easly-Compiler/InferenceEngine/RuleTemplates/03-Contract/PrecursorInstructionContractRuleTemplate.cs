namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IPrecursorInstruction"/>.
    /// </summary>
    public interface IPrecursorInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorInstruction"/>.
    /// </summary>
    public class PrecursorInstructionContractRuleTemplate : RuleTemplate<IPrecursorInstruction, PrecursorInstructionContractRuleTemplate>, IPrecursorInstructionContractRuleTemplate
    {
        #region Init
        static PrecursorInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IPrecursorInstruction, IArgument, IList<IExpressionType>>(nameof(IPrecursorInstruction.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorInstruction, IList<IExpressionType>>(nameof(IPrecursorInstruction.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IPrecursorInstruction, IList<IIdentifier>>(nameof(IPrecursorInstruction.ResolvedExceptions)),
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
        public override bool CheckConsistency(IPrecursorInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IPrecursorInstruction node, object data)
        {
            node.ResolvedResult.Item = new List<IExpressionType>();
            node.ResolvedExceptions.Item = new List<IIdentifier>();
        }
        #endregion
    }
}
