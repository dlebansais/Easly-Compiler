namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IRaiseEventInstruction"/>.
    /// </summary>
    public interface IRaiseEventInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IRaiseEventInstruction"/>.
    /// </summary>
    public class RaiseEventInstructionContractRuleTemplate : RuleTemplate<IRaiseEventInstruction, RaiseEventInstructionContractRuleTemplate>, IRaiseEventInstructionContractRuleTemplate
    {
        #region Init
        static RaiseEventInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IRaiseEventInstruction, IList<IExpressionType>>(nameof(IRaiseEventInstruction.ResolvedResult)),
            };
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// RaiseEvents for errors before applying a rule.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        /// <param name="dataList">Optional data collected during inspection of sources.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        public override bool CheckConsistency(IRaiseEventInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from RaiseEventConsistency().</param>
        public override void Apply(IRaiseEventInstruction node, object data)
        {
            node.ResolvedResult.Item = new List<IExpressionType>();
        }
        #endregion
    }
}
