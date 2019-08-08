namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IQueryExpression.
    /// </summary>
    public interface IQueryExpression : BaseNode.IQueryExpression, INodeWithReplicatedBlocks, IExpression, IComparableExpression
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryExpression.ArgumentBlocks"/>.
        /// </summary>
        IList<IArgument> ArgumentList { get; }

        /// <summary>
        /// The resolved feature at the end of the path.
        /// </summary>
        OnceReference<ICompiledFeature> ResolvedFinalFeature { get; }

        /// <summary>
        /// The resolved discrete at the end of the path.
        /// </summary>
        OnceReference<IDiscrete> ResolvedFinalDiscrete { get; }

        /// <summary>
        /// List of results from the selected overload.
        /// </summary>
        ISealableList<IParameter> SelectedResultList { get; }

        /// <summary>
        /// The selected overload, if any.
        /// </summary>
        OnceReference<IQueryOverloadType> SelectedOverloadType { get; }

        /// <summary>
        /// Details of the feature call.
        /// </summary>
        OnceReference<IFeatureCall> FeatureCall { get; }

        /// <summary>
        /// Inherit the side-by-side attribute.
        /// </summary>
        bool InheritBySideAttribute { get; set; }
    }

    /// <summary>
    /// Compiler IQueryExpression.
    /// </summary>
    public class QueryExpression : BaseNode.QueryExpression, IQueryExpression
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryExpression.ArgumentBlocks"/>.
        /// </summary>
        public IList<IArgument> ArgumentList { get; } = new List<IArgument>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyName">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyName, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyName)
            {
                case nameof(ArgumentBlocks):
                    TargetList = (IList)ArgumentList;
                    break;
            }

            Debug.Assert(TargetList != null);
            Debug.Assert(TargetList.Count == 0);

            foreach (BaseNode.INode Node in nodeList)
                TargetList.Add(Node);
        }
        #endregion

        #region Implementation of ISource
        /// <summary>
        /// The parent node, null if root.
        /// </summary>
        public ISource ParentSource { get; private set; }

        /// <summary>
        /// The parent class, null if none.
        /// </summary>
        public IClass EmbeddingClass { get; private set; }

        /// <summary>
        /// The parent feature, null if none.
        /// </summary>
        public IFeature EmbeddingFeature { get; private set; }

        /// <summary>
        /// The parent overload, null if none.
        /// </summary>
        public IOverload EmbeddingOverload { get; private set; }

        /// <summary>
        /// The parent body, null if none.
        /// </summary>
        public IBody EmbeddingBody { get; private set; }

        /// <summary>
        /// The parent assertion, null if none.
        /// </summary>
        public IAssertion EmbeddingAssertion { get; private set; }

        /// <summary>
        /// Initializes parents based on the provided <paramref name="parentSource"/> node.
        /// </summary>
        /// <param name="parentSource">The parent node.</param>
        public virtual void InitializeSource(ISource parentSource)
        {
            ParentSource = parentSource;

            EmbeddingClass = parentSource is IClass AsClass ? AsClass : parentSource?.EmbeddingClass;
            EmbeddingFeature = parentSource is IFeature AsFeature ? AsFeature : parentSource?.EmbeddingFeature;
            EmbeddingOverload = parentSource is IOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
            EmbeddingBody = parentSource is IBody AsBody ? AsBody : parentSource?.EmbeddingBody;
            EmbeddingAssertion = parentSource is IAssertion AsAssertion ? AsAssertion : parentSource?.EmbeddingAssertion;
        }

        /// <summary>
        /// Reset some intermediate results.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to reset.</param>
        public void Reset(IRuleTemplateList ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Types)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ResolvedResult = new OnceReference<IResultType>();
                ConstantSourceList = new SealableList<IExpression>();
                ExpressionConstant = new OnceReference<ILanguageConstant>();
                ResolvedFinalFeature = new OnceReference<ICompiledFeature>();
                ResolvedFinalDiscrete = new OnceReference<IDiscrete>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                ResolvedException = new OnceReference<IResultException>();
                SelectedResultList = new SealableList<IParameter>();
                SelectedOverloadType = new OnceReference<IQueryOverloadType>();
                FeatureCall = new OnceReference<IFeatureCall>();
                InheritBySideAttribute = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IRuleTemplateList ruleTemplateList)
        {
            bool IsResolved = false;

            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ExpressionConstant.IsAssigned;

                Debug.Assert(ResolvedResult.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedFinalFeature.IsAssigned || ResolvedFinalDiscrete.IsAssigned || !IsResolved);

                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = ResolvedException.IsAssigned;

                Debug.Assert(SelectedResultList.IsSealed || !IsResolved);
                Debug.Assert(FeatureCall.IsAssigned || !IsResolved);

                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IExpression
        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public bool IsComplex { get { return false; } }

        /// <summary>
        /// Types of expression results.
        /// </summary>
        public OnceReference<IResultType> ResolvedResult { get; private set; } = new OnceReference<IResultType>();

        /// <summary>
        /// List of exceptions the expression can throw.
        /// </summary>
        public OnceReference<IResultException> ResolvedException { get; private set; } = new OnceReference<IResultException>();

        /// <summary>
        /// The list of sources for a constant, if any.
        /// </summary>
        public ISealableList<IExpression> ConstantSourceList { get; private set; } = new SealableList<IExpression>();

        /// <summary>
        /// Specific constant number.
        /// </summary>
        public OnceReference<ILanguageConstant> ExpressionConstant { get; private set; } = new OnceReference<ILanguageConstant>();
        #endregion

        #region Compiler
        /// <summary>
        /// The resolved feature at the end of the path.
        /// </summary>
        public OnceReference<ICompiledFeature> ResolvedFinalFeature { get; private set; } = new OnceReference<ICompiledFeature>();

        /// <summary>
        /// The resolved discrete at the end of the path.
        /// </summary>
        public OnceReference<IDiscrete> ResolvedFinalDiscrete { get; private set; } = new OnceReference<IDiscrete>();

        /// <summary>
        /// List of results from the selected overload.
        /// </summary>
        public ISealableList<IParameter> SelectedResultList { get; private set; } = new SealableList<IParameter>();

        /// <summary>
        /// The selected overload, if any.
        /// </summary>
        public OnceReference<IQueryOverloadType> SelectedOverloadType { get; private set; } = new OnceReference<IQueryOverloadType>();

        /// <summary>
        /// Details of the feature call.
        /// </summary>
        public OnceReference<IFeatureCall> FeatureCall { get; private set; } = new OnceReference<IFeatureCall>();

        /// <summary>
        /// Inherit the side-by-side attribute.
        /// </summary>
        public bool InheritBySideAttribute { get; set; }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        public bool IsExpressionEqual(IComparableExpression other)
        {
            return IsExpressionEqual(other as IQueryExpression);
        }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        protected bool IsExpressionEqual(IQueryExpression other)
        {
            Debug.Assert(other != null);

            bool Result = true;

            Result &= QualifiedName.IsQualifiedNameEqual((IQualifiedName)Query, (IQualifiedName)other.Query);
            Result &= Argument.IsArgumentListEqual(ArgumentList, other.ArgumentList);

            return Result;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IQueryExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        /// <param name="resolvedFinalFeature">The feature if the end of the path is a feature.</param>
        /// <param name="resolvedFinalDiscrete">The discrete if the end of the path is a discrete.</param>
        /// <param name="selectedResultList">The selected results.</param>
        /// <param name="selectedOverloadType">The selected overload.</param>
        /// <param name="featureCall">Details of the feature call.</param>
        /// <param name="inheritBySideAttribute">Inherit the side-by-side attribute.</param>
        public static bool ResolveCompilerReferences(IQueryExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ICompiledFeature resolvedFinalFeature, out IDiscrete resolvedFinalDiscrete, out ISealableList<IParameter> selectedResultList, out IQueryOverloadType selectedOverloadType, out IFeatureCall featureCall, out bool inheritBySideAttribute)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new SealableList<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            resolvedFinalFeature = null;
            resolvedFinalDiscrete = null;
            selectedResultList = null;
            selectedOverloadType = null;
            featureCall = null;

            IQualifiedName Query = (IQualifiedName)node.Query;
            IList<IArgument> ArgumentList = node.ArgumentList;
            IClass EmbeddingClass = node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;
            IList<IIdentifier> ValidPath = Query.ValidPath.Item;

            ISealableDictionary<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

            if (!ObjectType.GetQualifiedPathFinalType(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, errorList, out ICompiledFeature FinalFeature, out IDiscrete FinalDiscrete, out ITypeName FinalTypeName, out ICompiledType FinalType, out inheritBySideAttribute))
                return false;

            Debug.Assert(FinalFeature != null || FinalDiscrete != null);

#if COVERAGE
            Debug.Assert(!node.IsComplex);
#endif

            if (FinalFeature != null)
            {
                resolvedFinalFeature = FinalFeature;
                return ResolveFeature(node, errorList, resolvedFinalFeature, FinalTypeName, FinalType, out resolvedResult, out resolvedException, out constantSourceList, out expressionConstant, out selectedResultList, out selectedOverloadType, out featureCall);
            }
            else
            {
                Debug.Assert(FinalDiscrete != null);

                resolvedFinalDiscrete = FinalDiscrete;
                return ResolveDiscrete(node, errorList, resolvedFinalDiscrete, out resolvedResult, out resolvedException, out constantSourceList, out expressionConstant, out selectedResultList, out featureCall);
            }
        }

        private static bool ResolveFeature(IQueryExpression node, IErrorList errorList, ICompiledFeature resolvedFinalFeature, ITypeName finalTypeName, ICompiledType finalType, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ISealableList<IParameter> selectedResultList, out IQueryOverloadType selectedOverloadType, out IFeatureCall featureCall)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new SealableList<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            selectedResultList = null;
            selectedOverloadType = null;
            featureCall = null;

            ISealableDictionary<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

            IQualifiedName Query = (IQualifiedName)node.Query;
            IList<IArgument> ArgumentList = node.ArgumentList;
            IClass EmbeddingClass = node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;
            IList<IIdentifier> ValidPath = Query.ValidPath.Item;

            List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
            if (!Argument.Validate(ArgumentList, MergedArgumentList, out TypeArgumentStyles TypeArgumentStyle, errorList))
                return false;

            IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();
            IIdentifier LastIdentifier = ValidPath[ValidPath.Count - 1];
            string ValidText = LastIdentifier.ValidText.Item;
            bool IsHandled = false;
            bool Success = true;

            switch (finalType)
            {
                case IFunctionType AsFunctionType:
                    foreach (IQueryOverloadType Overload in AsFunctionType.OverloadList)
                        ParameterTableList.Add(Overload.ParameterTable);

                    int SelectedIndex;
                    if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, TypeArgumentStyle, errorList, node, out SelectedIndex))
                        return false;

                    selectedOverloadType = AsFunctionType.OverloadList[SelectedIndex];
                    resolvedResult = new ResultType(selectedOverloadType.ResultTypeList);
                    resolvedException = new ResultException(selectedOverloadType.ExceptionIdentifierList);
                    selectedResultList = selectedOverloadType.ResultTable;
                    featureCall = new FeatureCall(selectedOverloadType.ParameterTable, selectedOverloadType.ResultTable, ArgumentList, MergedArgumentList, TypeArgumentStyle);
                    IsHandled = true;
                    break;

                case IProcedureType AsProcedureType:
                case IIndexerType AsIndexerType:
                    errorList.AddError(new ErrorInvalidExpression(node));
                    Success = false;
                    IsHandled = true;
                    break;

                case IPropertyType AsPropertyType:
                    resolvedResult = new ResultType(AsPropertyType.ResolvedEntityTypeName.Item, AsPropertyType.ResolvedEntityType.Item, ValidText);

                    resolvedException = new ResultException(AsPropertyType.GetExceptionIdentifierList);
                    selectedResultList = new SealableList<IParameter>();
                    featureCall = new FeatureCall();
                    IsHandled = true;
                    break;

                case IClassType AsClassType:
                case ITupleType AsTupleType:
                    resolvedResult = new ResultType(finalTypeName, finalType, ValidText);

                    resolvedException = new ResultException();
                    selectedResultList = new SealableList<IParameter>();
                    featureCall = new FeatureCall();
                    IsHandled = true;
                    break;

                case IFormalGenericType AsFormalGenericType:
                    resolvedResult = new ResultType(finalTypeName, AsFormalGenericType, ValidText);

                    resolvedException = new ResultException();
                    selectedResultList = new SealableList<IParameter>();
                    featureCall = new FeatureCall();
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            if (!Success)
                return false;

            ObjectType.FillResultPath(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, Query.ValidResultTypePath.Item);

            IsHandled = false;

            switch (resolvedFinalFeature)
            {
                case IConstantFeature AsConstantFeature:
                    IExpression ConstantValue = (IExpression)AsConstantFeature.ConstantValue;
                    constantSourceList.Add(ConstantValue);
                    IsHandled = true;
                    break;

                case IFunctionFeature AsFunctionFeature:
                    Argument.AddConstantArguments(node, resolvedResult, ArgumentList, constantSourceList, out expressionConstant);
                    IsHandled = true;
                    break;

                case IAttributeFeature AsAttributeFeature:
                case IPropertyFeature AsPropertyFeature:
                case IScopeAttributeFeature AsScopeAttributeFeature:
                    expressionConstant = Expression.GetDefaultConstant(node, resolvedResult);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            return true;
        }

        private static bool ResolveDiscrete(IQueryExpression node, IErrorList errorList, IDiscrete resolvedFinalDiscrete, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ISealableList<IParameter> selectedResultList, out IFeatureCall featureCall)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new SealableList<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            selectedResultList = null;
            featureCall = null;

            // This is enforced by the caller.
            bool IsNumberTypeAvailable = Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, out ITypeName NumberTypeName, out ICompiledType NumberType);
            Debug.Assert(IsNumberTypeAvailable);

            resolvedResult = new ResultType(NumberTypeName, NumberType, resolvedFinalDiscrete.ValidDiscreteName.Item.Name);
            resolvedException = new ResultException();

            if (resolvedFinalDiscrete.NumericValue.IsAssigned)
            {
                IExpression NumericValue = (IExpression)resolvedFinalDiscrete.NumericValue.Item;
                constantSourceList.Add(NumericValue);
            }
            else
                expressionConstant = new DiscreteLanguageConstant(resolvedFinalDiscrete);

            selectedResultList = new SealableList<IParameter>();
            featureCall = new FeatureCall();

            return true;
        }
        #endregion

        #region Numbers
        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType()
        {
            foreach (IArgument Argument in ArgumentList)
                Argument.RestartNumberType();
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            foreach (IArgument Argument in ArgumentList)
                Argument.CheckNumberType(ref isChanged);

            IExpressionType Preferred = ResolvedResult.Item.Preferred;
            if (Preferred != null && Preferred.ValueType is ICompiledNumberType AsNumberType)
            {
                if (AsNumberType.NumberKind == NumberKinds.NotChecked)
                {
                    //TODO
                    AsNumberType.UpdateNumberKind(NumberKinds.NotApplicable, ref isChanged);
                }
            }
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
            foreach (IArgument Argument in ArgumentList)
                Argument.ValidateNumberType(errorList);
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString
        {
            get
            {
                string QueryString = ((IQualifiedName)Query).PathToString;
                string ArgumentString = ArgumentList.Count == 0 ? string.Empty : $"({Argument.ArgumentListToString(ArgumentList)})";

                return $"{QueryString}{ArgumentString}";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Query Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
