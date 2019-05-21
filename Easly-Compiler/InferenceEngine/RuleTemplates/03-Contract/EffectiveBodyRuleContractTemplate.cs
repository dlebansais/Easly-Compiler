namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    public interface IEffectiveBodyRuleContractTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    /// <typeparam name="TBody">The body type.</typeparam>
    public interface IEffectiveBodyRuleContractTemplate<TBody> : IRuleTemplate<TBody, EffectiveBodyRuleContractTemplate<TBody>>
        where TBody : IEffectiveBody
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    /// <typeparam name="TBody">The body type.</typeparam>
    public class EffectiveBodyRuleContractTemplate<TBody> : RuleTemplate<TBody, EffectiveBodyRuleContractTemplate<TBody>>, IEffectiveBodyRuleContractTemplate<TBody>, IEffectiveBodyRuleContractTemplate
        where TBody : IEffectiveBody
    {
        #region Init
        static EffectiveBodyRuleContractTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<TBody, IList<IAssertion>>(nameof(IEffectiveBody.ResolvedRequireList)),
                new OnceReferenceSourceTemplate<TBody, IList<IAssertion>>(nameof(IEffectiveBody.ResolvedEnsureList)),
                new OnceReferenceSourceTemplate<TBody, IList<IIdentifier>>(nameof(IEffectiveBody.ResolvedExceptionIdentifierList)),
                new OnceReferenceCollectionSourceTemplate<TBody, IEntityDeclaration, IScopeAttributeFeature>(nameof(IEffectiveBody.EntityDeclarationList), nameof(IEntityDeclaration.ValidEntity)),
                new OnceReferenceCollectionSourceTemplate<TBody, IInstruction, IList<IExpressionType>>(nameof(IEffectiveBody.BodyInstructionList), nameof(IInstruction.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<TBody, IList<IExpressionType>>(nameof(IEffectiveBody.ResolvedResult)),
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
