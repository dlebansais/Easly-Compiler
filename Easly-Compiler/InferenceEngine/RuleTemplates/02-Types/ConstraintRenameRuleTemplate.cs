namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IConstraint"/>.
    /// </summary>
    public interface IConstraintRenameRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IConstraint"/>.
    /// </summary>
    public class ConstraintRenameRuleTemplate : RuleTemplate<IConstraint, ConstraintRenameRuleTemplate>, IConstraintRenameRuleTemplate
    {
        #region Init
        static ConstraintRenameRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IConstraint, IRename, string>(nameof(IConstraint.RenameList), nameof(IRename.ValidSourceText)),
                new OnceReferenceCollectionSourceTemplate<IConstraint, IRename, string>(nameof(IConstraint.RenameList), nameof(IRename.ValidDestinationText)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IConstraint, IIdentifier, IIdentifier>(nameof(IConstraint.RenameTable)),
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
        public override bool CheckConsistency(IConstraint node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IHashtableEx<IIdentifier, IIdentifier> RenameTable = new HashtableEx<IIdentifier, IIdentifier>();
            IHashtableEx<string, string> SourceToDestinationTable = new HashtableEx<string, string>();
            IHashtableEx<string, string> DestinationToSourceTable = new HashtableEx<string, string>();

            foreach (IRename Item in node.RenameList)
            {
                IIdentifier SourceIdentifier = (IIdentifier)Item.SourceIdentifier;
                Debug.Assert(SourceIdentifier.ValidText.IsAssigned);
                string ValidSourceIdentifier = SourceIdentifier.ValidText.Item;
                Debug.Assert(Item.ValidSourceText.IsAssigned);
                Debug.Assert(Item.ValidSourceText.Item == ValidSourceIdentifier);

                IIdentifier DestinationIdentifier = (IIdentifier)Item.DestinationIdentifier;
                Debug.Assert(DestinationIdentifier.ValidText.IsAssigned);
                string ValidDestinationIdentifier = DestinationIdentifier.ValidText.Item;
                Debug.Assert(Item.ValidDestinationText.IsAssigned);
                Debug.Assert(Item.ValidDestinationText.Item == ValidDestinationIdentifier);

                if (SourceToDestinationTable.ContainsKey(ValidSourceIdentifier))
                {
                    ErrorList.AddError(new ErrorIdentifierAlreadyListed(SourceIdentifier, ValidSourceIdentifier));
                    Success = false;
                }
                else if (DestinationToSourceTable.ContainsKey(ValidDestinationIdentifier))
                {
                    ErrorList.AddError(new ErrorDoubleRename(Item, DestinationToSourceTable[ValidDestinationIdentifier], ValidDestinationIdentifier));
                    Success = false;
                }
                else
                {
                    SourceToDestinationTable.Add(ValidSourceIdentifier, ValidDestinationIdentifier);
                    DestinationToSourceTable.Add(ValidDestinationIdentifier, ValidSourceIdentifier);
                    RenameTable.Add(SourceIdentifier, DestinationIdentifier);
                }
            }

            if (Success)
                data = RenameTable;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IConstraint node, object data)
        {
            IHashtableEx<IIdentifier, IIdentifier> RenameTable = (IHashtableEx<IIdentifier, IIdentifier>)data;

            Debug.Assert(node.RenameTable.Count == 0);
            node.RenameTable.Merge(RenameTable);
            node.RenameTable.Seal();
        }
        #endregion
    }
}
