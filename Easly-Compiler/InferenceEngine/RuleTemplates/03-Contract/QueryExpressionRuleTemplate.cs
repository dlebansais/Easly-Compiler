namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IQueryExpression"/>.
    /// </summary>
    public interface IQueryExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQueryExpression"/>.
    /// </summary>
    public class QueryExpressionRuleTemplate : RuleTemplate<IQueryExpression, QueryExpressionRuleTemplate>, IQueryExpressionRuleTemplate
    {
        #region Init
        static QueryExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceTableSourceTemplate<IQueryExpression, string, IScopeAttributeFeature, ITypeName>(nameof(IScope.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureTypeName), TemplateScopeStart<IQueryExpression>.Default),
                new OnceReferenceTableSourceTemplate<IQueryExpression, string, IScopeAttributeFeature, ICompiledType>(nameof(IScope.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureType), TemplateScopeStart<IQueryExpression>.Default),
                new OnceReferenceSourceTemplate<IQueryExpression, IList<IExpressionType>>(nameof(IQueryExpression.Query) + Dot + nameof(IQualifiedName.ValidResultTypePath)),
                new OnceReferenceCollectionSourceTemplate<IQueryExpression, IArgument, IList<IExpressionType>>(nameof(IQueryExpression.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IQueryExpression, IList<IExpressionType>>(nameof(IQueryExpression.ResolvedResult)),
                new UnsealedListDestinationTemplate<IQueryExpression, IExpression>(nameof(IQueryExpression.ConstantSourceList)),
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
        public override bool CheckConsistency(IQueryExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= QueryExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ICompiledFeature ResolvedFinalFeature, out ListTableEx<IParameter> SelectedParameterList, out ListTableEx<IParameter> SelectedResultList, out IList<IExpressionType> ResolvedArgumentList);

            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature, ListTableEx<IParameter>, ListTableEx<IParameter>, Tuple<IList<IExpressionType>>>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant, ResolvedFinalFeature, SelectedParameterList, SelectedResultList, new Tuple<IList<IExpressionType>>(ResolvedArgumentList));

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IQueryExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        /// <param name="resolvedFinalFeature">The feature if the end of the path is a feature.</param>
        /// <param name="selectedParameterList">The selected parameters.</param>
        /// <param name="selectedResultList">The selected results.</param>
        /// <param name="resolvedArgumentList">The list of arguments corresponding to selected parameters.</param>
        public static bool ResolveCompilerReferences(IQueryExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ICompiledFeature resolvedFinalFeature, out ListTableEx<IParameter> selectedParameterList, out ListTableEx<IParameter> selectedResultList, out IList<IExpressionType> resolvedArgumentList)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            resolvedFinalFeature = null;
            selectedParameterList = null;
            selectedResultList = null;
            resolvedArgumentList = null;

            IQualifiedName Query = (IQualifiedName)node.Query;
            IList<IArgument> ArgumentList = node.ArgumentList;
            IClass EmbeddingClass = node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;
            IList<IIdentifier> ValidPath = Query.ValidPath.Item;

            IHashtableEx<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);
            if (LocalScope == null)
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }

            if (!ObjectType.GetQualifiedPathFinalType(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, errorList, out ICompiledFeature FinalFeature, out IDiscrete FinalDiscrete, out ITypeName FinalTypeName, out ICompiledType FinalType, out bool InheritBySideAttribute))
                return false;

            if (FinalFeature == null)
            {
                errorList.AddError(new ErrorConstantQueryExpression(node));
                return false;
            }

            resolvedFinalFeature = FinalFeature;

            List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
            TypeArgumentStyles ArgumentStyle;
            if (!Argument.Validate(ArgumentList, MergedArgumentList, out ArgumentStyle, errorList))
            {
                return false;
            }

            IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();
            IIdentifier LastIdentifier = ValidPath[ValidPath.Count - 1];
            string ValidText = LastIdentifier.ValidText.Item;
            bool IsHandled = false;
            bool Success = false;

            switch (FinalType)
            {
                case IFunctionType AsFunctionType:
                    foreach (IQueryOverloadType Overload in AsFunctionType.OverloadList)
                        ParameterTableList.Add(Overload.ParameterTable);

                    int SelectedIndex;
                    if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, ArgumentStyle, errorList, node, out SelectedIndex))
                        return false;

                    IQueryOverloadType SelectedOverload = AsFunctionType.OverloadList[SelectedIndex];
                    resolvedResult = SelectedOverload.Result;
                    resolvedExceptions = SelectedOverload.ExceptionIdentifierList;
                    selectedParameterList = SelectedOverload.ParameterTable;
                    selectedResultList = SelectedOverload.ResultTable;
                    resolvedArgumentList = MergedArgumentList;
                    Success = true;
                    IsHandled = true;
                    break;

                case IProcedureType AsProcedureType:
                case IIndexerType AsIndexerType:
                    errorList.AddError(new ErrorInvalidExpression(node));
                    IsHandled = true;
                    break;

                case IPropertyType AsPropertyType:
                    resolvedResult = new List<IExpressionType>()
                    {
                        new ExpressionType(AsPropertyType.ResolvedEntityTypeName.Item, AsPropertyType.ResolvedEntityType.Item, ValidText)
                    };

                    resolvedExceptions = AsPropertyType.GetExceptionIdentifierList;
                    selectedParameterList = new ListTableEx<IParameter>();
                    selectedResultList = new ListTableEx<IParameter>();
                    resolvedArgumentList = new List<IExpressionType>();
                    Success = true;
                    IsHandled = true;
                    break;

                case IClassType AsClassType:
                    resolvedResult = new List<IExpressionType>()
                    {
                        new ExpressionType(FinalTypeName, AsClassType, ValidText)
                    };

                    resolvedExceptions = new List<IIdentifier>();
                    selectedParameterList = new ListTableEx<IParameter>();
                    selectedResultList = new ListTableEx<IParameter>();
                    resolvedArgumentList = MergedArgumentList;
                    Success = true;
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            if (!Success)
                return false;

            if (FinalFeature != null)
                ObjectType.FillResultPath(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, Query.ValidResultTypePath.Item);

            //IsResultConstant = false;
            //TODO: check if the precursor is a constant
            //TODO: check if the precursor is a constant number
            //ResultNumberConstant.Item = ??

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IQueryExpression node, object data)
        {
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature, ListTableEx<IParameter>, ListTableEx<IParameter>, Tuple<IList<IExpressionType>>>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature, ListTableEx<IParameter>, ListTableEx<IParameter>, Tuple<IList<IExpressionType>>>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature, ListTableEx<IParameter>, ListTableEx<IParameter>, Tuple<IList<IExpressionType>>>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature, ListTableEx<IParameter>, ListTableEx<IParameter>, Tuple<IList<IExpressionType>>>)data).Item4;
            ICompiledFeature ResolvedFinalFeature = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature, ListTableEx<IParameter>, ListTableEx<IParameter>, Tuple<IList<IExpressionType>>>)data).Item5;
            ListTableEx<IParameter> SelectedParameterList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature, ListTableEx<IParameter>, ListTableEx<IParameter>, Tuple<IList<IExpressionType>>>)data).Item6;
            ListTableEx<IParameter> SelectedResultList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature, ListTableEx<IParameter>, ListTableEx<IParameter>, Tuple<IList<IExpressionType>>>)data).Item7;
            IList<IExpressionType> ResolvedArgumentList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature, ListTableEx<IParameter>, ListTableEx<IParameter>, Tuple<IList<IExpressionType>>>)data).Rest.Item1;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.ExpressionConstant.Item = ExpressionConstant;
            node.ResolvedFinalFeature.Item = ResolvedFinalFeature;
        }
        #endregion
    }
}
