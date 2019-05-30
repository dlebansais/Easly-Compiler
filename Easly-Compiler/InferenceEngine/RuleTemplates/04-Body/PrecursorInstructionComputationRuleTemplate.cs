namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IPrecursorInstruction"/>.
    /// </summary>
    public interface IPrecursorInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorInstruction"/>.
    /// </summary>
    public class PrecursorInstructionComputationRuleTemplate : RuleTemplate<IPrecursorInstruction, PrecursorInstructionComputationRuleTemplate>, IPrecursorInstructionComputationRuleTemplate
    {
        #region Init
        static PrecursorInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IPrecursorInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IPrecursorInstruction>.Default),
                new OnceReferenceCollectionSourceTemplate<IPrecursorInstruction, IArgument, IResultException>(nameof(IPrecursorInstruction.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorInstruction, IResultException>(nameof(IPrecursorInstruction.ResolvedException)),
                new UnsealedListDestinationTemplate<IPrecursorInstruction, IParameter>(nameof(IPrecursorInstruction.SelectedParameterList)),
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
        public override bool CheckConsistency(IPrecursorInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IClass EmbeddingClass = node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;
            IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable = EmbeddingClass.FeatureTable;
            IFeature InnerFeature = node.EmbeddingFeature;

            if (InnerFeature is IIndexerFeature AsIndexerFeature)
            {
                AddSourceError(new ErrorPrecursorNotAllowedInIndexer(node));
                return false;
            }
            else
            {
                IFeature AsNamedFeature = InnerFeature;
                IFeatureInstance Instance = FeatureTable[AsNamedFeature.ValidFeatureName.Item];
                OnceReference<IFeatureInstance> SelectedPrecursor = new OnceReference<IFeatureInstance>();
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
                ICompiledType OperatorType = OperatorFeature.ResolvedFeatureType.Item;
                IList<IIdentifier> PrecursorInstructionException = null;
                ListTableEx<IParameter> SelectedParameterList = null;

                // This has been checked in the type pass.
                IProcedureType AsProcedureType = OperatorType as IProcedureType;
                Debug.Assert(AsProcedureType != null);
                foreach (ICommandOverloadType Overload in AsProcedureType.OverloadList)
                    ParameterTableList.Add(Overload.ParameterTable);

                if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, ArgumentStyle, ErrorList, node, out int SelectedIndex))
                    return false;

                ICommandOverloadType SelectedOverload = AsProcedureType.OverloadList[SelectedIndex];
                PrecursorInstructionException = SelectedOverload.ExceptionIdentifierList;
                SelectedParameterList = SelectedOverload.ParameterTable;

                IResultException ResolvedException = new ResultException();

                foreach (IArgument Item in node.ArgumentList)
                    ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                ResultException.Merge(ResolvedException, PrecursorInstructionException);

                data = new Tuple<IResultException, ListTableEx<IParameter>>(ResolvedException, SelectedParameterList);
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IPrecursorInstruction node, object data)
        {
            IResultException ResolvedException = ((Tuple<IResultException, ListTableEx<IParameter>>)data).Item1;
            ListTableEx<IParameter> SelectedParameterList = ((Tuple<IResultException, ListTableEx<IParameter>>)data).Item2;

            node.ResolvedException.Item = ResolvedException;
            node.SelectedParameterList.AddRange(SelectedParameterList);
            node.SelectedParameterList.Seal();

            IFeature EmbeddingFeature = node.EmbeddingFeature;
            IFeatureWithPrecursor ResolvedFeature = EmbeddingFeature.ResolvedFeature.Item as IFeatureWithPrecursor;
            Debug.Assert(ResolvedFeature != null);
            ResolvedFeature.MarkAsCallingPrecursor();
        }
        #endregion
    }
}
