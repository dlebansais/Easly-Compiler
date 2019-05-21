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
                new OnceReferenceDestinationTemplate<IQueryExpression, IList<IIdentifier>>(nameof(IQueryExpression.ResolvedExceptions)),
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

            Success &= QueryExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ICompiledFeature ResolvedFinalFeature, out IDiscrete ResolvedFinalDiscrete, out ListTableEx<IParameter> SelectedParameterList, out ListTableEx<IParameter> SelectedResultList, out IList<IExpressionType> ResolvedArgumentList);

            if (Success)
            {
                Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, IList<IExpressionType>> AdditionalData = new Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, IList<IExpressionType>>(ResolvedFinalFeature, ResolvedFinalDiscrete, SelectedParameterList, SelectedResultList, ResolvedArgumentList);
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, IList<IExpressionType>>>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant, AdditionalData);
            }

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
        /// <param name="resolvedFinalDiscrete">The discrete if the end of the path is a discrete.</param>
        /// <param name="selectedParameterList">The selected parameters.</param>
        /// <param name="selectedResultList">The selected results.</param>
        /// <param name="resolvedArgumentList">The list of arguments corresponding to selected parameters.</param>
        public static bool ResolveCompilerReferences(IQueryExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ICompiledFeature resolvedFinalFeature, out IDiscrete resolvedFinalDiscrete, out ListTableEx<IParameter> selectedParameterList, out ListTableEx<IParameter> selectedResultList, out IList<IExpressionType> resolvedArgumentList)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            resolvedFinalFeature = null;
            resolvedFinalDiscrete = null;
            selectedParameterList = null;
            selectedResultList = null;
            resolvedArgumentList = null;

            IQualifiedName Query = (IQualifiedName)node.Query;
            IList<IArgument> ArgumentList = node.ArgumentList;
            IClass EmbeddingClass = node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;
            IList<IIdentifier> ValidPath = Query.ValidPath.Item;

            IHashtableEx<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);
            Debug.Assert(LocalScope != null);

            if (!ObjectType.GetQualifiedPathFinalType(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, errorList, out ICompiledFeature FinalFeature, out IDiscrete FinalDiscrete, out ITypeName FinalTypeName, out ICompiledType FinalType, out bool InheritBySideAttribute))
                return false;

            Debug.Assert(FinalFeature != null || FinalDiscrete != null);

            if (FinalFeature != null)
            {
                resolvedFinalFeature = FinalFeature;

                List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
                TypeArgumentStyles ArgumentStyle;
                if (!Argument.Validate(ArgumentList, MergedArgumentList, out ArgumentStyle, errorList))
                    return false;

                IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();
                IIdentifier LastIdentifier = ValidPath[ValidPath.Count - 1];
                string ValidText = LastIdentifier.ValidText.Item;
                bool IsHandled = false;
                bool Success = true;

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
                        IsHandled = true;
                        break;

                    case IProcedureType AsProcedureType:
                    case IIndexerType AsIndexerType:
                        errorList.AddError(new ErrorInvalidExpression(node));
                        Success = false;
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
                        IsHandled = true;
                        break;

                    case IFormalGenericType AsFormalGenericType:
                        resolvedResult = new List<IExpressionType>()
                        {
                            new ExpressionType(FinalTypeName, AsFormalGenericType, ValidText)
                        };

                        resolvedExceptions = new List<IIdentifier>();
                        selectedParameterList = new ListTableEx<IParameter>();
                        selectedResultList = new ListTableEx<IParameter>();
                        resolvedArgumentList = MergedArgumentList;
                        IsHandled = true;
                        break;

                    case ITupleType AsTupleType:
                        resolvedResult = new List<IExpressionType>()
                        {
                            new ExpressionType(FinalTypeName, AsTupleType, ValidText)
                        };

                        resolvedExceptions = new List<IIdentifier>();
                        selectedParameterList = new ListTableEx<IParameter>();
                        selectedResultList = new ListTableEx<IParameter>();
                        resolvedArgumentList = MergedArgumentList;
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);

                if (!Success)
                    return false;

                ObjectType.FillResultPath(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, Query.ValidResultTypePath.Item);

                IsHandled = false;
                switch (FinalFeature)
                {
                    case IConstantFeature AsConstantFeature:
                        IExpression ConstantValue = (IExpression)AsConstantFeature.ConstantValue;
                        constantSourceList.Add(ConstantValue);
                        IsHandled = true;
                        break;

                    default:
                        IsHandled = true; // TODO: handle IFunctionFeature or the indexer to try to get a constant.
                        break;
                }
            }
            else
            {
                Debug.Assert(FinalDiscrete != null);

                resolvedFinalDiscrete = FinalDiscrete;

                // This is enforced by the code above.
                bool IsNumberTypeAvailable = Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, out ITypeName NumberTypeName, out ICompiledType NumberType);
                Debug.Assert(IsNumberTypeAvailable);

                resolvedResult = new List<IExpressionType>()
                {
                    new ExpressionType(NumberTypeName, NumberType, FinalDiscrete.ValidDiscreteName.Item.Name),
                };

                resolvedExceptions = new List<IIdentifier>();

                if (FinalDiscrete.NumericValue.IsAssigned)
                {
                    IExpression NumericValue = (IExpression)FinalDiscrete.NumericValue.Item;
                    constantSourceList.Add(NumericValue);
                }
                else
                    expressionConstant = new DiscreteLanguageConstant(FinalDiscrete);
            }

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IQueryExpression node, object data)
        {
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, IList<IExpressionType>>>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, IList<IExpressionType>>>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, IList<IExpressionType>>>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, IList<IExpressionType>>>)data).Item4;
            Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, IList<IExpressionType>> AdditionalData = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, IList<IExpressionType>>>)data).Item5;
            ICompiledFeature ResolvedFinalFeature = AdditionalData.Item1;
            IDiscrete ResolvedFinalDiscrete = AdditionalData.Item2;
            ListTableEx<IParameter> SelectedParameterList = AdditionalData.Item3;
            ListTableEx<IParameter> SelectedResultList = AdditionalData.Item4;
            IList<IExpressionType> ResolvedArgumentList = AdditionalData.Item5;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;

            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();

            if (node.ConstantSourceList.Count == 0)
                node.ExpressionConstant.Item = ExpressionConstant;

            Debug.Assert(ResolvedFinalFeature != null || ResolvedFinalDiscrete != null);

            if (ResolvedFinalFeature != null)
                node.ResolvedFinalFeature.Item = ResolvedFinalFeature;

            if (ResolvedFinalDiscrete != null)
                node.ResolvedFinalDiscrete.Item = ResolvedFinalDiscrete;
        }
        #endregion
    }
}
