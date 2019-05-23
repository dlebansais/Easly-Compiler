namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IAttachment"/>.
    /// </summary>
    public interface IAttachmentComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAttachment"/>.
    /// </summary>
    public class AttachmentComputationRuleTemplate : RuleTemplate<IAttachment, AttachmentComputationRuleTemplate>, IAttachmentComputationRuleTemplate
    {
        #region Init
        static AttachmentComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IAttachment, IResultException>(nameof(IAttachment.Instructions) + Dot + nameof(IScope.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAttachment, IResultException>(nameof(IAttachment.ResolvedException)),
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
        public override bool CheckConsistency(IAttachment node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IAttachment node, object data)
        {
            IScope Instructions = (IScope)node.Instructions;

            node.ResolvedException.Item = Instructions.ResolvedException.Item;
        }
        #endregion
    }
}
