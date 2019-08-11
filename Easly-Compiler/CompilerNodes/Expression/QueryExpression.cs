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
        OnceReference<IQueryOverload> SelectedOverload { get; }

        /// <summary>
        /// The selected overload type, if any.
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
                SelectedOverload = new OnceReference<IQueryOverload>();
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
        public OnceReference<IQueryOverload> SelectedOverload { get; private set; } = new OnceReference<IQueryOverload>();

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
        /// <param name="resolvedExpression">The result of the search.</param>
        public static bool ResolveCompilerReferences(IQueryExpression node, IErrorList errorList, out ResolvedExpression resolvedExpression)
        {
            resolvedExpression = new ResolvedExpression();

            IQualifiedName Query = (IQualifiedName)node.Query;
            IList<IArgument> ArgumentList = node.ArgumentList;
            IClass EmbeddingClass = node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;
            IList<IIdentifier> ValidPath = Query.ValidPath.Item;

            ISealableDictionary<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

            if (!ObjectType.GetQualifiedPathFinalType(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, errorList, out ICompiledFeature FinalFeature, out IDiscrete FinalDiscrete, out ITypeName FinalTypeName, out ICompiledType FinalType, out bool InheritBySideAttribute))
                return false;

            Debug.Assert(FinalFeature != null || FinalDiscrete != null);

#if COVERAGE
            Debug.Assert(!node.IsComplex);
#endif

            if (FinalFeature != null)
            {
                resolvedExpression.ResolvedFinalFeature = FinalFeature;
                return ResolveFeature(node, errorList, FinalFeature, FinalTypeName, FinalType, ref resolvedExpression);
            }
            else
            {
                Debug.Assert(FinalDiscrete != null);

                resolvedExpression.ResolvedFinalDiscrete = FinalDiscrete;
                return ResolveDiscrete(node, errorList, FinalDiscrete, ref resolvedExpression);
            }
        }

        private static bool ResolveFeature(IQueryExpression node, IErrorList errorList, ICompiledFeature resolvedFinalFeature, ITypeName finalTypeName, ICompiledType finalType, ref ResolvedExpression resolvedExpression)
        {
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

                    // IScopeAttributeFeature is the case of an agent.
                    IFunctionFeature AsFunctionFeature = resolvedFinalFeature as IFunctionFeature;
                    IScopeAttributeFeature AsScopeAttributeFeature = resolvedFinalFeature as IScopeAttributeFeature;
                    Debug.Assert(AsFunctionFeature != null || AsScopeAttributeFeature != null);

                    foreach (IQueryOverloadType Overload in AsFunctionType.OverloadList)
                        ParameterTableList.Add(Overload.ParameterTable);

                    int SelectedIndex;
                    if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, TypeArgumentStyle, errorList, node, out SelectedIndex))
                        return false;

                    resolvedExpression.SelectedOverloadType = AsFunctionType.OverloadList[SelectedIndex];
                    resolvedExpression.ResolvedResult = new ResultType(resolvedExpression.SelectedOverloadType.ResultTypeList);
                    resolvedExpression.ResolvedException = new ResultException(resolvedExpression.SelectedOverloadType.ExceptionIdentifierList);

                    if (AsFunctionFeature != null)
                    {
                        Debug.Assert(AsFunctionFeature.OverloadList.Count == AsFunctionType.OverloadList.Count);
                        Debug.Assert(AsFunctionFeature.ResolvedAgentType.IsAssigned);
                        Debug.Assert(AsFunctionFeature.ResolvedAgentType.Item == AsFunctionType);

                        resolvedExpression.SelectedOverload = AsFunctionFeature.OverloadList[SelectedIndex];

                        resolvedExpression.SelectedResultList = resolvedExpression.SelectedOverload.ResultTable;
                        resolvedExpression.FeatureCall = new FeatureCall(resolvedExpression.SelectedOverload.ParameterTable, resolvedExpression.SelectedOverload.ResultTable, ArgumentList, MergedArgumentList, TypeArgumentStyle);
                    }
                    else
                    {
                        resolvedExpression.SelectedResultList = resolvedExpression.SelectedOverloadType.ResultTable;
                        resolvedExpression.FeatureCall = new FeatureCall(resolvedExpression.SelectedOverloadType.ParameterTable, resolvedExpression.SelectedOverloadType.ResultTable, ArgumentList, MergedArgumentList, TypeArgumentStyle);
                    }

                    IsHandled = true;
                    break;

                case IProcedureType AsProcedureType:
                case IIndexerType AsIndexerType:
                    errorList.AddError(new ErrorInvalidExpression(node));
                    Success = false;
                    IsHandled = true;
                    break;

                case IPropertyType AsPropertyType:
                    resolvedExpression.ResolvedResult = new ResultType(AsPropertyType.ResolvedEntityTypeName.Item, AsPropertyType.ResolvedEntityType.Item, ValidText);

                    resolvedExpression.ResolvedException = new ResultException(AsPropertyType.GetExceptionIdentifierList);
                    resolvedExpression.SelectedResultList = new SealableList<IParameter>();
                    resolvedExpression.FeatureCall = new FeatureCall();
                    IsHandled = true;
                    break;

                case IClassType AsClassType:
                case ITupleType AsTupleType:
                    resolvedExpression.ResolvedResult = new ResultType(finalTypeName, finalType, ValidText);

                    resolvedExpression.ResolvedException = new ResultException();
                    resolvedExpression.SelectedResultList = new SealableList<IParameter>();
                    resolvedExpression.FeatureCall = new FeatureCall();
                    IsHandled = true;
                    break;

                case IFormalGenericType AsFormalGenericType:
                    resolvedExpression.ResolvedResult = new ResultType(finalTypeName, AsFormalGenericType, ValidText);

                    resolvedExpression.ResolvedException = new ResultException();
                    resolvedExpression.SelectedResultList = new SealableList<IParameter>();
                    resolvedExpression.FeatureCall = new FeatureCall();
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
                    resolvedExpression.ConstantSourceList.Add(ConstantValue);
                    IsHandled = true;
                    break;

                case IFunctionFeature AsFunctionFeature:
                    Argument.AddConstantArguments(node, resolvedExpression.ResolvedResult, ArgumentList, resolvedExpression.ConstantSourceList, out ILanguageConstant ExpressionConstant);
                    resolvedExpression.ExpressionConstant = ExpressionConstant;
                    IsHandled = true;
                    break;

                case IAttributeFeature AsAttributeFeature:
                case IPropertyFeature AsPropertyFeature:
                case IScopeAttributeFeature AsScopeAttributeFeature:
                    resolvedExpression.ExpressionConstant = Expression.GetDefaultConstant(node, resolvedExpression.ResolvedResult);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            return true;
        }

        private static bool ResolveDiscrete(IQueryExpression node, IErrorList errorList, IDiscrete resolvedFinalDiscrete, ref ResolvedExpression resolvedExpression)
        {
            // This is enforced by the caller.
            bool IsNumberTypeAvailable = Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, out ITypeName NumberTypeName, out ICompiledType NumberType);
            Debug.Assert(IsNumberTypeAvailable);

            resolvedExpression.ResolvedResult = new ResultType(NumberTypeName, NumberType, resolvedFinalDiscrete.ValidDiscreteName.Item.Name);
            resolvedExpression.ResolvedException = new ResultException();

            if (resolvedFinalDiscrete.NumericValue.IsAssigned)
            {
                IExpression NumericValue = (IExpression)resolvedFinalDiscrete.NumericValue.Item;
                resolvedExpression.ConstantSourceList.Add(NumericValue);
            }
            else
                resolvedExpression.ExpressionConstant = new DiscreteLanguageConstant(resolvedFinalDiscrete);

            resolvedExpression.SelectedResultList = new SealableList<IParameter>();
            resolvedExpression.FeatureCall = new FeatureCall();

            return true;
        }
        #endregion

        #region Numbers
        /// <summary>
        /// The number kind if the constant type is a number.
        /// </summary>
        public NumberKinds NumberKind { get { return ResolvedResult.Item.NumberKind; } }

        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
            foreach (IArgument Argument in ArgumentList)
                Argument.RestartNumberType(ref isChanged);
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            foreach (IArgument Argument in ArgumentList)
                Argument.CheckNumberType(ref isChanged);

            if (SelectedOverload.IsAssigned)
            {
                ResolvedResult.Item.UpdateNumberKind(SelectedOverload.Item.NumberKind, ref isChanged);

                IDictionary<IParameter, IList<NumberKinds>> NumberArgumentTable = SelectedOverload.Item.NumberArgumentTable;

                for (int i = 0; i < FeatureCall.Item.ArgumentList.Count && i < FeatureCall.Item.ParameterList.Count; i++)
                {
                    IArgument Argument = FeatureCall.Item.ArgumentList[i];
                    IParameter Parameter = FeatureCall.Item.ParameterList[i];

                    Debug.Assert(NumberArgumentTable.ContainsKey(Parameter));
                    IList<NumberKinds> NumberKindList = NumberArgumentTable[Parameter];

                    IExpression SourceExpression = null;
                    switch (Argument)
                    {
                        case IPositionalArgument AsPositionalArgument:
                            SourceExpression = (IExpression)AsPositionalArgument.Source;
                            break;

                        case IAssignmentArgument AsAssignmentArgument:
                            SourceExpression = (IExpression)AsAssignmentArgument.Source;
                            break;
                    }

                    Debug.Assert(SourceExpression != null);

                    IExpressionType PreferredArgumentType = SourceExpression.ResolvedResult.Item.Preferred;
                    if (PreferredArgumentType != null && PreferredArgumentType.ValueType is ICompiledNumberType AsNumberArgumentType)
                    {
                        Debug.Assert(AsNumberArgumentType.NumberKind != NumberKinds.NotChecked);

                        NumberKindList.Add(AsNumberArgumentType.NumberKind);
                    }
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
