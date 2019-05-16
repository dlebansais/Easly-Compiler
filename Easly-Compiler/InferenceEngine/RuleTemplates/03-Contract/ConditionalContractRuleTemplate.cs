namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IConditional"/>.
    /// </summary>
    public interface IConditionalContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IConditional"/>.
    /// </summary>
    public class ConditionalContractRuleTemplate : RuleTemplate<IConditional, ConditionalContractRuleTemplate>, IConditionalContractRuleTemplate
    {
        #region Init
        static ConditionalContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IConditional, IList<IExpressionType>>(nameof(IConditional.BooleanExpression) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceSourceTemplate<IConditional, IList<IExpressionType>>(nameof(IConditional.Instructions) + Dot + nameof(IScope.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IConditional, IList<IExpressionType>>(nameof(IConditional.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IConditional, IList<IIdentifier>>(nameof(IConditional.ResolvedExceptions)),
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
            bool Success = true;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IConditional node, object data)
        {
            node.ResolvedResult.Item = new List<IExpressionType>();
            node.ResolvedExceptions.Item = new List<IIdentifier>();
        }
        #endregion
    }
}
