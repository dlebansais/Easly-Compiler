﻿namespace EaslyCompiler
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
                IFeature AsNamedFeature = InnerFeature;
                IFeatureInstance Instance = FeatureTable[AsNamedFeature.ValidFeatureName.Item];
                OnceReference<IFeatureInstance> SelectedPrecursor = new OnceReference<IFeatureInstance>();

                if (node.AncestorType.IsAssigned)
                {
                    IObjectType AncestorType = (IObjectType)node.AncestorType.Item;

                    foreach (IPrecursorInstance PrecursorItem in Instance.PrecursorList)
                        if (PrecursorItem.Ancestor == AncestorType)
                        {
                            SelectedPrecursor.Item = PrecursorItem.Precursor;
                            break;
                        }

                    if (!SelectedPrecursor.IsAssigned)
                    {
                        AddSourceError(new ErrorInvalidPrecursor(AncestorType));
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
                bool IsHandled = false;

                switch (OperatorType)
                {
                    case IFunctionType AsFunctionType:
                        AddSourceError(new ErrorInvalidExpression(node));
                        Success = false;
                        IsHandled = true;
                        break;

                    case IProcedureType AsProcedureType:
                        foreach (ICommandOverloadType Overload in AsProcedureType.OverloadList)
                            ParameterTableList.Add(Overload.ParameterTable);

                        if (Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, ArgumentStyle, ErrorList, node, out int SelectedIndex))
                        {
                            ICommandOverloadType SelectedOverload = AsProcedureType.OverloadList[SelectedIndex];
                            PrecursorInstructionException = SelectedOverload.ExceptionIdentifierList;
                            SelectedParameterList = SelectedOverload.ParameterTable;
                        }
                        else
                            Success = false;

                        IsHandled = true;
                        break;

                    case IIndexerType AsIndexerType:
                        AddSourceError(new ErrorInvalidExpression(node));
                        Success = false;
                        IsHandled = true;
                        break;

                    case IPropertyType AsPropertyType:
                        AddSourceError(new ErrorInvalidExpression(node));
                        Success = false;
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);

                if (Success)
                {
                    IResultException ResolvedException = new ResultException();

                    foreach (IArgument Item in node.ArgumentList)
                        ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                    ResultException.Merge(ResolvedException, PrecursorInstructionException);

                    data = new Tuple<IResultException, ListTableEx<IParameter>>(ResolvedException, SelectedParameterList);
                }
            }
            else
            {
                AddSourceError(new ErrorPrecursorNotAllowedInIndexer(node));
                return false;
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

            // TODO
            /*IFeature EmbeddingFeature = (IFeature)node.EmbeddingFeature;
            EmbeddingFeature.ResolvedFeature.Item.MarkAsCallingPrecursor();*/
        }
        #endregion
    }
}
