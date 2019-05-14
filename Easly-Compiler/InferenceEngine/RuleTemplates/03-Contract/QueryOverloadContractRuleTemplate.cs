namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IQueryOverload"/>.
    /// </summary>
    public interface IQueryOverloadContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQueryOverload"/>.
    /// </summary>
    public class QueryOverloadContractRuleTemplate : RuleTemplate<IQueryOverload, QueryOverloadContractRuleTemplate>, IQueryOverloadContractRuleTemplate
    {
        #region Init
        static QueryOverloadContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IQueryOverload, IList<IAssertion>>(nameof(IQueryOverload.QueryBody) + Dot + nameof(IBody.ResolvedRequireList)),
                new OnceReferenceSourceTemplate<IQueryOverload, IList<IAssertion>>(nameof(IQueryOverload.QueryBody) + Dot + nameof(IBody.ResolvedEnsureList)),
                new OnceReferenceSourceTemplate<IQueryOverload, IList<IIdentifier>>(nameof(IQueryOverload.QueryBody) + Dot + nameof(IBody.ResolvedExceptionIdentifierList)),
                new OnceReferenceSourceTemplate<IQueryOverload, IList<IExpressionType>>(nameof(IQueryOverload.QueryBody) + Dot + nameof(IBody.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IQueryOverload, ICompiledBody>(nameof(IQueryOverload.ResolvedBody)),
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
        public override bool CheckConsistency(IQueryOverload node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IQueryOverload node, object data)
        {
            node.ResolvedBody.Item = (ICompiledBody)node.QueryBody;
        }
        #endregion
    }
}
