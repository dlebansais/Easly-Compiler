namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    public interface IDeferredBodyRuleContractTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    /// <typeparam name="TBody">The body type.</typeparam>
    public interface IDeferredBodyRuleContractTemplate<TBody> : IRuleTemplate<TBody, DeferredBodyRuleContractTemplate<TBody>>
        where TBody : IDeferredBody
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    /// <typeparam name="TBody">The body type.</typeparam>
    public class DeferredBodyRuleContractTemplate<TBody> : RuleTemplate<TBody, DeferredBodyRuleContractTemplate<TBody>>, IDeferredBodyRuleContractTemplate<TBody>, IDeferredBodyRuleContractTemplate
        where TBody : IDeferredBody
    {
        #region Init
        static DeferredBodyRuleContractTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<TBody, IList<IAssertion>>(nameof(IDeferredBody.ResolvedRequireList)),
                new OnceReferenceSourceTemplate<TBody, IList<IAssertion>>(nameof(IDeferredBody.ResolvedEnsureList)),
                new OnceReferenceSourceTemplate<TBody, IList<IIdentifier>>(nameof(IDeferredBody.ResolvedExceptionIdentifierList)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<TBody, IList<IExpressionType>>(nameof(IDeferredBody.ResolvedResult)),
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
            node.ResolvedResult.Item = new List<IExpressionType>();
        }
        #endregion
    }
}
