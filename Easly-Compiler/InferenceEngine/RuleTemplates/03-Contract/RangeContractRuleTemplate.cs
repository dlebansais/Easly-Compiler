namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IRange"/>.
    /// </summary>
    public interface IRangeContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IRange"/>.
    /// </summary>
    public class RangeContractRuleTemplate : RuleTemplate<IRange, RangeContractRuleTemplate>, IRangeContractRuleTemplate
    {
        #region Init
        static RangeContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IRange, IResultType>(nameof(IRange.LeftExpression) + Dot + nameof(IExpression.ResolvedResult)),
                new ConditionallyAssignedReferenceSourceTemplate<IRange, IExpression, IResultType>(nameof(IRange.RightExpression), nameof(IExpression.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IRange, IConstantRange>(nameof(IRange.ResolvedRange)),
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

            if (!ConstantRange.TryParseRange(node, out IConstantRange Result, out IError Error))
            {
                AddSourceError(Error);
                Success = false;
            }
            else
                data = Result;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IRange node, object data)
        {
            IConstantRange ResolvedRange = (IConstantRange)data;

            node.ResolvedRange.Item = ResolvedRange;
        }
        #endregion
    }
}
