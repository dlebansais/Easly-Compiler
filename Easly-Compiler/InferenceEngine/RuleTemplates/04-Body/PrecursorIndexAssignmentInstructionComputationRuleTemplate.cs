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
            OnceReference<IFeatureInstance> SelectedPrecursor = new OnceReference<IFeatureInstance>();
            IFeature InnerFeature = node.EmbeddingFeature;

            if (InnerFeature is IIndexerFeature AsIndexerFeature)
            {
                IFeatureInstance Instance = FeatureTable[AsIndexerFeature.ValidFeatureName.Item];
                IList<IPrecursorInstance> PrecursorList = Instance.PrecursorList;

                if (node.AncestorType.IsAssigned)
                {
                    IObjectType DeclaredAncestor = (IObjectType)node.AncestorType.Item;

                    if (DeclaredAncestor.ResolvedType.Item is IClassType AsClassTypeAncestor)
                    {
                        foreach (IPrecursorInstance Item in PrecursorList)
                            if (Item.Ancestor.BaseClass == AsClassTypeAncestor.BaseClass)
                            {
                                SelectedPrecursor.Item = Item.Precursor;
                                break;
                            }
                    }

                    if (!SelectedPrecursor.IsAssigned)
                    {
                        AddSourceError(new ErrorInvalidPrecursor(DeclaredAncestor));
                        return false;
                    }
                }
                else if (Instance.PrecursorList.Count > 1)
                {
                    AddSourceError(new ErrorInvalidPrecursor(node));
                    return false;
                }
                else if (Instance.PrecursorList.Count == 0)
                {
                    AddSourceError(new ErrorNoPrecursor(node));
                    return false;
                }
                else
                    SelectedPrecursor.Item = Instance.PrecursorList[0].Precursor;

                List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
                if (!Argument.Validate(node.ArgumentList, MergedArgumentList, out TypeArgumentStyles ArgumentStyle, ErrorList))
                    return false;

                IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();

                ICompiledFeature OperatorFeature = SelectedPrecursor.Item.Feature.Item;
                IIndexerType AsIndexerType = OperatorFeature.ResolvedFeatureType.Item as IIndexerType;
                Debug.Assert(AsIndexerType != null);

                ParameterTableList.Add(AsIndexerType.ParameterTable);

                if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, ArgumentStyle, ErrorList, node, out int SelectedIndex))
                    return false;

                if (SourceResult.Count != 1)
                {
                    AddSourceError(new ErrorInvalidExpression(SourceExpression));
                    return false;
                }

                ListTableEx<IParameter> SelectedParameterList = ParameterTableList[SelectedIndex];
                ICompiledType SourceType = SourceResult.At(0).ValueType;

                IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();
                if (!ObjectType.TypeConformToBase(SourceType, AsIndexerType.ResolvedEntityType.Item, SubstitutionTypeTable, ErrorList, SourceExpression))
                {
                    AddSourceError(new ErrorInvalidExpression(SourceExpression));
                    return false;
                }

                IResultException ResolvedException = new ResultException();

                ResultException.Merge(ResolvedException, SourceExpression.ResolvedException.Item);

                foreach (IArgument Item in node.ArgumentList)
                    ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                ResultException.Merge(ResolvedException, AsIndexerType.SetExceptionIdentifierList);

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
        public override void Apply(IPrecursorIndexAssignmentInstruction node, object data)
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
