namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IContinuation"/>.
    /// </summary>
    public interface IContinuationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IContinuation"/>.
    /// </summary>
    public class ContinuationRuleTemplate : RuleTemplate<IContinuation, ContinuationRuleTemplate>, IContinuationRuleTemplate
    {
        #region Init
        static ContinuationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IContinuation, string, IScopeAttributeFeature>(nameof(IContinuation.Instructions) + Dot + nameof(IScope.LocalScope)),
                new SealedTableCollectionSourceTemplate<IContinuation, IInstruction, string, IScopeAttributeFeature>(nameof(IContinuation.CleanupList), nameof(IInstruction.LocalScope)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IContinuation, string, IScopeAttributeFeature>(nameof(IContinuation.LocalScope)),
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
            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IContinuation node, object data)
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
