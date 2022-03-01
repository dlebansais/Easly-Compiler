namespace CompilerNode
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IKeywordExpression.
    /// </summary>
    public interface IKeywordExpression : IExpression, IComparableExpression
    {
        /// <summary>
        /// Gets or sets the keyword.
        /// </summary>
        BaseNode.Keyword Value { get; }
    }

    /// <summary>
    /// Compiler IKeywordExpression.
    /// </summary>
    public class KeywordExpression : BaseNode.KeywordExpression, IKeywordExpression
    {
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
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                ResolvedException = new OnceReference<IResultException>();
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

                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = ResolvedException.IsAssigned;
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
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        public bool IsExpressionEqual(IComparableExpression other)
        {
            return IsExpressionEqual(other as IKeywordExpression);
        }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        protected bool IsExpressionEqual(IKeywordExpression other)
        {
            Debug.Assert(other != null);

            bool Result = true;

            Result &= Value == other.Value;

            return Result;
        }

        /// <summary>
        /// Checks that a keyword is available in the context of the source.
        /// </summary>
        /// <param name="keyword">The keyword to check.</param>
        /// <param name="source">The source node.</param>
        /// <param name="errorList">The list of errors found if not available.</param>
        /// <param name="resultTypeName">The resulting type name upon return if available.</param>
        /// <param name="resultType">The resulting type upon return if available.</param>
        public static bool IsKeywordAvailable(BaseNode.Keyword keyword, ISource source, IErrorList errorList, out ITypeName resultTypeName, out ICompiledType resultType)
        {
            resultTypeName = null;
            resultType = null;
            bool Result = false;
            bool IsHandled = false;

            IClass EmbeddingClass = source.EmbeddingClass;

            switch (keyword)
            {
                case BaseNode.Keyword.True:
                case BaseNode.Keyword.False:
                case BaseNode.Keyword.Retry:
                    if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, source, out resultTypeName, out resultType))
                        errorList.AddError(new ErrorBooleanTypeMissing(source));
                    else
                        Result = true;

                    IsHandled = true;
                    break;

                case BaseNode.Keyword.Current:
                    Debug.Assert(EmbeddingClass.ResolvedClassTypeName.IsAssigned);
                    Debug.Assert(EmbeddingClass.ResolvedClassType.IsAssigned);
                    resultTypeName = EmbeddingClass.ResolvedClassTypeName.Item;
                    resultType = EmbeddingClass.ResolvedClassType.Item;
                    Result = true;
                    IsHandled = true;
                    break;

                case BaseNode.Keyword.Value:
                    Result = IsWithinProperty(source, errorList, out resultTypeName, out resultType);
                    IsHandled = true;
                    break;

                case BaseNode.Keyword.Result:
                    Result = IsWithinGetter(source, errorList, out resultTypeName, out resultType);
                    IsHandled = true;
                    break;

                case BaseNode.Keyword.Exception:
                    if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Exception.Guid, source, out resultTypeName, out resultType))
                        errorList.AddError(new ErrorExceptionTypeMissing(source));
                    else
                        Result = true;

                    IsHandled = true;
                    break;

                case BaseNode.Keyword.Indexer:
                    if (EmbeddingClass.ClassIndexer.IsAssigned)
                    {
                        IIndexerFeature IndexerFeature = EmbeddingClass.ClassIndexer.Item;

                        Debug.Assert(IndexerFeature.ResolvedAgentTypeName.IsAssigned);
                        Debug.Assert(IndexerFeature.ResolvedAgentType.IsAssigned);

                        resultTypeName = IndexerFeature.ResolvedAgentTypeName.Item;
                        resultType = IndexerFeature.ResolvedAgentType.Item;
                        Result = true;
                    }
                    else
                        errorList.AddError(new ErrorMissingIndexer(source));
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            return Result;
        }

        private static bool IsWithinProperty(ISource source, IErrorList errorList, out ITypeName resultTypeName, out ICompiledType resultType)
        {
            resultTypeName = null;
            resultType = null;
            bool Result = false;

            if (source.EmbeddingFeature is IPropertyFeature AsPropertyFeature)
            {
                Debug.Assert(AsPropertyFeature.ResolvedEntityTypeName.IsAssigned);
                Debug.Assert(AsPropertyFeature.ResolvedEntityType.IsAssigned);

                resultTypeName = AsPropertyFeature.ResolvedEntityTypeName.Item;
                resultType = AsPropertyFeature.ResolvedEntityType.Item;
                Result = true;
            }
            else
                errorList.AddError(new ErrorUnavailableValue(source));

            return Result;
        }

        private static bool IsWithinGetter(ISource source, IErrorList errorList, out ITypeName resultTypeName, out ICompiledType resultType)
        {
            resultTypeName = null;
            resultType = null;

            if (source.EmbeddingOverload is IQueryOverload AsQueryOverload)
                return CheckQueryConsistency(source, AsQueryOverload, errorList, out resultTypeName, out resultType);
            else if (source.EmbeddingFeature is IPropertyFeature AsPropertyFeature)
            {
                if (CheckGetterConsistency(source, AsPropertyFeature.GetterBody, errorList))
                {
                    resultTypeName = AsPropertyFeature.ResolvedEntityTypeName.Item;
                    resultType = AsPropertyFeature.ResolvedEntityType.Item;
                    return true;
                }
            }
            else if (source.EmbeddingFeature is IIndexerFeature AsIndexerFeature)
            {
                if (CheckGetterConsistency(source, AsIndexerFeature.GetterBody, errorList))
                {
                    resultTypeName = AsIndexerFeature.ResolvedEntityTypeName.Item;
                    resultType = AsIndexerFeature.ResolvedEntityType.Item;
                    return true;
                }
            }
            else
                errorList.AddError(new ErrorUnavailableResult(source));

            return false;
        }

        private static bool CheckQueryConsistency(ISource source, IQueryOverload innerQueryOverload, IErrorList errorList, out ITypeName resultTypeName, out ICompiledType resultType)
        {
            resultTypeName = null;
            resultType = null;
            bool Success = false;

            foreach (IParameter Item in innerQueryOverload.ResultTable)
                if (Item.Name == nameof(BaseNode.Keyword.Result))
                {
                    resultTypeName = Item.ResolvedParameter.ResolvedEffectiveTypeName.Item;
                    resultType = Item.ResolvedParameter.ResolvedEffectiveType.Item;
                    Success = true;
                    break;
                }

            if (!Success)
                errorList.AddError(new ErrorResultNotReturned(source));

            return Success;
        }

        private static bool CheckGetterConsistency(ISource source, IOptionalReference<BaseNode.Body> optionalGetter, IErrorList errorList)
        {
            if (source.EmbeddingBody is IBody AsBody)
            {
                if (optionalGetter.IsAssigned && AsBody == optionalGetter.Item)
                    return true;
            }

            errorList.AddError(new ErrorResultUsedOutsideGetter(source));
            return false;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IKeywordExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        public static bool ResolveCompilerReferences(IKeywordExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IExpression> constantSourceList, out ILanguageConstant expressionConstant)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new SealableList<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;

            IClass EmbeddingClass = node.EmbeddingClass;
            BaseNode.Keyword Value = node.Value;

            if (!IsKeywordAvailable(Value, node, errorList, out ITypeName KeywordTypeName, out ICompiledType KeywordType))
                return false;

            resolvedResult = new ResultType(KeywordTypeName, KeywordType, Value.ToString());

            resolvedException = new ResultException();

            bool IsHandled = false;

            switch (Value)
            {
                case BaseNode.Keyword.True:
                case BaseNode.Keyword.False:
                    expressionConstant = new BooleanLanguageConstant(Value == BaseNode.Keyword.True);
                    IsHandled = true;
                    break;

                case BaseNode.Keyword.Current:
                case BaseNode.Keyword.Value:
                case BaseNode.Keyword.Result:
                case BaseNode.Keyword.Retry:
                case BaseNode.Keyword.Exception:
                case BaseNode.Keyword.Indexer:
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

#if COVERAGE
            Debug.Assert(!node.IsComplex);
#endif

            return true;
        }
        #endregion

        #region Numbers
        /// <summary>
        /// The number kind if the constant type is a number.
        /// </summary>
        public NumberKinds NumberKind
        {
            get
            {
                if (Value == BaseNode.Keyword.Result)
                    return ResolvedResult.Item.NumberKind;
                else
                    return NumberKinds.NotApplicable;
            }
        }

        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            if (Value == BaseNode.Keyword.Result)
            {
                bool IsHandled = false;

                if (EmbeddingOverload is IQueryOverload AsQueryOverload)
                {
                    IsHandled = true;
                    ResolvedResult.Item.UpdateNumberKind(AsQueryOverload.NumberKind, ref isChanged);
                }
                else if (EmbeddingFeature is IPropertyFeature AsPropertyFeature)
                {
                    ResolvedResult.Item.UpdateNumberKind(AsPropertyFeature.NumberKind, ref isChanged);
                    IsHandled = true;
                }
                else if (EmbeddingFeature is IIndexerFeature AsIndexerFeature)
                {
                    ResolvedResult.Item.UpdateNumberKind(AsIndexerFeature.NumberKind, ref isChanged);
                    IsHandled = true;
                }

                Debug.Assert(IsHandled);
            }
            else if (Value == BaseNode.Keyword.Value)
            {
                bool IsHandled = false;

                if (EmbeddingFeature is IPropertyFeature AsPropertyFeature)
                {
                    ResolvedResult.Item.UpdateNumberKind(AsPropertyFeature.NumberKind, ref isChanged);
                    IsHandled = true;
                }
                else if (EmbeddingFeature is IIndexerFeature AsIndexerFeature)
                {
                    ResolvedResult.Item.UpdateNumberKind(AsIndexerFeature.NumberKind, ref isChanged);
                    IsHandled = true;
                }

                Debug.Assert(IsHandled);
            }
            else
                Debug.Assert(ResolvedResult.Item.NumberKind == NumberKinds.NotApplicable);
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString { get { return $"{Value}�"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Keyword Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
