namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IQueryOverload"/>.
    /// </summary>
    public interface IQueryOverloadAssociatedTypeConformanceRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQueryOverload"/>.
    /// </summary>
    public class QueryOverloadAssociatedTypeConformanceRuleTemplate : RuleTemplate<IQueryOverload, QueryOverloadAssociatedTypeConformanceRuleTemplate>, IQueryOverloadAssociatedTypeConformanceRuleTemplate
    {
        #region Init
        static QueryOverloadAssociatedTypeConformanceRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IQueryOverload, ICompiledType>(nameof(IQueryOverload.ResolvedAssociatedType) + Dot + nameof(IQueryOverloadType.ConformantResultTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedListDestinationTemplate<IQueryOverload, ICompiledType>(nameof(IQueryOverload.CompleteConformantResultTable)),
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
        public override bool CheckConsistency(IQueryOverload node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IQueryOverload node, object data)
        {
            Debug.Assert(node.ResolvedAssociatedType.IsAssigned);

            IQueryOverloadType OverloadType = node.ResolvedAssociatedType.Item;

            Debug.Assert(OverloadType.ConformantResultTable.IsSealed);
            node.CompleteConformantResultTable.AddRange(OverloadType.ConformantResultTable);
            node.CompleteConformantResultTable.Seal();
        }
        #endregion
    }
}
