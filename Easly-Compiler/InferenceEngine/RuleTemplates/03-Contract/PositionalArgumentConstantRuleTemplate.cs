namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IPositionalArgument"/>.
    /// </summary>
    public interface IPositionalArgumentConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPositionalArgument"/>.
    /// </summary>
    public class PositionalArgumentConstantRuleTemplate : RuleTemplate<IPositionalArgument, PositionalArgumentConstantRuleTemplate>, IPositionalArgumentConstantRuleTemplate
    {
        #region Init
        static PositionalArgumentConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IPositionalArgument, IExpression>(nameof(IPositionalArgument.ConstantSourceList)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPositionalArgument, ILanguageConstant>(nameof(IPositionalArgument.ExpressionConstant)),
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
            IExpression SourceExpression = (IExpression)node.Source;

            Debug.Assert(SourceExpression.ExpressionConstant.IsAssigned);
            ILanguageConstant ExpressionConstant = SourceExpression.ExpressionConstant.Item;

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
