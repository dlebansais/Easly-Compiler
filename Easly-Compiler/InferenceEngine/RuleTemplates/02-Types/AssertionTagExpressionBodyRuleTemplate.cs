﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IAssertionTagExpression"/>.
    /// </summary>
    public interface IAssertionTagExpressionBodyRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAssertionTagExpression"/>.
    /// </summary>
    public class AssertionTagExpressionBodyRuleTemplate : RuleTemplate<IAssertionTagExpression, AssertionTagExpressionBodyRuleTemplate>, IAssertionTagExpressionBodyRuleTemplate
    {
        #region Init
        static AssertionTagExpressionBodyRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAssertionTagExpression, IBody>(nameof(IAssertionTagExpression.ResolvedBody)),
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
        public override bool CheckConsistency(IAssertionTagExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IBody ResolvedBody = node.EmbeddingBody;

            if (ResolvedBody == null)
            {
                AddSourceError(new ErrorInvalidExpressionContext(node));
                Success = false;
            }
            else
                data = ResolvedBody;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAssertionTagExpression node, object data)
        {
            IBody ResolvedBody = (IBody)data;

            node.ResolvedBody.Item = ResolvedBody;
        }
        #endregion
    }
}