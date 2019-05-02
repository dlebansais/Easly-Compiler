namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IRename"/>.
    /// </summary>
    public interface IRenameRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IRename"/>.
    /// </summary>
    public class RenameRuleTemplate : RuleTemplate<IRename, RenameRuleTemplate>, IRenameRuleTemplate
    {
        #region Init
        static RenameRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IRename, string>(nameof(IRename.SourceIdentifier) + Dot + nameof(IIdentifier.ValidText)),
                new OnceReferenceSourceTemplate<IRename, string>(nameof(IRename.DestinationIdentifier) + Dot + nameof(IIdentifier.ValidText)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IRename, string>(nameof(IRename.ValidSourceText)),
                new OnceReferenceDestinationTemplate<IRename, string>(nameof(IRename.ValidDestinationText)),
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
        public override bool CheckConsistency(IRename node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IIdentifier SourceIdentifier = (IIdentifier)node.SourceIdentifier;
            IIdentifier DestinationIdentifier = (IIdentifier)node.DestinationIdentifier;

            Debug.Assert(SourceIdentifier.ValidText.IsAssigned);
            Debug.Assert(DestinationIdentifier.ValidText.IsAssigned);

            if (SourceIdentifier.ValidText.Item == DestinationIdentifier.ValidText.Item)
            {
                ErrorList.AddError(new ErrorNameUnchanged(node, SourceIdentifier.ValidText.Item));
                Success = false;
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IRename node, object data)
        {
            IIdentifier SourceIdentifier = (IIdentifier)node.SourceIdentifier;
            IIdentifier DestinationIdentifier = (IIdentifier)node.DestinationIdentifier;

            Debug.Assert(SourceIdentifier.ValidText.IsAssigned);
            node.ValidSourceText.Item = ((IIdentifier)node.SourceIdentifier).ValidText.Item;

            Debug.Assert(DestinationIdentifier.ValidText.IsAssigned);
            node.ValidDestinationText.Item = ((IIdentifier)node.DestinationIdentifier).ValidText.Item;
        }
        #endregion
    }
}
