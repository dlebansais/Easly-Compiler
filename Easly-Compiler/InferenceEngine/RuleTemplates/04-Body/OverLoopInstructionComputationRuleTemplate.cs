namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IOverLoopInstruction"/>.
    /// </summary>
    public interface IOverLoopInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IOverLoopInstruction"/>.
    /// </summary>
    public class OverLoopInstructionComputationRuleTemplate : RuleTemplate<IOverLoopInstruction, OverLoopInstructionComputationRuleTemplate>, IOverLoopInstructionComputationRuleTemplate
    {
        #region Init
        static OverLoopInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IOverLoopInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IOverLoopInstruction>.Default),
                new OnceReferenceSourceTemplate<IOverLoopInstruction, IResultException>(nameof(IOverLoopInstruction.OverList) + Dot + nameof(IExpression.ResolvedException)),
                new OnceReferenceSourceTemplate<IOverLoopInstruction, IResultException>(nameof(IOverLoopInstruction.LoopInstructions) + Dot + nameof(IScope.ResolvedException)),
                new OnceReferenceCollectionSourceTemplate<IOverLoopInstruction, IAssertion, IResultException>(nameof(IOverLoopInstruction.InvariantList), nameof(IAssertion.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IOverLoopInstruction, IResultException>(nameof(IOverLoopInstruction.ResolvedException)),
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
            IScope LoopInstructions = (IScope)node.LoopInstructions;
            IClass EmbeddingClass = node.EmbeddingClass;

            bool IsOverLoopSourceTypeAvailable = Expression.IsLanguageTypeAvailable(LanguageClasses.OverLoopSource.Guid, node, out ITypeName OverLoopSourceTypeName, out ICompiledType OverLoopSourceType);
            bool IsNumberTypeAvailable = Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, out ITypeName NumberTypeName, out ICompiledType NumberType);

            foreach (IExpressionType Item in OverTypeList)
            {
                ICompiledType ResultType = Item.ValueType;
                IErrorList OverSourceErrorList = new ErrorList();
                bool IsConformantToEnumerable = false;
                bool IsConformantToNumericIndexer = false;

                if (IsOverLoopSourceTypeAvailable && ObjectType.TypeConformToBase(ResultType, OverLoopSourceType))
                    IsConformantToEnumerable = true;

                if (IsNumberTypeAvailable && ResultType.FeatureTable.ContainsKey(FeatureName.IndexerFeatureName))
                {
                    IFeatureInstance IndexerInstance = ResultType.FeatureTable[FeatureName.IndexerFeatureName];
                    IIndexerFeature IndexerFeature = IndexerInstance.Feature as IIndexerFeature;
                    Debug.Assert(IndexerFeature != null);

                    if (IndexerFeature.IndexParameterList.Count == 1)
                    {
                        IEntityDeclaration IndexParameterDeclaration = IndexerFeature.IndexParameterList[0];
                        if (IndexParameterDeclaration.ValidEntity.Item.ResolvedFeatureType.Item == NumberType)
                            IsConformantToNumericIndexer = true;
                    }
                }

                Debug.Assert(IsConformantToEnumerable != IsConformantToNumericIndexer);
            }

            IResultException ResolvedException = new ResultException();

            ResultException.Merge(ResolvedException, OverList.ResolvedException.Item);
            ResultException.Merge(ResolvedException, LoopInstructions.ResolvedException.Item);

            foreach (IAssertion Item in node.InvariantList)
                ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

            data = ResolvedException;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IOverLoopInstruction node, object data)
        {
            IResultException ResolvedException = (IResultException)data;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
