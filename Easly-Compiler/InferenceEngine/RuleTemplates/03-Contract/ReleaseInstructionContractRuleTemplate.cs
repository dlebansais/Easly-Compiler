namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IReleaseInstruction"/>.
    /// </summary>
    public interface IReleaseInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IReleaseInstruction"/>.
    /// </summary>
    public class ReleaseInstructionContractRuleTemplate : RuleTemplate<IReleaseInstruction, ReleaseInstructionContractRuleTemplate>, IReleaseInstructionContractRuleTemplate
    {
        #region Init
        static ReleaseInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IReleaseInstruction, IList<IExpressionType>>(nameof(IReleaseInstruction.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IReleaseInstruction, IList<IIdentifier>>(nameof(IReleaseInstruction.ResolvedExceptions)),
            };
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Releases for errors before applying a rule.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        /// <param name="dataList">Optional data collected during inspection of sources.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        public override bool CheckConsistency(IReleaseInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from ReleaseConsistency().</param>
        public override void Apply(IReleaseInstruction node, object data)
        {
            node.ResolvedResult.Item = new List<IExpressionType>();
            node.ResolvedExceptions.Item = new List<IIdentifier>();
        }
        #endregion
    }
}
