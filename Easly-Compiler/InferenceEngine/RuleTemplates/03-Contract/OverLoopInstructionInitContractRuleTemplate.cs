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
                new OnceReferenceSourceTemplate<IOverLoopInstruction, IResultType>(nameof(IOverLoopInstruction.OverList) + Dot + nameof(IScope.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IOverLoopInstruction, IResultType>(nameof(IOverLoopInstruction.ResolvedInitResult)),
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
            IResultType OverTypeList = OverList.ResolvedResult.Item;
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
                foreach (IExpressionType Item in OverTypeList)
                {
                    ICompiledType ResultType = Item.ValueType;
                    bool IsConformantToEnumerable = false;
                    bool IsConformantToNumericIndexer = false;
                    ITypeName IndexTypeName = null;
                    ICompiledType IndexType = null;

                    List<Error> OverSourceErrorList = new List<Error>();

                    if (ResultType is IClassType AsClassType)
                    {
                        if (IsOverLoopSourceAvailable && ObjectType.TypeConformToBase(ResultType, OverLoopSourceType, isConversionAllowed: true))
                            IsConformantToEnumerable = FindSourceItemPrecursor(AsClassType, ResultType.FeatureTable, out IndexTypeName, out IndexType);

                        if (AsClassType.BaseClass.FeatureTable.ContainsKey(FeatureName.IndexerFeatureName))
                        {
                            IFeatureInstance IndexerInstance = AsClassType.BaseClass.FeatureTable[FeatureName.IndexerFeatureName];
                            IsConformantToNumericIndexer = FindIndexer(AsClassType, node, IndexerInstance, out IndexTypeName, out IndexType);
                        }
                    }

                    if (IsConformantToEnumerable && IsConformantToNumericIndexer)
                    {
                        AddSourceError(new ErrorInvalidOverSourceType(OverList));
                        Success = false;
                    }
                    else if (!IsConformantToEnumerable && !IsConformantToNumericIndexer)
                    {
                        AddSourceError(new ErrorMissingOverSourceAndIndexer(OverList));
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

        private bool FindSourceItemPrecursor(IClassType instancingClassType, ISealableDictionary<IFeatureName, IFeatureInstance> featureTable, out ITypeName indexTypeName, out ICompiledType indexType)
        {
            bool IsConformantToEnumerable = false;
            indexTypeName = null;
            indexType = null;

            foreach (KeyValuePair<IFeatureName, IFeatureInstance> Entry in featureTable)
                IsConformantToEnumerable |= FindSourceItemPrecursor(instancingClassType, Entry.Value, ref indexTypeName, ref indexType);

            return IsConformantToEnumerable;
        }

        private bool FindSourceItemPrecursor(IClassType instancingClassType, IFeatureInstance featureInstance, ref ITypeName indexTypeName, ref ICompiledType indexType)
        {
            bool IsConformantToEnumerable = false;

            foreach (IPrecursorInstance PrecursorInstance in featureInstance.PrecursorList)
                IsConformantToEnumerable |= FindSourceItemPrecursor(instancingClassType, featureInstance, PrecursorInstance, ref indexTypeName, ref indexType);

            return IsConformantToEnumerable;
        }

        private bool FindSourceItemPrecursor(IClassType instancingClassType, IFeatureInstance featureInstance, IPrecursorInstance precursorInstance, ref ITypeName indexTypeName, ref ICompiledType indexType)
        {
            bool IsConformantToEnumerable = false;

            if (precursorInstance.Ancestor.BaseClass.ClassGuid == LanguageClasses.OverLoopSource.Guid)
                if (precursorInstance.Precursor.Feature is IPropertyFeature AsPropertyAncestor)
                    if (AsPropertyAncestor.ValidFeatureName.Item.Name == "Item")
                        if (featureInstance.Feature is IPropertyFeature AsPropertyFeature)
                        {
                            Debug.Assert(indexTypeName == null);
                            Debug.Assert(indexType == null);

                            ITypeName IndexResultTypeName = AsPropertyFeature.ResolvedEntityTypeName.Item;
                            ICompiledType IndexResultType = AsPropertyFeature.ResolvedEntityType.Item;

                            IndexResultType.InstanciateType(instancingClassType, ref IndexResultTypeName, ref IndexResultType);

                            IsConformantToEnumerable = true;
                            indexTypeName = IndexResultTypeName;
                            indexType = IndexResultType;
                        }

            return IsConformantToEnumerable;
        }

        private bool FindIndexer(IClassType instancingClassType, ISource source, IFeatureInstance indexerInstance, out ITypeName indexTypeName, out ICompiledType indexType)
        {
            bool IsConformantToNumericIndexer = false;
            indexTypeName = null;
            indexType = null;

            IIndexerFeature AsIndexer = indexerInstance.Feature as IIndexerFeature;
            Debug.Assert(AsIndexer != null);

            if (AsIndexer.IndexParameterList.Count == 1)
                if (Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, source, out ITypeName NumberTypeName, out ICompiledType NumberType))
                {
                    IEntityDeclaration IndexParameterDeclaration = AsIndexer.IndexParameterList[0];
                    ICompiledType IndexParameterType = IndexParameterDeclaration.ValidEntity.Item.ResolvedEffectiveType.Item;

                    if (IndexParameterType is IClassType AsClassIndexType)
                        if (AsClassIndexType == NumberType)
                        {
                            ITypeName IndexResultTypeName = AsIndexer.ResolvedEntityTypeName.Item;
                            ICompiledType IndexResultType = AsIndexer.ResolvedEntityType.Item;

                            IndexResultType.InstanciateType(instancingClassType, ref IndexResultTypeName, ref IndexResultType);

                            IsConformantToNumericIndexer = true;
                            indexTypeName = IndexResultTypeName;
                            indexType = IndexResultType;
                        }
                }

            return IsConformantToNumericIndexer;
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

            node.ResolvedInitResult.Item = ResultType.Empty;

            node.AdditionalScope.Merge(node.InnerLoopScope);
            node.AdditionalScope.Seal();
        }
        #endregion
    }
}
