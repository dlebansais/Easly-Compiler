namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IRaiseEventInstruction"/>.
    /// </summary>
    public interface IRaiseEventInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IRaiseEventInstruction"/>.
    /// </summary>
    public class RaiseEventInstructionComputationRuleTemplate : RuleTemplate<IRaiseEventInstruction, RaiseEventInstructionComputationRuleTemplate>, IRaiseEventInstructionComputationRuleTemplate
    {
        #region Init
        static RaiseEventInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IRaiseEventInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IRaiseEventInstruction>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IRaiseEventInstruction, IResultException>(nameof(IRaiseEventInstruction.ResolvedException)),
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
        public override bool CheckConsistency(IRaiseEventInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            // TODO : check associated feature, check type consistency

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IRaiseEventInstruction node, object data)
        {
            node.ResolvedException.Item = new ResultException();
        }
        #endregion
    }
}
