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
                new OnceReferenceTableSourceTemplate<IIndexAssignmentInstruction, string, IScopeAttributeFeature, ITypeName>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureTypeName), TemplateNodeStart<IIndexAssignmentInstruction>.Default),
                new OnceReferenceTableSourceTemplate<IIndexAssignmentInstruction, string, IScopeAttributeFeature, ICompiledType>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureType), TemplateNodeStart<IIndexAssignmentInstruction>.Default),
                new SealedTableSourceTemplate<IIndexAssignmentInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IIndexAssignmentInstruction>.Default),
                new OnceReferenceCollectionSourceTemplate<IIndexAssignmentInstruction, IArgument, IResultException>(nameof(IIndexAssignmentInstruction.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIndexAssignmentInstruction, IResultException>(nameof(IIndexAssignmentInstruction.ResolvedException)),
                new UnsealedListDestinationTemplate<IIndexAssignmentInstruction, IParameter>(nameof(IIndexAssignmentInstruction.SelectedParameterList)),
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

            IHashtableEx<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

            if (!ObjectType.GetQualifiedPathFinalType(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, ErrorList, out ICompiledFeature FinalFeature, out IDiscrete FinalDiscrete, out ITypeName FinalTypeName, out ICompiledType FinalType, out bool InheritBySideAttribute))
                return false;

            Debug.Assert(FinalFeature != null);

            if (FinalType is IClassType AsClassType)
            {
                IClass IndexedBaseClass = AsClassType.BaseClass;
                IHashtableEx<IFeatureName, IFeatureInstance> IndexedFeatureTable = IndexedBaseClass.FeatureTable;

                if (!IndexedFeatureTable.ContainsKey(FeatureName.IndexerFeatureName))
                {
                    AddSourceError(new ErrorMissingIndexer(node));
                    return false;
                }

                IFeatureInstance IndexerInstance = IndexedFeatureTable[FeatureName.IndexerFeatureName];
                IIndexerFeature Indexer = (IndexerFeature)IndexerInstance.Feature.Item;
                IIndexerType AsIndexerType = (IndexerType)Indexer.ResolvedFeatureType.Item;

                List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
                IErrorList ArgumentErrorList = new ErrorList();
                if (!Argument.Validate(node.ArgumentList, MergedArgumentList, out TypeArgumentStyles ArgumentStyle, ArgumentErrorList))
                {
                    AddSourceErrorList(ArgumentErrorList);
                    return false;
                }

                IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();
                ParameterTableList.Add(AsIndexerType.ParameterTable);

                if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, ArgumentStyle, ErrorList, node, out int SelectedIndex))
                    return false;

                IResultType SourceResult = Source.ResolvedResult.Item;

                if (SourceResult.Count != 1)
                {
                    AddSourceError(new ErrorInvalidExpression(Source));
                    return false;
                }

                ICompiledType SourceType = SourceResult.At(0).ValueType;

                IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();
                if (!ObjectType.TypeConformToBase(SourceType, AsIndexerType.ResolvedEntityType.Item, SubstitutionTypeTable, ErrorList, Source))
                {
                    AddSourceError(new ErrorInvalidExpression(Source));
                    return false;
                }

                ObjectType.FillResultPath(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, Destination.ValidResultTypePath.Item);
                ListTableEx<IParameter> SelectedParameterList = ParameterTableList[SelectedIndex];

                IResultException ResolvedException = new ResultException();

                ResultException.Merge(ResolvedException, AsIndexerType.SetExceptionIdentifierList);

                foreach (IArgument Item in node.ArgumentList)
                    ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                data = new Tuple<IResultException, ListTableEx<IParameter>>(ResolvedException, SelectedParameterList);
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
            IResultException ResolvedException = ((Tuple<IResultException, ListTableEx<IParameter>>)data).Item1;
            ListTableEx<IParameter> SelectedParameterList = ((Tuple<IResultException, ListTableEx<IParameter>>)data).Item2;

            node.ResolvedException.Item = ResolvedException;
            node.SelectedParameterList.AddRange(SelectedParameterList);
            node.SelectedParameterList.Seal();
        }
        #endregion
    }
}
