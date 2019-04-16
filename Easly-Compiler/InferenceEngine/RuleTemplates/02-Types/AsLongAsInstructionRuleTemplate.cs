namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IAsLongAsInstruction"/>.
    /// </summary>
    public interface IAsLongAsInstructionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAsLongAsInstruction"/>.
    /// </summary>
    public class AsLongAsInstructionRuleTemplate : RuleTemplate<IAsLongAsInstruction, AsLongAsInstructionRuleTemplate>, IAsLongAsInstructionRuleTemplate
    {
        #region Init
        static AsLongAsInstructionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableCollectionSourceTemplate<IAsLongAsInstruction, IContinuation, string, IScopeAttributeFeature>(nameof(IAsLongAsInstruction.ContinuationList), nameof(IContinuation.LocalScope)),
                new ConditionallyAssignedSealedTableSourceTemplate<IAsLongAsInstruction, IScope, string, IScopeAttributeFeature>(nameof(IAsLongAsInstruction.ElseInstructions), nameof(IContinuation.LocalScope)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IAsLongAsInstruction, string, IScopeAttributeFeature>(nameof(IAsLongAsInstruction.LocalScope)),
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
            bool Success = true;
            data = null;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAsLongAsInstruction node, object data)
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
