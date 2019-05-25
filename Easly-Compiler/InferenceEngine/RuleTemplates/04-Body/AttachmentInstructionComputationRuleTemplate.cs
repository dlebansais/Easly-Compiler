namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IAttachmentInstruction"/>.
    /// </summary>
    public interface IAttachmentInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAttachmentInstruction"/>.
    /// </summary>
    public class AttachmentInstructionComputationRuleTemplate : RuleTemplate<IAttachmentInstruction, AttachmentInstructionComputationRuleTemplate>, IAttachmentInstructionComputationRuleTemplate
    {
        #region Init
        static AttachmentInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IAttachmentInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IAttachmentInstruction>.Default),
                new OnceReferenceSourceTemplate<IAttachmentInstruction, IResultException>(nameof(IAttachmentInstruction.Source) + Dot + nameof(IExpression.ResolvedException)),
                new OnceReferenceCollectionSourceTemplate<IAttachmentInstruction, IAttachment, IResultException>(nameof(IAttachmentInstruction.AttachmentList), nameof(IAttachment.ResolvedException)),
                new ConditionallyAssignedReferenceSourceTemplate<IAttachmentInstruction, IScope, IResultException>(nameof(IAttachmentInstruction.ElseInstructions), nameof(IScope.ResolvedException))
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAttachmentInstruction, IResultException>(nameof(IAttachmentInstruction.ResolvedException)),
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

            IExpression SourceExpression = (IExpression)node.Source;
            IResultType SourceResult = SourceExpression.ResolvedResult.Item;
            IClass EmbeddingClass = node.EmbeddingClass;

            int MaxAttachmentCount = 0;
            foreach (IAttachment AttachmentItem in node.AttachmentList)
                if (AttachmentItem.AttachTypeList.Count > SourceResult.Count)
                {
                    AddSourceError(new ErrorInvalidAttachment(AttachmentItem));
                    return false;
                }
                else if (MaxAttachmentCount < AttachmentItem.AttachTypeList.Count)
                    MaxAttachmentCount = AttachmentItem.AttachTypeList.Count;

            bool ConformanceError = false;
            for (int i = 0; i < MaxAttachmentCount; i++)
            {
                ICompiledType SourceType = SourceResult.At(i).ValueType;

                if (SourceType == ClassType.ClassAnyType)
                    ConformanceError |= CheckConsistencyAny(node, i);
                else
                    ConformanceError |= CheckConsistencyTyped(node, i, SourceType);
            }

            if (ConformanceError)
                return false;

            if (Success)
            {
                IResultException ResolvedException = new ResultException();

                ResultException.Merge(ResolvedException, SourceExpression.ResolvedException.Item);

                foreach (IAttachment Item in node.AttachmentList)
                    ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                if (node.ElseInstructions.IsAssigned)
                {
                    IScope ElseInstructions = (IScope)node.ElseInstructions.Item;
                    ResultException.Merge(ResolvedException, ElseInstructions.ResolvedException.Item);
                }

                data = ResolvedException;
            }

            return Success;
        }

        private bool CheckConsistencyAny(IAttachmentInstruction node, int index)
        {
            bool ConformanceError = false;
            bool AttachmentToAny = false;
            BaseNode.CopySemantic AttachmentType = BaseNode.CopySemantic.Any;

            IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();

            foreach (IAttachment AttachmentItem in node.AttachmentList)
                if (index < AttachmentItem.AttachTypeList.Count)
                {
                    IObjectType AttachType = AttachmentItem.AttachTypeList[index];
                    ICompiledType DestinationType = AttachType.ResolvedType.Item;

                    if (DestinationType.IsReference)
                    {
                        if (AttachmentType == BaseNode.CopySemantic.Value || AttachmentToAny)
                        {
                            AddSourceError(new ErrorInvalidAttachment(AttachmentItem));
                            ConformanceError = true;
                        }
                        else if (AttachmentType == BaseNode.CopySemantic.Any)
                            AttachmentType = BaseNode.CopySemantic.Reference;
                    }
                    else if (DestinationType.IsValue)
                    {
                        if (AttachmentType == BaseNode.CopySemantic.Reference || AttachmentToAny)
                        {
                            AddSourceError(new ErrorInvalidAttachment(AttachmentItem));
                            ConformanceError = true;
                        }
                        else if (AttachmentType == BaseNode.CopySemantic.Any)
                            AttachmentType = BaseNode.CopySemantic.Value;
                    }
                    else if (AttachmentToAny)
                    {
                        AddSourceError(new ErrorInvalidAttachment(AttachmentItem));
                        ConformanceError = true;
                    }
                    else if (AttachmentType != BaseNode.CopySemantic.Any)
                    {
                        AddSourceError(new ErrorInvalidAttachment(AttachmentItem));
                        ConformanceError = true;
                    }
                    else
                        AttachmentToAny = true;
                }

            if (!ConformanceError && !AttachmentToAny)
            {
                for (int j = 0; j < node.AttachmentList.Count; j++)
                {
                    IAttachment AttachmentItem = node.AttachmentList[j];
                    if (index < AttachmentItem.AttachTypeList.Count)
                    {
                        IObjectType AttachType = AttachmentItem.AttachTypeList[index];
                        ICompiledType DestinationType = AttachType.ResolvedType.Item;

                        for (int k = 0; k < j; k++)
                        {
                            IAttachment PreviousAttachmentItem = node.AttachmentList[k];
                            IObjectType PreviousAttachType = PreviousAttachmentItem.AttachTypeList[index];
                            ICompiledType PreviousDestinationType = PreviousAttachType.ResolvedType.Item;

                            if (ObjectType.TypeConformToBase(DestinationType, PreviousDestinationType, SubstitutionTypeTable))
                            {
                                AddSourceError(new ErrorInvalidAttachment(AttachmentItem));
                                ConformanceError = true;
                            }
                        }
                    }
                }
            }

            return ConformanceError;
        }

        private bool CheckConsistencyTyped(IAttachmentInstruction node, int index, ICompiledType sourceType)
        {
            bool ConformanceError = false;
            IClass EmbeddingClass = node.EmbeddingClass;
            IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();

            for (int j = 0; j < node.AttachmentList.Count; j++)
            {
                IAttachment AttachmentItem = node.AttachmentList[j];
                if (index < AttachmentItem.AttachTypeList.Count)
                {
                    IObjectType AttachType = AttachmentItem.AttachTypeList[index];
                    ICompiledType DestinationType = AttachType.ResolvedType.Item;

                    if (!ObjectType.TypesHaveCommonDescendant(EmbeddingClass, DestinationType, sourceType, SubstitutionTypeTable))
                    {
                        AddSourceError(new ErrorInvalidAttachment(AttachmentItem));
                        ConformanceError = true;
                    }
                    else
                        for (int k = 0; k < j; k++)
                        {
                            IAttachment PreviousAttachmentItem = node.AttachmentList[k];
                            IObjectType PreviousAttachType = PreviousAttachmentItem.AttachTypeList[index];
                            ICompiledType PreviousDestinationType = PreviousAttachType.ResolvedType.Item;

                            if (ObjectType.TypeConformToBase(DestinationType, PreviousDestinationType, SubstitutionTypeTable))
                            {
                                AddSourceError(new ErrorInvalidAttachment(AttachmentItem));
                                ConformanceError = true;
                            }
                        }
                }
            }

            return ConformanceError;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAttachmentInstruction node, object data)
        {
            IResultException ResolvedException = (IResultException)data;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
