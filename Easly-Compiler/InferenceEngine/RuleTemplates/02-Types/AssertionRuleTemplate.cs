namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IAssertion"/>.
    /// </summary>
    public interface IAssertionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAssertion"/>.
    /// </summary>
    public class AssertionRuleTemplate : RuleTemplate<IAssertion, AssertionRuleTemplate>, IAssertionRuleTemplate
    {
        #region Init
        static AssertionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
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
        public override bool CheckConsistency(IAssertion node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAssertion node, object data)
        {
#if COVERAGE
            Debug.Assert(node.AdditionalScope.Count == 0);
            Debug.Assert(node.InnerScopes.Count == 0);
#endif
        }
        #endregion
    }
}
