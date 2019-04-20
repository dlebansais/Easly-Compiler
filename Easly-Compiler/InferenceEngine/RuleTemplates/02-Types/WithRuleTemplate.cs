namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IWith"/>.
    /// </summary>
    public interface IWithRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IWith"/>.
    /// </summary>
    public class WithRuleTemplate : RuleTemplate<IWith, WithRuleTemplate>, IWithRuleTemplate
    {
        #region Init
        static WithRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IWith, string, IScopeAttributeFeature>(nameof(IWith.Instructions) + Dot + nameof(IScope.LocalScope)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IWith, string, IScopeAttributeFeature>(nameof(IWith.LocalScope)),
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
        public override bool CheckConsistency(IWith node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IWith node, object data)
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
