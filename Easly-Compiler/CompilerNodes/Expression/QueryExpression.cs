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
    public interface IQueryExpression : BaseNode.IQueryExpression, IExpression, INodeWithReplicatedBlocks
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
        /// The list of resolved arguments.
        /// </summary>
        OnceReference<List<ExpressionType>> ResolvedArgumentList { get; }

        /// <summary>
        /// The resolved discrete at the end of the path.
        /// </summary>
        OnceReference<IDiscrete> ResolvedFinalDiscrete { get; }
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
        public void Reset(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ResolvedResult = new OnceReference<IResultType>();
                ResolvedExceptions = new OnceReference<IList<IIdentifier>>();
                ConstantSourceList = new ListTableEx<IExpression>();
                ExpressionConstant = new OnceReference<ILanguageConstant>();
                ResolvedFinalFeature = new OnceReference<ICompiledFeature>();
                ResolvedFinalDiscrete = new OnceReference<IDiscrete>();
                ResolvedArgumentList = new OnceReference<List<ExpressionType>>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsResolved = false;

            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ExpressionConstant.IsAssigned;

                Debug.Assert(ResolvedResult.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedExceptions.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedFinalFeature.IsAssigned || ResolvedFinalDiscrete.IsAssigned || !IsResolved);

                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IExpression
        /// <summary>
        /// Types of expression results.
        /// </summary>
        public OnceReference<IResultType> ResolvedResult { get; private set; } = new OnceReference<IResultType>();

        /// <summary>
        /// List of exceptions the expression can throw.
        /// </summary>
        public OnceReference<IList<IIdentifier>> ResolvedExceptions { get; private set; } = new OnceReference<IList<IIdentifier>>();

        /// <summary>
        /// The list of sources for a constant, if any.
        /// </summary>
        public ListTableEx<IExpression> ConstantSourceList { get; private set; } = new ListTableEx<IExpression>();

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
        /// The list of resolved arguments.
        /// </summary>
        public OnceReference<List<ExpressionType>> ResolvedArgumentList { get; private set; } = new OnceReference<List<ExpressionType>>();

        /// <summary>
        /// The resolved discrete at the end of the path.
        /// </summary>
        public OnceReference<IDiscrete> ResolvedFinalDiscrete { get; private set; } = new OnceReference<IDiscrete>();

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="expression1">The first expression.</param>
        /// <param name="expression2">The second expression.</param>
        public static bool IsExpressionEqual(IQueryExpression expression1, IQueryExpression expression2)
        {
            bool Result = true;

            Result &= QualifiedName.IsQualifiedNameEqual((IQualifiedName)expression1.Query, (IQualifiedName)expression2.Query);
            Result &= Argument.IsArgumentListEqual(expression1.ArgumentList, expression2.ArgumentList);

            return Result;
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
        public static bool ResolveCompilerReferences(IQueryExpression node, IErrorList errorList, out IResultType resolvedResult, out IList<IIdentifier> resolvedExceptions, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ICompiledFeature resolvedFinalFeature, out IDiscrete resolvedFinalDiscrete, out ListTableEx<IParameter> selectedParameterList, out ListTableEx<IParameter> selectedResultList, out List<IExpressionType> resolvedArgumentList)
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
                        resolvedResult = new ResultType(SelectedOverload.ResultTypeList);
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
                        resolvedResult = new ResultType(AsPropertyType.ResolvedEntityTypeName.Item, AsPropertyType.ResolvedEntityType.Item, ValidText);

                        resolvedExceptions = AsPropertyType.GetExceptionIdentifierList;
                        selectedParameterList = new ListTableEx<IParameter>();
                        selectedResultList = new ListTableEx<IParameter>();
                        resolvedArgumentList = new List<IExpressionType>();
                        IsHandled = true;
                        break;

                    case IClassType AsClassType:
                        resolvedResult = new ResultType(FinalTypeName, AsClassType, ValidText);

                        resolvedExceptions = new List<IIdentifier>();
                        selectedParameterList = new ListTableEx<IParameter>();
                        selectedResultList = new ListTableEx<IParameter>();
                        resolvedArgumentList = MergedArgumentList;
                        IsHandled = true;
                        break;

                    case IFormalGenericType AsFormalGenericType:
                        resolvedResult = new ResultType(FinalTypeName, AsFormalGenericType, ValidText);

                        resolvedExceptions = new List<IIdentifier>();
                        selectedParameterList = new ListTableEx<IParameter>();
                        selectedResultList = new ListTableEx<IParameter>();
                        resolvedArgumentList = MergedArgumentList;
                        IsHandled = true;
                        break;

                    case ITupleType AsTupleType:
                        resolvedResult = new ResultType(FinalTypeName, AsTupleType, ValidText);

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

                resolvedResult = new ResultType(NumberTypeName, NumberType, FinalDiscrete.ValidDiscreteName.Item.Name);

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
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString { get { return $"{((IQualifiedName)Query).PathToString}({Argument.ArgumentListToString(ArgumentList)})"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Query Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
