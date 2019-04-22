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
        public IQueryOverload EmbeddingOverload { get; private set; }

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
            EmbeddingOverload = parentSource is IQueryOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
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

            Debug.Assert(IsHandled);
            return IsResolved;
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
        /// <param name="source">The source node.</param>
        /// <param name="keyword">The keyword to check.</param>
        /// <param name="errorList">The list of errors found if not available.</param>
        /// <param name="resultTypeName">The resulting type name upon return if available.</param>
        /// <param name="resultType">The resulting type upon return if available.</param>
        public static bool IsKeywordAvailable(ISource source, BaseNode.Keyword keyword, IList<IError> errorList, out ITypeName resultTypeName, out ICompiledType resultType)
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
                    Result = IsLanguageTypeAvailable(source, LanguageClasses.Boolean.Guid, errorList, out resultTypeName, out resultType);
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
                    Result = IsLanguageTypeAvailable(source, LanguageClasses.Exception.Guid, errorList, out resultTypeName, out resultType);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            return Result;
        }

        private static bool IsLanguageTypeAvailable(ISource source, Guid guid, IList<IError> errorList, out ITypeName resultTypeName, out ICompiledType resultType)
        {
            resultTypeName = null;
            resultType = null;

            IClass EmbeddingClass = source.EmbeddingClass;

            if (!EmbeddingClass.ImportedLanguageTypeTable.ContainsKey(guid))
            {
                errorList.Add(new ErrorBooleanTypeMissing(source));
                return false;
            }

            Tuple<ITypeName, IClassType> ImportedLanguageType = EmbeddingClass.ImportedLanguageTypeTable[guid];
            resultTypeName = ImportedLanguageType.Item1;
            resultType = ImportedLanguageType.Item2;
            return true;
        }

        private static bool IsWithinProperty(ISource source, IList<IError> errorList, out ITypeName resultTypeName, out ICompiledType resultType)
        {
            resultTypeName = null;
            resultType = null;

            IPropertyFeature EmbeddingProperty = source.EmbeddingFeature as IPropertyFeature;
            if (EmbeddingProperty == null)
            {
                errorList.Add(new ErrorUnavailableValue(source));
                return false;
            }

            resultTypeName = EmbeddingProperty.ResolvedEntityTypeName.Item;
            resultType = EmbeddingProperty.ResolvedEntityType.Item;
            return true;
        }

        private static bool IsWithinGetter(ISource source, IList<IError> errorList, out ITypeName resultTypeName, out ICompiledType resultType)
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
                errorList.Add(new ErrorUnavailableResult(source));

            return false;
        }

        private static bool CheckQueryConsistency(ISource source, IQueryOverload innerQueryOverload, IList<IError> errorList, out ITypeName resultTypeName, out ICompiledType resultType)
        {
            resultTypeName = null;
            resultType = null;
            bool Success = false;

            foreach (IParameter Item in innerQueryOverload.ResultTable)
                if (Item.Name == BaseNode.Keyword.Result.ToString())
                {
                    resultTypeName = Item.ResolvedParameter.ResolvedFeatureTypeName.Item;
                    resultType = Item.ResolvedParameter.ResolvedFeatureType.Item;
                    Success = true;
                    break;
                }

            if (!Success)
                errorList.Add(new ErrorResultNotReturned(source));

            return Success;
        }

        private static bool CheckGetterConsistency(ISource source, IOptionalReference<BaseNode.IBody> optionalGetter, IList<IError> errorList)
        {
            if (source.EmbeddingBody is IEffectiveBody AsEffectiveBody)
            {
                if (optionalGetter.IsAssigned && AsEffectiveBody == optionalGetter.Item)
                    return true;
            }

            errorList.Add(new ErrorResultUsedOutsideGetter(source));
            return false;
        }
        #endregion
    }
}
