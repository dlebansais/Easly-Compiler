namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexAssignmentInstruction"/>.
    /// </summary>
    public interface IPrecursorIndexAssignmentInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexAssignmentInstruction"/>.
    /// </summary>
    public class PrecursorIndexAssignmentInstructionComputationRuleTemplate : RuleTemplate<IPrecursorIndexAssignmentInstruction, PrecursorIndexAssignmentInstructionComputationRuleTemplate>, IPrecursorIndexAssignmentInstructionComputationRuleTemplate
    {
        #region Init
        static PrecursorIndexAssignmentInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IPrecursorIndexAssignmentInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IPrecursorIndexAssignmentInstruction>.Default),
                new OnceReferenceSourceTemplate<IPrecursorIndexAssignmentInstruction, IResultException>(nameof(IPrecursorIndexAssignmentInstruction.Source) + Dot + nameof(IExpression.ResolvedException)),
                new OnceReferenceCollectionSourceTemplate<IPrecursorIndexAssignmentInstruction, IArgument, IResultException>(nameof(IPrecursorIndexAssignmentInstruction.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorIndexAssignmentInstruction, IResultException>(nameof(IPrecursorIndexAssignmentInstruction.ResolvedException)),
                new UnsealedListDestinationTemplate<IPrecursorIndexAssignmentInstruction, IParameter>(nameof(IPrecursorIndexAssignmentInstruction.SelectedParameterList)),
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
        public override bool CheckConsistency(IPrecursorIndexAssignmentInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IExpression SourceExpression = (IExpression)node.Source;
            IResultType SourceResult = SourceExpression.ResolvedResult.Item;
            IClass EmbeddingClass = node.EmbeddingClass;
            IHashtableEx<string, IImportedClass> ClassTable = EmbeddingClass.ImportedClassTable;
            IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable = EmbeddingClass.FeatureTable;
            IFeature InnerFeature = node.EmbeddingFeature;

            if (InnerFeature is IIndexerFeature AsIndexerFeature)
            {
                IFeatureInstance Instance = FeatureTable[AsIndexerFeature.ValidFeatureName.Item];
                if (!Instance.FindPrecursor(node.AncestorType, ErrorList, node, out IFeatureInstance SelectedPrecursor))
                    return false;

                IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();
                ICompiledFeature OperatorFeature = SelectedPrecursor.Feature.Item;
                IIndexerType AsIndexerType = OperatorFeature.ResolvedFeatureType.Item as IIndexerType;
                Debug.Assert(AsIndexerType != null);

                ParameterTableList.Add(AsIndexerType.ParameterTable);

                if (!Argument.CheckAssignmentConformance(ParameterTableList, node.ArgumentList, SourceExpression, AsIndexerType.ResolvedEntityType.Item, ErrorList, node, out ListTableEx<IParameter> SelectedParameterList, out TypeArgumentStyles ArgumentStyle))
                    return false;

                IResultException ResolvedException = new ResultException();

                ResultException.Merge(ResolvedException, SourceExpression.ResolvedException.Item);

                foreach (IArgument Item in node.ArgumentList)
                    ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                ResultException.Merge(ResolvedException, AsIndexerType.SetExceptionIdentifierList);

                data = new Tuple<IResultException, ListTableEx<IParameter>, TypeArgumentStyles>(ResolvedException, SelectedParameterList, ArgumentStyle);
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
        public override void Apply(IPrecursorIndexAssignmentInstruction node, object data)
        {
            IResultException ResolvedException = ((Tuple<IResultException, ListTableEx<IParameter>, TypeArgumentStyles>)data).Item1;
            ListTableEx<IParameter> SelectedParameterList = ((Tuple<IResultException, ListTableEx<IParameter>, TypeArgumentStyles>)data).Item2;
            TypeArgumentStyles ArgumentStyle = ((Tuple<IResultException, ListTableEx<IParameter>, TypeArgumentStyles>)data).Item3;

            node.ResolvedException.Item = ResolvedException;
            node.SelectedParameterList.AddRange(SelectedParameterList);
            node.SelectedParameterList.Seal();
            node.ArgumentStyle = ArgumentStyle;
        }
        #endregion
    }
}
