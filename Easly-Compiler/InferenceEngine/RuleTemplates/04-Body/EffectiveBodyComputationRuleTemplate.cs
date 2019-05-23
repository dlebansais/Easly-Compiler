namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IEffectiveBody"/>.
    /// </summary>
    public interface IEffectiveBodyComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IEffectiveBody"/>.
    /// </summary>
    /// <typeparam name="T">The effective body type.</typeparam>
    public class EffectiveBodyComputationRuleTemplate<T> : RuleTemplate<T, EffectiveBodyComputationRuleTemplate<T>>, IEffectiveBodyComputationRuleTemplate
        where T : IEffectiveBody
    {
        #region Init
        static EffectiveBodyComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IEffectiveBody, IInstruction, IResultException>(nameof(IEffectiveBody.BodyInstructionList), nameof(IInstruction.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IEffectiveBody, IList<IInstruction>>(nameof(IEffectiveBody.ResolvedInstructionList)),
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
        public override bool CheckConsistency(T node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(T node, object data)
        {
            node.ResolvedInstructionList.Item = new List<IInstruction>(node.BodyInstructionList);
        }
        #endregion
    }
}
