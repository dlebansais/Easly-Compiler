namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IIfThenElseInstruction"/>.
    /// </summary>
    public interface IIfThenElseInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIfThenElseInstruction"/>.
    /// </summary>
    public class IfThenElseInstructionComputationRuleTemplate : RuleTemplate<IIfThenElseInstruction, IfThenElseInstructionComputationRuleTemplate>, IIfThenElseInstructionComputationRuleTemplate
    {
        #region Init
        static IfThenElseInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IIfThenElseInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IIfThenElseInstruction>.Default),
                new OnceReferenceCollectionSourceTemplate<IIfThenElseInstruction, IConditional, IResultException>(nameof(IIfThenElseInstruction.ConditionalList), nameof(IConditional.ResolvedException)),
                new ConditionallyAssignedReferenceSourceTemplate<IIfThenElseInstruction, IScope, IResultException>(nameof(IIfThenElseInstruction.ElseInstructions), nameof(IScope.ResolvedException))
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIfThenElseInstruction, IResultException>(nameof(IIfThenElseInstruction.ResolvedException)),
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

            IResultException ResolvedException = new ResultException();

            foreach (IConditional Item in node.ConditionalList)
                ResultException.Merge(ResolvedException, Item.ResolvedException);

            if (node.ElseInstructions.IsAssigned)
            {
                IScope ElseInstructions = (IScope)node.ElseInstructions.Item;
                ResultException.Merge(ResolvedException, ElseInstructions.ResolvedException);
            }

            data = ResolvedException;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IIfThenElseInstruction node, object data)
        {
            IResultException ResolvedException = (IResultException)data;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
