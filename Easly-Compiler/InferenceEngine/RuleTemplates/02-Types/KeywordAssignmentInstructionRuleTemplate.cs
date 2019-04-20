﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IKeywordAssignmentInstruction"/>.
    /// </summary>
    public interface IKeywordAssignmentInstructionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IKeywordAssignmentInstruction"/>.
    /// </summary>
    public class KeywordAssignmentInstructionRuleTemplate : RuleTemplate<IKeywordAssignmentInstruction, KeywordAssignmentInstructionRuleTemplate>, IKeywordAssignmentInstructionRuleTemplate
    {
        #region Init
        static KeywordAssignmentInstructionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IKeywordAssignmentInstruction, string, IScopeAttributeFeature>(nameof(IKeywordAssignmentInstruction.LocalScope)),
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
        public override bool CheckConsistency(IKeywordAssignmentInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IKeywordAssignmentInstruction node, object data)
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
