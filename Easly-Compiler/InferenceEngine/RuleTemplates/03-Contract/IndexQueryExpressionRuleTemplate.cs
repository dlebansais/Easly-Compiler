namespace EaslyCompiler
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
                new OnceReferenceSourceTemplate<IIndexQueryExpression, IList<IIdentifier>>(nameof(IIndexQueryExpression.IndexedExpression) + Dot + nameof(IExpression.ResolvedExceptions)),
                new OnceReferenceCollectionSourceTemplate<IIndexQueryExpression, IArgument, IList<IExpressionType>>(nameof(IIndexQueryExpression.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIndexQueryExpression, IList<IExpressionType>>(nameof(IIndexQueryExpression.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IIndexQueryExpression, IList<IIdentifier>>(nameof(IIndexQueryExpression.ResolvedExceptions)),
                new UnsealedListDestinationTemplate<IIndexQueryExpression, IExpression>(nameof(IIndexQueryExpression.ConstantSourceList)),
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

            Success &= IndexQueryExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ListTableEx<IParameter> SelectedParameterList, out IList<IExpressionType> ResolvedArgumentList);

            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, IList<IExpressionType>>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant, SelectedParameterList, ResolvedArgumentList);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IIndexQueryExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        /// <param name="selectedParameterList">The selected parameters.</param>
        /// <param name="resolvedArgumentList">The list of arguments corresponding to selected parameters.</param>
        public static bool ResolveCompilerReferences(IIndexQueryExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ListTableEx<IParameter> selectedParameterList, out IList<IExpressionType> resolvedArgumentList)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            selectedParameterList = null;
            resolvedArgumentList = null;

            IExpression IndexedExpression = (IExpression)node.IndexedExpression;
            IList<IArgument> ArgumentList = (IList<IArgument>)node.ArgumentList;
            IClass EmbeddingClass = node.EmbeddingClass;
            IList<IExpressionType> ResolvedIndexerResult = IndexedExpression.ResolvedResult.Item;

            OnceReference<ICompiledType> IndexedExpressionType = new OnceReference<ICompiledType>();
            foreach (IExpressionType Item in ResolvedIndexerResult)
                if (Item.Name == nameof(BaseNode.Keyword.Result) || ResolvedIndexerResult.Count == 1)
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
                //ExpressionConstant.Item = ??
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
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, IList<IExpressionType>>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, IList<IExpressionType>>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, IList<IExpressionType>>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, IList<IExpressionType>>)data).Item4;
            ListTableEx<IParameter> SelectedParameterList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, IList<IExpressionType>>)data).Item5;
            IList<IExpressionType> ResolvedArgumentList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ListTableEx<IParameter>, IList<IExpressionType>>)data).Item6;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
