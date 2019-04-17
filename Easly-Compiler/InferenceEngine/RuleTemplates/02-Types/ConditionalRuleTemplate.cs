namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IConditional"/>.
    /// </summary>
    public interface IConditionalRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IConditional"/>.
    /// </summary>
    public class ConditionalRuleTemplate : RuleTemplate<IConditional, ConditionalRuleTemplate>, IConditionalRuleTemplate
    {
        #region Init
        static ConditionalRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IConditional, string, IScopeAttributeFeature>(nameof(IConditional.Instructions) + Dot + nameof(IScope.LocalScope)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IConditional, string, IScopeAttributeFeature>(nameof(IConditional.LocalScope)),
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
        public override bool CheckConsistency(IConditional node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IConditional node, object data)
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
