namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IAsLongAsInstruction"/>.
    /// </summary>
    public interface IAsLongAsInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAsLongAsInstruction"/>.
    /// </summary>
    public class AsLongAsInstructionContractRuleTemplate : RuleTemplate<IAsLongAsInstruction, AsLongAsInstructionContractRuleTemplate>, IAsLongAsInstructionContractRuleTemplate
    {
        #region Init
        static AsLongAsInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IAsLongAsInstruction, IResultType>(nameof(IAsLongAsInstruction.ContinueCondition) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceCollectionSourceTemplate<IAsLongAsInstruction, IContinuation, IResultType>(nameof(IAsLongAsInstruction.ContinuationList), nameof(IContinuation.ResolvedResult)),
                new ConditionallyAssignedReferenceSourceTemplate<IAsLongAsInstruction, IScope, IResultType>(nameof(IAsLongAsInstruction.ElseInstructions), nameof(IScope.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAsLongAsInstruction, IResultType>(nameof(IAsLongAsInstruction.ResolvedResult)),
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
        public override bool CheckConsistency(IAsLongAsInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IAsLongAsInstruction node, object data)
        {
            node.ResolvedResult.Item = ResultType.Empty;
        }
        #endregion
    }
}
