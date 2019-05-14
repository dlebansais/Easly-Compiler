namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IForLoopInstruction"/>.
    /// </summary>
    public interface IForLoopInstructionContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IForLoopInstruction"/>.
    /// </summary>
    public class ForLoopInstructionContractRuleTemplate : RuleTemplate<IForLoopInstruction, ForLoopInstructionContractRuleTemplate>, IForLoopInstructionContractRuleTemplate
    {
        #region Init
        static ForLoopInstructionContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IForLoopInstruction, IInstruction, IList<IExpressionType>>(nameof(IForLoopInstruction.InitInstructionList), nameof(IInstruction.ResolvedResult)),
                new OnceReferenceSourceTemplate<IForLoopInstruction, IList<IExpressionType>>(nameof(IForLoopInstruction.WhileCondition) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceCollectionSourceTemplate<IForLoopInstruction, IInstruction, IList<IExpressionType>>(nameof(IForLoopInstruction.LoopInstructionList), nameof(IInstruction.ResolvedResult)),
                new OnceReferenceCollectionSourceTemplate<IForLoopInstruction, IInstruction, IList<IExpressionType>>(nameof(IForLoopInstruction.IterationInstructionList), nameof(IInstruction.ResolvedResult)),
                new OnceReferenceCollectionSourceTemplate<IForLoopInstruction, IAssertion, ITaggedContract>(nameof(IForLoopInstruction.InvariantList), nameof(IAssertion.ResolvedContract)),
                new ConditionallyAssignedReferenceSourceTemplate<IForLoopInstruction, IExpression, IList<IExpressionType>>(nameof(IForLoopInstruction.Variant), nameof(IExpression.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IForLoopInstruction, IList<IExpressionType>>(nameof(IForLoopInstruction.ResolvedResult)),
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
        public override bool CheckConsistency(IForLoopInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IForLoopInstruction node, object data)
        {
            node.ResolvedResult.Item = new List<IExpressionType>();
        }
        #endregion
    }
}
