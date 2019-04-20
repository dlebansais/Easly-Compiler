namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IInspectInstruction"/>.
    /// </summary>
    public interface IInspectInstructionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IInspectInstruction"/>.
    /// </summary>
    public class InspectInstructionRuleTemplate : RuleTemplate<IInspectInstruction, InspectInstructionRuleTemplate>, IInspectInstructionRuleTemplate
    {
        #region Init
        static InspectInstructionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableCollectionSourceTemplate<IInspectInstruction, IWith, string, IScopeAttributeFeature>(nameof(IInspectInstruction.WithList), nameof(IWith.LocalScope)),
                new ConditionallyAssignedSealedTableSourceTemplate<IInspectInstruction, IScope, string, IScopeAttributeFeature>(nameof(IInspectInstruction.ElseInstructions), nameof(IScope.LocalScope)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IInspectInstruction, string, IScopeAttributeFeature>(nameof(IInspectInstruction.LocalScope)),
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
            bool Success = true;
            data = null;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IInspectInstruction node, object data)
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
