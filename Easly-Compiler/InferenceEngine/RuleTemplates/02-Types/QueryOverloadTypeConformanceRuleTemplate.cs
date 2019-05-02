namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IQueryOverloadType"/>.
    /// </summary>
    public interface IQueryOverloadTypeConformanceRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQueryOverloadType"/>.
    /// </summary>
    public class QueryOverloadTypeConformanceRuleTemplate : RuleTemplate<IQueryOverloadType, QueryOverloadTypeConformanceRuleTemplate>, IQueryOverloadTypeConformanceRuleTemplate
    {
        #region Init
        static QueryOverloadTypeConformanceRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IQueryOverloadType, IParameter>(nameof(IQueryOverloadType.ResultTable)),
                new SealedTableCollectionSourceTemplate<IQueryOverloadType, ICompiledType, ITypeName, ICompiledType>(nameof(IQueryOverloadType.ConformantResultTable), nameof(ICompiledType.ConformanceTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedListDestinationTemplate<IQueryOverloadType, ICompiledType>(nameof(IQueryOverloadType.ConformantResultTable)),
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
        public override bool CheckConsistency(IQueryOverloadType node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IQueryOverloadType node, object data)
        {
            node.ConformantResultTable.Seal();
        }
        #endregion
    }
}
