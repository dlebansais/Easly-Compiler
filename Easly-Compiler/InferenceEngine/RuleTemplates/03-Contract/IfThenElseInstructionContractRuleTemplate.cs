namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IIfThenElseInstruction"/>.
    /// </summary>
    public interface IIfThenElseInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIfThenElseInstruction"/>.
    /// </summary>
    public class IfThenElseInstructionContractRuleTemplate : RuleTemplate<IIfThenElseInstruction, IfThenElseInstructionContractRuleTemplate>, IIfThenElseInstructionContractRuleTemplate
    {
        #region Init
        static IfThenElseInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IIfThenElseInstruction, IConditional, IList<IExpressionType>>(nameof(IIfThenElseInstruction.ConditionalList), nameof(IConditional.ResolvedResult)),
                new ConditionallyAssignedReferenceSourceTemplate<IIfThenElseInstruction, IScope, IList<IExpressionType>>(nameof(IIfThenElseInstruction.ElseInstructions), nameof(IScope.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIfThenElseInstruction, IList<IExpressionType>>(nameof(IIfThenElseInstruction.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IIfThenElseInstruction, IList<IIdentifier>>(nameof(IIfThenElseInstruction.ResolvedExceptions)),
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
        public override bool CheckConsistency(IIfThenElseInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IIfThenElseInstruction node, object data)
        {
            node.ResolvedResult.Item = new List<IExpressionType>();
            node.ResolvedExceptions.Item = new List<IIdentifier>();
        }
        #endregion
    }
}
