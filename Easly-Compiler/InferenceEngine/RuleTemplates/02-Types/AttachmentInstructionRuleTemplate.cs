namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IAttachmentInstruction"/>.
    /// </summary>
    public interface IAttachmentInstructionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAttachmentInstruction"/>.
    /// </summary>
    public class AttachmentInstructionRuleTemplate : RuleTemplate<IAttachmentInstruction, AttachmentInstructionRuleTemplate>, IAttachmentInstructionRuleTemplate
    {
        #region Init
        static AttachmentInstructionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableCollectionSourceTemplate<IAttachmentInstruction, IAttachment, string, IScopeAttributeFeature>(nameof(IAttachmentInstruction.AttachmentList), nameof(IAttachment.LocalScope)),
                new ConditionallyAssignedSealedTableSourceTemplate<IAttachmentInstruction, IScope, string, IScopeAttributeFeature>(nameof(IAttachmentInstruction.ElseInstructions), nameof(IScope.LocalScope)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IAttachmentInstruction, string, IScopeAttributeFeature>(nameof(IAttachmentInstruction.LocalScope)),
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
        public override bool CheckConsistency(IAttachmentInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IClass EmbeddingClass = node.EmbeddingClass;

            IList<IHashtableEx<string, IScopeAttributeFeature>> CheckedScopeList = new List<IHashtableEx<string, IScopeAttributeFeature>>();

            for (int i = 0; i < node.AttachmentList.Count; i++)
                CheckedScopeList.Add(new HashtableEx<string, IScopeAttributeFeature>());

            for (int Index = 0; Index < node.EntityNameList.Count; Index++)
            {
                IName Item = node.EntityNameList[Index];

                Debug.Assert(Item.ValidText.IsAssigned);
                string ValidText = Item.ValidText.Item;

                if (CheckedScopeList[0].ContainsKey(ValidText))
                {
                    AddSourceError(new ErrorVariableAlreadyDefined(Item, ValidText));
                    Success = false;
                    continue;
                }

                for (int i = 0; i < node.AttachmentList.Count; i++)
                {
                    Attachment AttachmentItem = (Attachment)node.AttachmentList[i];
                    IHashtableEx<string, IScopeAttributeFeature> CheckedScope = CheckedScopeList[i];

                    Debug.Assert(Index < AttachmentItem.AttachTypeList.Count);
                    IObjectType AttachedType = AttachmentItem.AttachTypeList[Index];

                    Debug.Assert(AttachedType.ResolvedTypeName.IsAssigned);
                    Debug.Assert(AttachedType.ResolvedType.IsAssigned);

                    //IScopeAttributeFeature NewEntity = ScopeAttributeFeature.Create(Item, ValidText);
                    IScopeAttributeFeature NewEntity = ScopeAttributeFeature.Create(Item, ValidText, AttachedType.ResolvedTypeName.Item, AttachedType.ResolvedType.Item);
                    CheckedScope.Add(ValidText, NewEntity);
                }
            }

            IList<string> ConflictList = new List<string>();
            for (int i = 0; i < node.AttachmentList.Count; i++)
            {
                IAttachment AttachmentItem = (Attachment)node.AttachmentList[i];
                IHashtableEx<string, IScopeAttributeFeature> CheckedScope = CheckedScopeList[i];
                ScopeHolder.RecursiveCheck(CheckedScope, AttachmentItem.InnerScopes, ConflictList);
            }

            foreach (IName Item in node.EntityNameList)
            {
                Debug.Assert(Item.ValidText.IsAssigned);
                string ValidText = Item.ValidText.Item;

                if (ConflictList.Contains(ValidText))
                {
                    AddSourceError(new ErrorVariableAlreadyDefined(Item, ValidText));
                    Success = false;
                }
            }

            if (Success)
                data = CheckedScopeList;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAttachmentInstruction node, object data)
        {
            IList<IHashtableEx<string, IScopeAttributeFeature>> CheckedScopeList = (IList<IHashtableEx<string, IScopeAttributeFeature>>)data;
            Debug.Assert(CheckedScopeList.Count == node.AttachmentList.Count);

            node.LocalScope.Seal();

            for (int i = 0; i < node.AttachmentList.Count; i++)
            {
                IAttachment AttachmentItem = node.AttachmentList[i];
                IHashtableEx<string, IScopeAttributeFeature> CheckedScope = CheckedScopeList[i];
                AttachmentItem.FullScope.Merge(CheckedScope);
                ScopeHolder.RecursiveAdd(CheckedScope, AttachmentItem.InnerScopes);
            }

            IList<IScopeHolder> EmbeddingScopeList = ScopeHolder.EmbeddingScope(node);
            EmbeddingScopeList.Add(node);
        }
        #endregion
    }
}
