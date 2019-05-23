namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IRange"/>.
    /// </summary>
    public interface IRangeComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IRange"/>.
    /// </summary>
    public class RangeComputationRuleTemplate : RuleTemplate<IRange, RangeComputationRuleTemplate>, IRangeComputationRuleTemplate
    {
        #region Init
        static RangeComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IRange, IResultException>(nameof(IRange.LeftExpression) + Dot + nameof(IExpression.ResolvedException)),
                new ConditionallyAssignedReferenceSourceTemplate<IRange, IExpression, IResultException>(nameof(IRange.RightExpression), nameof(IExpression.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IRange, IResultException>(nameof(IRange.ResolvedException)),
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
        public override bool CheckConsistency(IRange node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IExpression LeftExpression = (IExpression)node.LeftExpression;

            IResultException ResolvedException = new ResultException();

            ResultException.Merge(ResolvedException, LeftExpression.ResolvedException.Item);

            if (node.RightExpression.IsAssigned)
            {
                IExpression RightExpression = (IExpression)node.RightExpression.Item;
                ResultException.Merge(ResolvedException, RightExpression.ResolvedException.Item);
            }

            data = ResolvedException;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IRange node, object data)
        {
            IResultException ResolvedException = (IResultException)data;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
