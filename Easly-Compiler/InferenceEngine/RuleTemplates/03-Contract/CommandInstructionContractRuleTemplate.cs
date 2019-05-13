namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="ICommandInstruction"/>.
    /// </summary>
    public interface ICommandInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ICommandInstruction"/>.
    /// </summary>
    public class CommandInstructionContractRuleTemplate : RuleTemplate<ICommandInstruction, CommandInstructionContractRuleTemplate>, ICommandInstructionContractRuleTemplate
    {
        #region Init
        static CommandInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<ICommandInstruction, IArgument, IList<IExpressionType>>(nameof(ICommandInstruction.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<ICommandInstruction, IList<IExpressionType>>(nameof(ICommandInstruction.ResolvedResult)),
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
        public override bool CheckConsistency(ICommandInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(ICommandInstruction node, object data)
        {
            node.ResolvedResult.Item = new List<IExpressionType>();
        }
        #endregion
    }
}
