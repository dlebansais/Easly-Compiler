namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IReleaseInstruction"/>.
    /// </summary>
    public interface IReleaseInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IReleaseInstruction"/>.
    /// </summary>
    public class ReleaseInstructionComputationRuleTemplate : RuleTemplate<IReleaseInstruction, ReleaseInstructionComputationRuleTemplate>, IReleaseInstructionComputationRuleTemplate
    {
        #region Init
        static ReleaseInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceTableSourceTemplate<IReleaseInstruction, string, IScopeAttributeFeature, ITypeName>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureTypeName), TemplateNodeStart<IReleaseInstruction>.Default),
                new OnceReferenceTableSourceTemplate<IReleaseInstruction, string, IScopeAttributeFeature, ICompiledType>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureType2), TemplateNodeStart<IReleaseInstruction>.Default),
                new SealedTableSourceTemplate<IReleaseInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IReleaseInstruction>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IReleaseInstruction, IResultException>(nameof(IReleaseInstruction.ResolvedException)),
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
        public override bool CheckConsistency(IReleaseInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IReleaseInstruction node, object data)
        {
            node.ResolvedException.Item = new ResultException();
        }
        #endregion
    }
}
