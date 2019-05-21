namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IAttachmentInstruction"/>.
    /// </summary>
    public interface IAttachmentInstructionInitContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAttachmentInstruction"/>.
    /// </summary>
    public class AttachmentInstructionInitContractRuleTemplate : RuleTemplate<IAttachmentInstruction, AttachmentInstructionInitContractRuleTemplate>, IAttachmentInstructionInitContractRuleTemplate
    {
        #region Init
        static AttachmentInstructionInitContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IAttachmentInstruction, IList<IExpressionType>>(nameof(IAttachmentInstruction.Source) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceCollectionSourceTemplate<IAttachmentInstruction, IAttachment, IList<IExpressionType>>(nameof(IAttachmentInstruction.AttachmentList), nameof(IAttachment.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAttachmentInstruction, IList<IExpressionType>>(nameof(IAttachmentInstruction.ResolvedInitResult)),
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
            data = null;
            bool Success = true;

            IExpression AttachmentSource = (IExpression)node.Source;
            IList<IExpressionType> SourceTypeList = AttachmentSource.ResolvedResult.Item;
            IClass EmbeddingClass = node.EmbeddingClass;

            IList<IList<ITypeName>> FullAttachmentTypeNameList = new List<IList<ITypeName>>();
            IList<IList<ICompiledType>> FullAttachmentTypeList = new List<IList<ICompiledType>>();

            if (SourceTypeList.Count < node.EntityNameList.Count)
            {
                AddSourceError(new ErrorInvalidInstruction(node));
                Success = false;
            }
            else
            {
                for (int i = 0; i < node.EntityNameList.Count; i++)
                {
                    IList<ITypeName> AttachmentTypeNameList = new List<ITypeName>();
                    IList<ICompiledType> AttachmentTypeList = new List<ICompiledType>();

                    FullAttachmentTypeNameList.Add(AttachmentTypeNameList);
                    FullAttachmentTypeList.Add(AttachmentTypeList);
                }

                for (int i = 0; i < node.EntityNameList.Count; i++)
                {
                    IExpressionType Item = SourceTypeList[i];
                    IName ItemName = node.EntityNameList[i];
                    IList<ITypeName> AttachmentTypeNameList = FullAttachmentTypeNameList[i];
                    IList<ICompiledType> AttachmentTypeList = FullAttachmentTypeList[i];

                    foreach (IAttachment Attachment in node.AttachmentList)
                    {
                        IObjectType AttachedType = Attachment.AttachTypeList[i];
                        IHashtableEx<string, IScopeAttributeFeature> CheckedScope = Attachment.FullScope;

                        IList<IClass> AssignedSingleClassList = new List<IClass>();
                        IErrorList CheckErrorList = new ErrorList();
                        if (ScopeHolder.HasConflictingSingleAttributes(CheckedScope, node.InnerScopes, AssignedSingleClassList, node, CheckErrorList))
                        {
                            AddSourceErrorList(CheckErrorList);
                            Success = false;
                        }

                        AttachmentTypeNameList.Add(AttachedType.ResolvedTypeName.Item);
                        AttachmentTypeList.Add(AttachedType.ResolvedType.Item);
                    }
                }

                data = new Tuple<IList<IList<ITypeName>>, IList<IList<ICompiledType>>>(FullAttachmentTypeNameList, FullAttachmentTypeList);
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAttachmentInstruction node, object data)
        {
            IExpression AttachmentSource = (IExpression)node.Source;
            IList<IExpressionType> SourceTypeList = AttachmentSource.ResolvedResult.Item;
            IClass EmbeddingClass = node.EmbeddingClass;

            IList<IList<ITypeName>> FullAttachmentTypeNameList = ((Tuple<IList<IList<ITypeName>>, IList<IList<ICompiledType>>>)data).Item1;
            IList<IList<ICompiledType>> FullAttachmentTypeList = ((Tuple<IList<IList<ITypeName>>, IList<IList<ICompiledType>>>)data).Item2;

            for (int i = 0; i < node.EntityNameList.Count; i++)
            {
                IExpressionType Item = SourceTypeList[i];
                IName ItemName = node.EntityNameList[i];
                string ValidText = ItemName.ValidText.Item;
                IList<ITypeName> AttachmentTypeNameList = FullAttachmentTypeNameList[i];
                IList<ICompiledType> AttachmentTypeList = FullAttachmentTypeList[i];

                for (int j = 0; j < node.AttachmentList.Count; j++)
                {
                    IAttachment Attachment = node.AttachmentList[j];
                    ITypeName AttachmentTypeName = AttachmentTypeNameList[j];
                    ICompiledType AttachmentType = AttachmentTypeList[j];

                    IScopeAttributeFeature TypeFixedEntity = Attachment.FullScope[ValidText];
                    TypeFixedEntity.FixFeatureType(AttachmentTypeName, AttachmentType);

                    Attachment.ResolvedLocalEntitiesList.Add(TypeFixedEntity);
                }
            }

            node.ResolvedInitResult.Item = new List<IExpressionType>();
        }
        #endregion
    }
}
