namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IWith"/>.
    /// </summary>
    public interface IWithComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IWith"/>.
    /// </summary>
    public class WithComputationRuleTemplate : RuleTemplate<IWith, WithComputationRuleTemplate>, IWithComputationRuleTemplate
    {
        #region Init
        static WithComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IWith, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IWith>.Default),
                new OnceReferenceCollectionSourceTemplate<IWith, IRange, IResultException>(nameof(IWith.RangeList), nameof(IRange.ResolvedException)),
                new OnceReferenceSourceTemplate<IWith, IResultException>(nameof(IWith.Instructions) + Dot + nameof(IScope.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IWith, IResultException>(nameof(IWith.ResolvedException)),
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

            IScope Instructions = (IScope)node.Instructions;

            // This is enforced when the root node is validated.
            Debug.Assert(node.RangeList.Count > 0);

            IResultException ResolvedException = new ResultException();

            foreach (IRange Item in node.RangeList)
                ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

            ResultException.Merge(ResolvedException, Instructions.ResolvedException.Item);

            data = ResolvedException;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IWith node, object data)
        {
            IResultException ResolvedException = (IResultException)data;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
