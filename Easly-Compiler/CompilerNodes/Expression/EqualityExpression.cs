﻿namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IEqualityExpression.
    /// </summary>
    public interface IEqualityExpression : BaseNode.IEqualityExpression, IExpression, IComparableExpression
    {
    }

    /// <summary>
    /// Compiler IEqualityExpression.
    /// </summary>
    public class EqualityExpression : BaseNode.EqualityExpression, IEqualityExpression
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
        public bool IsComplex { get { return true; } }

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
            return IsExpressionEqual(other as IEqualityExpression);
        }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        protected bool IsExpressionEqual(IEqualityExpression other)
        {
            Debug.Assert(other != null);

            bool Result = true;

            Result &= Expression.IsExpressionEqual((IExpression)LeftExpression, (IExpression)other.LeftExpression);
            Result &= Comparison == other.Comparison;
            Result &= Equality == other.Equality;
            Result &= Expression.IsExpressionEqual((IExpression)RightExpression, (IExpression)other.RightExpression);

            return Result;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IEqualityExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The constant value upon return, if any.</param>
        public static bool ResolveCompilerReferences(IEqualityExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IExpression> constantSourceList, out ILanguageConstant expressionConstant)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new SealableList<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;

            IExpression LeftExpression = (IExpression)node.LeftExpression;
            IExpression RightExpression = (IExpression)node.RightExpression;
            IClass EmbeddingClass = node.EmbeddingClass;
            IResultType LeftResult = LeftExpression.ResolvedResult.Item;
            IResultType RightResult = RightExpression.ResolvedResult.Item;

            if (LeftResult.Count != RightResult.Count)
            {
                errorList.AddError(new ErrorExpressionResultMismatch(node));
                return false;
            }

            if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, node, out ITypeName ResultTypeName, out ICompiledType ResultType))
            {
                errorList.AddError(new ErrorBooleanTypeMissing(node));
                return false;
            }

            if (LeftResult.Count > 1)
            {
                int MismatchingResultCount = 0;
                foreach (IExpressionType LeftItem in LeftResult)
                {
                    ICompiledType LeftExpressionType = LeftItem.ValueType;

                    bool MatchingNameFound = false;
                    foreach (IExpressionType RightItem in RightResult)
                        if (LeftItem.Name == RightItem.Name)
                        {
                            MatchingNameFound = true;
                            ICompiledType RightExpressionType = RightItem.ValueType;

                            if (!ObjectType.TypeConformToBase(LeftExpressionType, RightExpressionType, isConversionAllowed: true) && !ObjectType.TypeConformToBase(RightExpressionType, LeftExpressionType, isConversionAllowed: true))
                                MismatchingResultCount++;

                            break;
                        }

                    if (!MatchingNameFound)
                        MismatchingResultCount++;
                }

                if (MismatchingResultCount > 0)
                {
                    errorList.AddError(new ErrorExpressionResultMismatch(node));
                    return false;
                }
            }

            resolvedResult = new ResultType(ResultTypeName, ResultType, string.Empty);

            constantSourceList.Add(LeftExpression);
            constantSourceList.Add(RightExpression);

            resolvedException = new ResultException();
            ResultException.Merge(resolvedException, LeftExpression.ResolvedException);
            ResultException.Merge(resolvedException, RightExpression.ResolvedException);

#if COVERAGE
            Debug.Assert(node.IsComplex);
#endif

            return true;
        }
        #endregion

        #region Numbers
        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType()
        {
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
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
        public string ExpressionToString
        {
            get
            {
                string ComparisonString = Comparison == BaseNode.ComparisonType.Equal ? "=" : "≠";
                string EqualityString = Equality == BaseNode.EqualityType.Physical ? string.Empty : "/deep";

                IExpression Expression;

                Expression = (IExpression)LeftExpression;
                string LeftExpressionString = Expression.IsComplex ? $"({Expression.ExpressionToString})" : Expression.ExpressionToString;

                Expression = (IExpression)RightExpression;
                string RightExpressionString = Expression.IsComplex ? $"({Expression.ExpressionToString})" : Expression.ExpressionToString;

                return $"{LeftExpressionString} {ComparisonString}{EqualityString} {RightExpressionString}";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Equality Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
