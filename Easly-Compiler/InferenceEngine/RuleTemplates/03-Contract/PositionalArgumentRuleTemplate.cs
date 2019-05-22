namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IPositionalArgument"/>.
    /// </summary>
    public interface IPositionalArgumentRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPositionalArgument"/>.
    /// </summary>
    public class PositionalArgumentRuleTemplate : RuleTemplate<IPositionalArgument, PositionalArgumentRuleTemplate>, IPositionalArgumentRuleTemplate
    {
        #region Init
        static PositionalArgumentRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IPositionalArgument, IResultType>(nameof(IPositionalArgument.Source) + Dot + nameof(IExpression.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPositionalArgument, IResultType>(nameof(IPositionalArgument.ResolvedResult)),
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
        public override bool CheckConsistency(IPositionalArgument node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IPositionalArgument node, object data)
        {
            IExpression Source = (IExpression)node.Source;
            node.ResolvedResult.Item = Source.ResolvedResult.Item;
            node.ConstantSourceList.Add(Source);
            node.ConstantSourceList.Seal();
        }
        #endregion
    }
}
