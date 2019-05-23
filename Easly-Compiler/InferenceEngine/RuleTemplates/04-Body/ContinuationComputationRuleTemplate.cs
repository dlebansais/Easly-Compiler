namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IContinuation"/>.
    /// </summary>
    public interface IContinuationComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IContinuation"/>.
    /// </summary>
    public class ContinuationComputationRuleTemplate : RuleTemplate<IContinuation, ContinuationComputationRuleTemplate>, IContinuationComputationRuleTemplate
    {
        #region Init
        static ContinuationComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IContinuation, IResultException>(nameof(IContinuation.Instructions) + Dot + nameof(IScope.ResolvedException)),
                new OnceReferenceCollectionSourceTemplate<IContinuation, IInstruction, IResultException>(nameof(IContinuation.CleanupList), nameof(IInstruction.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IContinuation, IResultException>(nameof(IContinuation.ResolvedException)),
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
        public override bool CheckConsistency(IContinuation node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IScope Instructions = (IScope)node.Instructions;

            IResultException ResolvedException = new ResultException();

            ResultException.Merge(ResolvedException, Instructions.ResolvedException.Item);

            foreach (IInstruction Instruction in node.CleanupList)
                ResultException.Merge(ResolvedException, Instruction.ResolvedException.Item);

            data = ResolvedException;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IContinuation node, object data)
        {
            IScope Instructions = (IScope)node.Instructions;

            node.ResolvedException.Item = Instructions.ResolvedException.Item;
        }
        #endregion
    }
}
