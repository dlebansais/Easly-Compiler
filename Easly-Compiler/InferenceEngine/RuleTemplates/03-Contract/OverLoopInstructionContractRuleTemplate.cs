namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IOverLoopInstruction"/>.
    /// </summary>
    public interface IOverLoopInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IOverLoopInstruction"/>.
    /// </summary>
    public class OverLoopInstructionContractRuleTemplate : RuleTemplate<IOverLoopInstruction, OverLoopInstructionContractRuleTemplate>, IOverLoopInstructionContractRuleTemplate
    {
        #region Init
        static OverLoopInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IOverLoopInstruction, IList<IExpressionType>>(nameof(IOverLoopInstruction.ResolvedInitResult)),
                new OnceReferenceSourceTemplate<IOverLoopInstruction, IList<IExpressionType>>(nameof(IOverLoopInstruction.LoopInstructions) + Dot + nameof(IScope.ResolvedResult)),
                new OnceReferenceCollectionSourceTemplate<IOverLoopInstruction, IAssertion, ITaggedContract>(nameof(IOverLoopInstruction.InvariantList), nameof(IAssertion.ResolvedContract)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IOverLoopInstruction, IList<IExpressionType>>(nameof(IOverLoopInstruction.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IOverLoopInstruction, IList<IIdentifier>>(nameof(IOverLoopInstruction.ResolvedExceptions)),
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
        public override bool CheckConsistency(IOverLoopInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IOverLoopInstruction node, object data)
        {
            node.ResolvedResult.Item = new List<IExpressionType>();
            node.ResolvedExceptions.Item = new List<IIdentifier>();
        }
        #endregion
    }
}
