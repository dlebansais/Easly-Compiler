﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IIndexQueryExpression"/>.
    /// </summary>
    public interface IIndexQueryExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIndexQueryExpression"/>.
    /// </summary>
    public class IndexQueryExpressionRuleTemplate : RuleTemplate<IIndexQueryExpression, IndexQueryExpressionRuleTemplate>, IIndexQueryExpressionRuleTemplate
    {
        #region Init
        static IndexQueryExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IIndexQueryExpression, IList<IExpressionType>>(nameof(IIndexQueryExpression.IndexedExpression) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceCollectionSourceTemplate<IIndexQueryExpression, IArgument, IList<IExpressionType>>(nameof(IIndexQueryExpression.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIndexQueryExpression, IList<IExpressionType>>(nameof(IIndexQueryExpression.ResolvedResult)),
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
        public override bool CheckConsistency(IIndexQueryExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= IndexQueryExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out ListTableEx<IParameter> SelectedParameterList, out IList<IExpressionType> ResolvedArgumentList, out ILanguageConstant ResultNumberConstant, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions);

            if (Success)
                data = new Tuple<ListTableEx<IParameter>, IList<IExpressionType>, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>(SelectedParameterList, ResolvedArgumentList, ResultNumberConstant, ResolvedResult, ResolvedExceptions);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IIndexQueryExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="selectedParameterList">The selected parameters.</param>
        /// <param name="resolvedArgumentList">The list of arguments corresponding to selected parameters.</param>
        /// <param name="resultNumberConstant">The expression constant upon return.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        public static bool ResolveCompilerReferences(IIndexQueryExpression node, IErrorList errorList, out ListTableEx<IParameter> selectedParameterList, out IList<IExpressionType> resolvedArgumentList, out ILanguageConstant resultNumberConstant, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions)
        {
            selectedParameterList = null;
            resolvedArgumentList = null;
            resultNumberConstant = null;
            resolvedResult = null;
            resolvedExceptions = null;

            IExpression IndexedExpression = (IExpression)node.IndexedExpression;
            IList<IArgument> ArgumentList = (IList<IArgument>)node.ArgumentList;
            IClass EmbeddingClass = node.EmbeddingClass;
            IList<IExpressionType> ResolvedIndexerResult = IndexedExpression.ResolvedResult.Item;

            OnceReference<ICompiledType> IndexedExpressionType = new OnceReference<ICompiledType>();
            if (ResolvedIndexerResult.Count == 1)
                IndexedExpressionType.Item = ResolvedIndexerResult[0].ValueType;
            else
                foreach (IExpressionType Item in ResolvedIndexerResult)
                    if (Item.Name == nameof(BaseNode.Keyword.Result))
                    {
                        IndexedExpressionType.Item = Item.ValueType;
                        break;
                    }

            if (!IndexedExpressionType.IsAssigned)
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }

            if (IndexedExpressionType.Item is IClassType AsClassType)
            {
                IClass IndexedBaseClass = AsClassType.BaseClass;
                IHashtableEx<IFeatureName, IFeatureInstance> IndexedFeatureTable = IndexedBaseClass.FeatureTable;

                if (!IndexedFeatureTable.ContainsKey(FeatureName.IndexerFeatureName))
                {
                    errorList.AddError(new ErrorMissingIndexer(node));
                    return false;
                }

                IFeatureInstance IndexerInstance = IndexedFeatureTable[FeatureName.IndexerFeatureName];
                IIndexerFeature Indexer = (IndexerFeature)IndexerInstance.Feature.Item;
                IIndexerType AsIndexerType = (IndexerType)Indexer.ResolvedFeatureType.Item;

                List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
                TypeArgumentStyles ArgumentStyle;
                if (!Argument.Validate(ArgumentList, MergedArgumentList, out ArgumentStyle, errorList))
                    return false;

                IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();
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

                //isResultConstant = false; // TODO: check if the result is a constant
                //TODO: check if the result is a constant number
                //ResultNumberConstant.Item = ??
            }
            else
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IIndexQueryExpression node, object data)
        {
            ListTableEx<IParameter> SelectedParameterList = ((Tuple<ListTableEx<IParameter>, IList<IExpressionType>, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item1;
            IList<IExpressionType> ResolvedArgumentList = ((Tuple<ListTableEx<IParameter>, IList<IExpressionType>, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item2;
            ILanguageConstant ResultNumberConstant = ((Tuple<ListTableEx<IParameter>, IList<IExpressionType>, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item3;
            IList<IExpressionType> ResolvedResult = ((Tuple<ListTableEx<IParameter>, IList<IExpressionType>, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item4;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<ListTableEx<IParameter>, IList<IExpressionType>, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item5;

            node.ResolvedResult.Item = ResolvedResult;
            node.SetIsConstant(ResultNumberConstant);
        }
        #endregion
    }
}