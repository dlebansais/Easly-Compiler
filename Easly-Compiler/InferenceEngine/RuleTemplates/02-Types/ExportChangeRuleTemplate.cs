namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IExportChange"/>.
    /// </summary>
    public interface IExportChangeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IExportChange"/>.
    /// </summary>
    public class ExportChangeRuleTemplate : RuleTemplate<IExportChange, ExportChangeRuleTemplate>, IExportChangeRuleTemplate
    {
        #region Init
        static ExportChangeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IExportChange, string>(nameof(IExportChange.ExportIdentifier) + Dot + nameof(IIdentifier.ValidText)),
                new OnceReferenceCollectionSourceTemplate<IExportChange, IIdentifier, string>(nameof(IExportChange.IdentifierList), nameof(IIdentifier.ValidText)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IExportChange, string>(nameof(IExportChange.ValidExportIdentifier)),
                new UnsealedTableDestinationTemplate<IExportChange, string, IIdentifier>(nameof(IExportChange.IdentifierTable)),
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
        public override bool CheckConsistency(IExportChange node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IHashtableEx<string, IIdentifier> IdentifierTable = new HashtableEx<string, IIdentifier>();

            foreach (IIdentifier Item in node.IdentifierList)
            {
                Debug.Assert(Item.ValidText.IsAssigned);
                string ValidText = Item.ValidText.Item;

                if (IdentifierTable.ContainsKey(ValidText))
                {
                    AddSourceError(new ErrorDuplicateName(Item, ValidText));
                    Success = false;
                }
                else
                    IdentifierTable.Add(ValidText, Item);
            }

            if (Success)
                data = IdentifierTable;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IExportChange node, object data)
        {
            IIdentifier ExportIdentifier = (IIdentifier)node.ExportIdentifier;
            IHashtableEx<string, IIdentifier> IdentifierTable = (IHashtableEx<string, IIdentifier>)data;

            Debug.Assert(ExportIdentifier.ValidText.IsAssigned);
            string ValidText = ExportIdentifier.ValidText.Item;

            node.ValidExportIdentifier.Item = ValidText;

            Debug.Assert(node.IdentifierTable.Count == 0);
            node.IdentifierTable.Merge(IdentifierTable);
            node.IdentifierTable.Seal();
        }
        #endregion
    }
}
