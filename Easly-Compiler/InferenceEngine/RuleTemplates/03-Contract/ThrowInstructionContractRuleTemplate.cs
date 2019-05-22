namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IThrowInstruction"/>.
    /// </summary>
    public interface IThrowInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IThrowInstruction"/>.
    /// </summary>
    public class ThrowInstructionContractRuleTemplate : RuleTemplate<IThrowInstruction, ThrowInstructionContractRuleTemplate>, IThrowInstructionContractRuleTemplate
    {
        #region Init
        static ThrowInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IThrowInstruction, IArgument, IResultType>(nameof(IThrowInstruction.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IThrowInstruction, IResultType>(nameof(IThrowInstruction.ResolvedResult)),
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
        public override bool CheckConsistency(IThrowInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IThrowInstruction node, object data)
        {
            node.ResolvedResult.Item = ResultType.Empty;
        }
        #endregion
    }
}
