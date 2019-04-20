namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IIfThenElseInstruction"/>.
    /// </summary>
    public interface IIfThenElseInstructionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIfThenElseInstruction"/>.
    /// </summary>
    public class IfThenElseInstructionRuleTemplate : RuleTemplate<IIfThenElseInstruction, IfThenElseInstructionRuleTemplate>, IIfThenElseInstructionRuleTemplate
    {
        #region Init
        static IfThenElseInstructionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableCollectionSourceTemplate<IIfThenElseInstruction, IConditional, string, IScopeAttributeFeature>(nameof(IIfThenElseInstruction.ConditionalList), nameof(IConditional.LocalScope)),
                new ConditionallyAssignedSealedTableSourceTemplate<IIfThenElseInstruction, IScope, string, IScopeAttributeFeature>(nameof(IIfThenElseInstruction.ElseInstructions), nameof(IScope.LocalScope)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IIfThenElseInstruction, string, IScopeAttributeFeature>(nameof(IIfThenElseInstruction.LocalScope)),
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
            bool Success = true;
            data = null;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IIfThenElseInstruction node, object data)
        {
            node.LocalScope.Seal();
            node.FullScope.Merge(node.LocalScope);

            ScopeHolder.RecursiveAdd(node.FullScope, node.InnerScopes);

            IList<IScopeHolder> EmbeddingScopeList = ScopeHolder.EmbeddingScope(node);
            EmbeddingScopeList.Add(node);
        }
        #endregion
    }
}
