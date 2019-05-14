namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IOverLoopInstruction"/>.
    /// </summary>
    public interface IOverLoopInstructionInitContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IOverLoopInstruction"/>.
    /// </summary>
    public class OverLoopInstructionInitContractRuleTemplate : RuleTemplate<IOverLoopInstruction, OverLoopInstructionInitContractRuleTemplate>, IOverLoopInstructionInitContractRuleTemplate
    {
        #region Init
        static OverLoopInstructionInitContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IOverLoopInstruction, IList<IExpressionType>>(nameof(IOverLoopInstruction.OverList) + Dot + nameof(IScope.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IOverLoopInstruction, IList<IExpressionType>>(nameof(IOverLoopInstruction.ResolvedInitResult)),
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
        public override bool CheckConsistency(IOverLoopInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IExpression OverList = (IExpression)node.OverList;
            IList<IExpressionType> OverTypeList = OverList.ResolvedResult.Item;
            IClass EmbeddingClass = node.EmbeddingClass;

            IList<ITypeName> IndexTypeNameList = new List<ITypeName>();
            IList<ICompiledType> IndexTypeList = new List<ICompiledType>();

            bool IsOverLoopSourceAvailable = Expression.IsLanguageTypeAvailable(LanguageClasses.OverLoopSource.Guid, node, out ITypeName OverLoopSourceTypeName, out ICompiledType OverLoopSourceType);

            if (OverTypeList.Count < node.IndexerList.Count)
            {
                AddSourceError(new ErrorInvalidInstruction(node));
                Success = false;
            }
            else
            {
                IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();

                foreach (IExpressionType Item in OverTypeList)
                {
                    ICompiledType ResultType = Item.ValueType;
                    bool ConformantToEnumerable = false;
                    bool ConformantToNumericIndexer = false;
                    ITypeName IndexTypeName = null;
                    ICompiledType IndexType = null;

                    List<Error> OverSourceErrorList = new List<Error>();

                    if (ResultType is IClassType AsClassType)
                    {
                        if (IsOverLoopSourceAvailable && ObjectType.TypeConformToBase(ResultType, OverLoopSourceType, SubstitutionTypeTable))
                        {
                            foreach (KeyValuePair<IFeatureName, IFeatureInstance> FeatureItem in ResultType.FeatureTable)
                            {
                                foreach (IPrecursorInstance Precursor in FeatureItem.Value.PrecursorList)
                                {
                                    if (Precursor.Ancestor.BaseClass.ClassGuid == LanguageClasses.OverLoopSource.Guid)
                                    {
                                        if (Precursor.Precursor.Feature.Item is IPropertyFeature AsPropertyAncestor)
                                        {
                                            if (AsPropertyAncestor.ValidFeatureName.Item.Name == "Item")
                                            {
                                                if (FeatureItem.Value.Feature.Item is IPropertyFeature AsPropertyFeature)
                                                {
                                                    ITypeName IndexResultTypeName = AsPropertyFeature.ResolvedEntityTypeName.Item;
                                                    ICompiledType IndexResultType = AsPropertyFeature.ResolvedEntityType.Item;

                                                    IndexResultType.InstanciateType(AsClassType, ref IndexResultTypeName, ref IndexResultType);

                                                    ConformantToEnumerable = true;
                                                    IndexTypeName = IndexResultTypeName;
                                                    IndexType = IndexResultType;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (ConformantToEnumerable)
                                    break;
                            }
                        }

                        if (AsClassType.BaseClass.FeatureTable.ContainsKey(FeatureName.IndexerFeatureName))
                        {
                            IFeatureInstance IndexerInstance = AsClassType.BaseClass.FeatureTable[FeatureName.IndexerFeatureName];
                            IIndexerFeature AsIndexer = IndexerInstance.Feature.Item as IIndexerFeature;
                            Debug.Assert(AsIndexer != null);

                            if (AsIndexer.IndexParameterList.Count == 1)
                            {
                                if (Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, out ITypeName NumberTypeName, out ICompiledType NumberType))
                                {
                                    IEntityDeclaration IndexParameterDeclaration = AsIndexer.IndexParameterList[0];
                                    ICompiledType IndexParameterType = IndexParameterDeclaration.ValidEntity.Item.ResolvedFeatureType.Item;

                                    if (IndexParameterType is IClassType AsClassIndexType)
                                    {
                                        if (AsClassIndexType == NumberType)
                                        {
                                            ITypeName IndexResultTypeName = AsIndexer.ResolvedEntityTypeName.Item;
                                            ICompiledType IndexResultType = AsIndexer.ResolvedEntityType.Item;

                                            IndexResultType.InstanciateType(AsClassType, ref IndexResultTypeName, ref IndexResultType);

                                            ConformantToNumericIndexer = true;
                                            IndexTypeName = IndexResultTypeName;
                                            IndexType = IndexResultType;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (ConformantToEnumerable && ConformantToNumericIndexer)
                    {
                        AddSourceError(new ErrorInvalidOverSourceType(OverList));
                        Success = false;
                    }
                    else if (!ConformantToEnumerable && !ConformantToNumericIndexer)
                    {
                        AddSourceError(new ErrorInvalidOverSourceType(OverList));
                        Success = false;
                    }
                    else
                    {
                        IndexTypeNameList.Add(IndexTypeName);
                        IndexTypeList.Add(IndexType);
                    }
                }
            }

            if (Success)
                data = new Tuple<IList<ITypeName>, IList<ICompiledType>>(IndexTypeNameList, IndexTypeList);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IOverLoopInstruction node, object data)
        {
            IExpression OverList = (IExpression)node.OverList;
            IList<ITypeName> IndexTypeNameList = ((Tuple<IList<ITypeName>, IList<ICompiledType>>)data).Item1;
            IList<ICompiledType> IndexTypeList = ((Tuple<IList<ITypeName>, IList<ICompiledType>>)data).Item2;

            for (int i = 0; i < node.IndexerList.Count; i++)
            {
                ITypeName ItemTypeName = IndexTypeNameList[i];
                ICompiledType ItemType = IndexTypeList[i];
                IName ItemName = node.IndexerList[i];
                string ValidText = ItemName.ValidText.Item;

                IScopeAttributeFeature TypeFixedEntity = node.InnerLoopScope[ValidText];
                TypeFixedEntity.FixFeatureType(ItemTypeName, ItemType);
            }

            node.ResolvedResult.Item = new List<IExpressionType>();
        }
        #endregion
    }
}
