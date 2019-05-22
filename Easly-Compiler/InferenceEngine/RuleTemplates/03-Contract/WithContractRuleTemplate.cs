namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IWith"/>.
    /// </summary>
    public interface IWithContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IWith"/>.
    /// </summary>
    public class WithContractRuleTemplate : RuleTemplate<IWith, WithContractRuleTemplate>, IWithContractRuleTemplate
    {
        #region Init
        static WithContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IWith, IRange, IConstantRange>(nameof(IWith.RangeList), nameof(IRange.ResolvedRange)),
                new OnceReferenceSourceTemplate<IWith, IResultType>(nameof(IWith.Instructions) + Dot + nameof(IScope.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IWith, IResultType>(nameof(IWith.ResolvedResult)),
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
            bool Success = true;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IWith node, object data)
        {
            node.ResolvedResult.Item = ResultType.Empty;
        }
        #endregion
    }
}
