namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    public interface IExternBodyRuleContractTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    /// <typeparam name="TBody">The body type.</typeparam>
    public interface IExternBodyRuleContractTemplate<TBody> : IRuleTemplate<TBody, ExternBodyRuleContractTemplate<TBody>>
        where TBody : IExternBody
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    /// <typeparam name="TBody">The body type.</typeparam>
    public class ExternBodyRuleContractTemplate<TBody> : RuleTemplate<TBody, ExternBodyRuleContractTemplate<TBody>>, IExternBodyRuleContractTemplate<TBody>, IExternBodyRuleContractTemplate
        where TBody : IExternBody
    {
        #region Init
        static ExternBodyRuleContractTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<TBody, IList<IAssertion>>(nameof(IExternBody.ResolvedRequireList)),
                new OnceReferenceSourceTemplate<TBody, IList<IAssertion>>(nameof(IExternBody.ResolvedEnsureList)),
                new OnceReferenceSourceTemplate<TBody, IList<IIdentifier>>(nameof(IExternBody.ResolvedExceptionIdentifierList)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<TBody, IResultType>(nameof(IExternBody.ResolvedResult)),
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
        public override bool CheckConsistency(TBody node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(TBody node, object data)
        {
            node.ResolvedResult.Item = ResultType.Empty;
        }
        #endregion
    }
}
