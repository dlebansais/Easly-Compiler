namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexExpression"/>.
    /// </summary>
    public interface IPrecursorIndexExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexExpression"/>.
    /// </summary>
    public class PrecursorIndexExpressionRuleTemplate : RuleTemplate<IPrecursorIndexExpression, PrecursorIndexExpressionRuleTemplate>, IPrecursorIndexExpressionRuleTemplate
    {
        #region Init
        static PrecursorIndexExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new ConditionallyAssignedReferenceSourceTemplate<IPrecursorExpression, IObjectType, ITypeName>(nameof(IPrecursorIndexExpression.AncestorType), nameof(IObjectType.ResolvedTypeName)),
                new ConditionallyAssignedReferenceSourceTemplate<IPrecursorExpression, IObjectType, ICompiledType>(nameof(IPrecursorIndexExpression.AncestorType), nameof(IObjectType.ResolvedType)),
                new OnceReferenceCollectionSourceTemplate<IPrecursorIndexExpression, IArgument, IList<IExpressionType>>(nameof(IPrecursorIndexExpression.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorIndexExpression, IList<IExpressionType>>(nameof(IPrecursorIndexExpression.ResolvedResult)),
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
        public override bool CheckConsistency(IPrecursorIndexExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= PrecursorIndexExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out ListTableEx<IParameter> SelectedParameterList, out IList<IExpressionType> ResolvedArgumentList, out ILanguageConstant ResultNumberConstant, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions);

            if (Success)
                data = new Tuple<ListTableEx<IParameter>, IList<IExpressionType>, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>(SelectedParameterList, ResolvedArgumentList, ResultNumberConstant, ResolvedResult, ResolvedExceptions);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IPrecursorIndexExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="selectedParameterList">The selected parameters.</param>
        /// <param name="resolvedArgumentList">The list of arguments corresponding to selected parameters.</param>
        /// <param name="resultNumberConstant">The expression constant upon return.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        public static bool ResolveCompilerReferences(IPrecursorIndexExpression node, IErrorList errorList, out ListTableEx<IParameter> selectedParameterList, out IList<IExpressionType> resolvedArgumentList, out ILanguageConstant resultNumberConstant, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions)
        {
            selectedParameterList = null;
            resolvedArgumentList = null;
            resultNumberConstant = null;
            resolvedResult = null;
            resolvedExceptions = null;

            IOptionalReference<BaseNode.IObjectType> AncestorType = node.AncestorType;
            IList<IArgument> ArgumentList = node.ArgumentList;
            IClass EmbeddingClass = node.EmbeddingClass;

            IHashtableEx<string, IImportedClass> ClassTable = EmbeddingClass.ImportedClassTable;
            IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable = EmbeddingClass.FeatureTable;
            OnceReference<IFeatureInstance> SelectedPrecursor = new OnceReference<IFeatureInstance>();
            IFeature InnerFeature = node.EmbeddingFeature;

            if (InnerFeature is IIndexerFeature AsIndexerFeature)
            {
                IFeatureInstance Instance = FeatureTable[FeatureName.IndexerFeatureName];

                if (AncestorType.IsAssigned)
                {
                    IObjectType AssignedAncestorType = (IObjectType)AncestorType.Item;

                    foreach (IPrecursorInstance PrecursorItem in Instance.PrecursorList)
                        if (PrecursorItem.Ancestor == AssignedAncestorType)
                        {
                            SelectedPrecursor.Item = PrecursorItem.Precursor;
                            break;
                        }

                    if (!SelectedPrecursor.IsAssigned)
                    {
                        errorList.AddError(new ErrorInvalidPrecursor(AssignedAncestorType));
                        return false;
                    }
                }
                else
                {
                    if (Instance.PrecursorList.Count > 0)
                        SelectedPrecursor.Item = Instance.PrecursorList[0].Precursor;
                    else
                    {
                        errorList.AddError(new ErrorNoPrecursor(node));
                        return false;
                    }
                }

                ICompiledFeature OperatorFeature = SelectedPrecursor.Item.Feature.Item;
                ICompiledType OperatorType = OperatorFeature.ResolvedFeatureType.Item;

                List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
                if (!Argument.Validate(ArgumentList, MergedArgumentList, out TypeArgumentStyles ArgumentStyle, errorList))
                    return false;

                IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();
                bool IsHandled = false;
                bool Success = false;

                switch (OperatorType)
                {
                    case IFunctionType AsFunctionType:
                    case IProcedureType AsProcedureType:
                    case IPropertyType AsPropertyType:
                        errorList.AddError(new ErrorInvalidExpression(node));
                        IsHandled = true;
                        break;

                    case IIndexerType AsIndexerType:
                        ParameterTableList.Add(AsIndexerType.ParameterTable);

                        int SelectedIndex;
                        if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, ArgumentStyle, errorList, node, out SelectedIndex))
                            return false;

                        resolvedResult = new List<IExpressionType>()
                        {
                            new ExpressionType(AsIndexerType.ResolvedEntityTypeName.Item, AsIndexerType.ResolvedEntityType.Item, string.Empty)
                        };

                        resolvedExceptions = AsIndexerType.GetExceptionIdentifierList;
                        selectedParameterList = ParameterTableList[SelectedIndex];
                        resolvedArgumentList = MergedArgumentList;
                        Success = true;
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);

                //IsResultConstant = false;
                //TODO: check if the precursor is a constant
                //TODO: check if the precursor is a constant number
                //ResultNumberConstant.Item = ??
                return Success;
            }
            else
            {
                errorList.AddError(new ErrorPrecursorNotAllowedInIndexer(node));
                return false;
            }
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IPrecursorIndexExpression node, object data)
        {
            ListTableEx<IParameter> SelectedParameterList = ((Tuple<ListTableEx<IParameter>, IList<IExpressionType>, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item1;
            IList<IExpressionType> ResolvedArgumentList = ((Tuple<ListTableEx<IParameter>, IList<IExpressionType>, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item2;
            ILanguageConstant ResultNumberConstant = ((Tuple<ListTableEx<IParameter>, IList<IExpressionType>, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item3;
            IList<IExpressionType> ResolvedResult = ((Tuple<ListTableEx<IParameter>, IList<IExpressionType>, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item4;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<ListTableEx<IParameter>, IList<IExpressionType>, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item5;

            node.ResolvedResult.Item = ResolvedResult;
            //node.SetIsConstant(ResultNumberConstant);
        }
        #endregion
    }
}
