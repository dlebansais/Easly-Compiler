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
    public interface IKeywordExpression : BaseNode.IKeywordExpression, IExpression
    {
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
                ResolvedResult = new OnceReference<IList<IExpressionType>>();
                ExpressionConstant = new OnceReference<ILanguageConstant>();
                ResolvedExceptions = new OnceReference<IList<IIdentifier>>();
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
                IsResolved = ResolvedResult.IsAssigned && ResolvedExceptions.IsAssigned;

                Debug.Assert(!ExpressionConstant.IsAssigned || IsResolved);

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
        public OnceReference<IList<IExpressionType>> ResolvedResult { get; private set; } = new OnceReference<IList<IExpressionType>>();

        /// <summary>
        /// Specific constant number.
        /// </summary>
        public OnceReference<ILanguageConstant> ExpressionConstant { get; private set; } = new OnceReference<ILanguageConstant>();

        /// <summary>
        /// List of exceptions the expression can throw.
        /// </summary>
        public OnceReference<IList<IIdentifier>> ResolvedExceptions { get; private set; } = new OnceReference<IList<IIdentifier>>();

        /// <summary>
        /// Sets the <see cref="IExpression.ExpressionConstant"/> property.
        /// </summary>
        /// <param name="expressionConstant">The expression constant.</param>
        public void SetExpressionConstant(ILanguageConstant expressionConstant)
        {
            Debug.Assert(!ExpressionConstant.IsAssigned);

            bool IsHandled = false;

            switch (Value)
            {
                case BaseNode.Keyword.True:
                case BaseNode.Keyword.False:
                    IBooleanLanguageConstant BooleanConstant = expressionConstant as IBooleanLanguageConstant;
                    Debug.Assert(BooleanConstant != null && BooleanConstant.Value.HasValue);
                    Debug.Assert(BooleanConstant.Value.Value == (Value == BaseNode.Keyword.True));

                    ExpressionConstant.Item = BooleanConstant;
                    IsHandled = true;
                    break;

                case BaseNode.Keyword.Current:
                case BaseNode.Keyword.Value:
                case BaseNode.Keyword.Result:
                case BaseNode.Keyword.Retry:
                case BaseNode.Keyword.Exception:
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="expression1">The first expression.</param>
        /// <param name="expression2">The second expression.</param>
        public static bool IsExpressionEqual(IKeywordExpression expression1, IKeywordExpression expression2)
        {
            bool Result = true;

            Result &= expression1.Value == expression2.Value;

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
                    resultTypeName = Item.ResolvedParameter.ResolvedFeatureTypeName.Item;
                    resultType = Item.ResolvedParameter.ResolvedFeatureType.Item;
                    Success = true;
                    break;
                }

            if (!Success)
                errorList.AddError(new ErrorResultNotReturned(source));

            return Success;
        }

        private static bool CheckGetterConsistency(ISource source, IOptionalReference<BaseNode.IBody> optionalGetter, IErrorList errorList)
        {
            if (source.EmbeddingBody is IEffectiveBody AsEffectiveBody)
            {
                if (optionalGetter.IsAssigned && AsEffectiveBody == optionalGetter.Item)
                    return true;
            }

            errorList.AddError(new ErrorResultUsedOutsideGetter(source));
            return false;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString { get { return $"{Value}†"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Keyword Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
