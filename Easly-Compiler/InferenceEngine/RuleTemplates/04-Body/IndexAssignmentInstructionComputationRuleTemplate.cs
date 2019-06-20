namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IIndexAssignmentInstruction"/>.
    /// </summary>
    public interface IIndexAssignmentInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIndexAssignmentInstruction"/>.
    /// </summary>
    public class IndexAssignmentInstructionComputationRuleTemplate : RuleTemplate<IIndexAssignmentInstruction, IndexAssignmentInstructionComputationRuleTemplate>, IIndexAssignmentInstructionComputationRuleTemplate
    {
        #region Init
        static IndexAssignmentInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceTableSourceTemplate<IIndexAssignmentInstruction, string, IScopeAttributeFeature, ITypeName>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedEffectiveTypeName), TemplateNodeStart<IIndexAssignmentInstruction>.Default),
                new OnceReferenceTableSourceTemplate<IIndexAssignmentInstruction, string, IScopeAttributeFeature, ICompiledType>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedEffectiveType), TemplateNodeStart<IIndexAssignmentInstruction>.Default),
                new SealedTableSourceTemplate<IIndexAssignmentInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IIndexAssignmentInstruction>.Default),
                new OnceReferenceCollectionSourceTemplate<IIndexAssignmentInstruction, IArgument, IResultException>(nameof(IIndexAssignmentInstruction.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIndexAssignmentInstruction, IResultException>(nameof(IIndexAssignmentInstruction.ResolvedException)),
                new OnceReferenceDestinationTemplate<IIndexAssignmentInstruction, IFeatureCall>(nameof(IIndexAssignmentInstruction.FeatureCall)),
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
        public override bool CheckConsistency(IIndexAssignmentInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IQualifiedName Destination = (IQualifiedName)node.Destination;
            IExpression Source = (IExpression)node.Source;
            IList<IIdentifier> ValidPath = Destination.ValidPath.Item;
            IClass EmbeddingClass = node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;

            ISealableDictionary<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

            if (!ObjectType.GetQualifiedPathFinalType(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, ErrorList, out ICompiledFeature FinalFeature, out IDiscrete FinalDiscrete, out ITypeName FinalTypeName, out ICompiledType FinalType, out bool InheritBySideAttribute))
                return false;

            Debug.Assert(FinalFeature != null);
            FinalType = FinalFeature.ResolvedEffectiveType.Item;

            if (FinalType is IClassType AsClassType)
            {
                IClass IndexedBaseClass = AsClassType.BaseClass;
                ISealableDictionary<IFeatureName, IFeatureInstance> IndexedFeatureTable = IndexedBaseClass.FeatureTable;

                if (!IndexedFeatureTable.ContainsKey(FeatureName.IndexerFeatureName))
                {
                    AddSourceError(new ErrorMissingIndexer(node));
                    return false;
                }

                IFeatureInstance IndexerInstance = IndexedFeatureTable[FeatureName.IndexerFeatureName];
                IIndexerFeature Indexer = (IndexerFeature)IndexerInstance.Feature;
                IIndexerType AsIndexerType = (IndexerType)Indexer.ResolvedAgentType.Item;

                bool IsReadOnlyIndexer = Indexer.GetterBody.IsAssigned && !Indexer.SetterBody.IsAssigned;
                bool IsReadOnlyIndexerType = AsIndexerType.IndexerKind == BaseNode.UtilityType.ReadOnly;
                Debug.Assert(IsReadOnlyIndexerType == IsReadOnlyIndexer);

                if (IsReadOnlyIndexer)
                {
                    AddSourceError(new ErrorInvalidInstruction(node));
                    return false;
                }
                else
                {
                    IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();
                    ParameterTableList.Add(AsIndexerType.ParameterTable);

                    IList<ISealableList<IParameter>> ResultTableList = new List<ISealableList<IParameter>>();
                    ResultTableList.Add(new SealableList<IParameter>());

                    ICompiledType DestinationType = AsIndexerType.ResolvedEntityType.Item;

                    if (!Argument.CheckAssignmentConformance(ParameterTableList, ResultTableList, node.ArgumentList, Source, DestinationType, ErrorList, node, out IFeatureCall FeatureCall))
                        return false;

                    ObjectType.FillResultPath(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, Destination.ValidResultTypePath.Item);

                    IResultException ResolvedException = new ResultException();

                    ResultException.Merge(ResolvedException, AsIndexerType.SetExceptionIdentifierList);

                    foreach (IArgument Item in node.ArgumentList)
                        ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                    data = new Tuple<IResultException, IFeatureCall>(ResolvedException, FeatureCall);
                }
            }
            else
            {
                AddSourceError(new ErrorInvalidInstruction(node));
                return false;
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IIndexAssignmentInstruction node, object data)
        {
            IResultException ResolvedException = ((Tuple<IResultException, IFeatureCall>)data).Item1;
            IFeatureCall FeatureCall = ((Tuple<IResultException, IFeatureCall>)data).Item2;

            node.ResolvedException.Item = ResolvedException;
            node.FeatureCall.Item = FeatureCall;
        }
        #endregion
    }
}
