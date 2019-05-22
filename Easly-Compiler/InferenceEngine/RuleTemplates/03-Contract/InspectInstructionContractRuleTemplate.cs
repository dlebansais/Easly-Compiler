namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IInspectInstruction"/>.
    /// </summary>
    public interface IInspectInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IInspectInstruction"/>.
    /// </summary>
    public class InspectInstructionContractRuleTemplate : RuleTemplate<IInspectInstruction, InspectInstructionContractRuleTemplate>, IInspectInstructionContractRuleTemplate
    {
        #region Init
        static InspectInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IInspectInstruction, IResultType>(nameof(IInspectInstruction.Source) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceCollectionSourceTemplate<IInspectInstruction, IWith, IResultType>(nameof(IInspectInstruction.WithList), nameof(IWith.ResolvedResult)),
                new ConditionallyAssignedReferenceSourceTemplate<IInspectInstruction, IScope, IResultType>(nameof(IInspectInstruction.ElseInstructions), nameof(IScope.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IInspectInstruction, IResultType>(nameof(IInspectInstruction.ResolvedResult)),
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
        public override bool CheckConsistency(IInspectInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IInspectInstruction node, object data)
        {
            node.ResolvedResult.Item = ResultType.Empty;
        }
        #endregion
    }
}
