namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IEqualityExpression"/>.
    /// </summary>
    public interface IEqualityExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IEqualityExpression"/>.
    /// </summary>
    public class EqualityExpressionRuleTemplate : RuleTemplate<IEqualityExpression, EqualityExpressionRuleTemplate>, IEqualityExpressionRuleTemplate
    {
        #region Init
        static EqualityExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IEqualityExpression, IList<IExpressionType>>(nameof(IEqualityExpression.LeftExpression) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceSourceTemplate<IEqualityExpression, IList<IExpressionType>>(nameof(IEqualityExpression.RightExpression) + Dot + nameof(IExpression.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IEqualityExpression, IList<IExpressionType>>(nameof(IEqualityExpression.ResolvedResult)),
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
        public override bool CheckConsistency(IEqualityExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= EqualityExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out IBooleanLanguageConstant ExpressionConstant);
            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, IBooleanLanguageConstant>(ResolvedResult, ResolvedExceptions, ExpressionConstant);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IEqualityExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="expressionConstant">The constant value upon return, if any.</param>
        public static bool ResolveCompilerReferences(IEqualityExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out IBooleanLanguageConstant expressionConstant)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            expressionConstant = null;

            IExpression LeftExpression = (IExpression)node.LeftExpression;
            IExpression RightExpression = (IExpression)node.RightExpression;
            IClass EmbeddingClass = node.EmbeddingClass;
            IList<IExpressionType> LeftResult = LeftExpression.ResolvedResult.Item;
            IList<IExpressionType> RightResult = RightExpression.ResolvedResult.Item;

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
                IHashtableEx<ICompiledType, ICompiledType> EmptySubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();

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

                            if (!ObjectType.TypeConformToBase(LeftExpressionType, RightExpressionType, EmptySubstitutionTypeTable) && !ObjectType.TypeConformToBase(RightExpressionType, LeftExpressionType, EmptySubstitutionTypeTable))
                            {
                                errorList.AddError(new ErrorExpressionResultMismatch(node));
                                MismatchingResultCount++;
                            }

                            break;
                        }

                    if (!MatchingNameFound)
                    {
                        errorList.AddError(new ErrorExpressionResultMismatch(node));
                        MismatchingResultCount++;
                    }
                }

                if (MismatchingResultCount > 0)
                {
                    errorList.AddError(new ErrorExpressionResultMismatch(node));
                    return false;
                }
            }

            resolvedResult = new List<IExpressionType>()
            {
                new ExpressionType(ResultTypeName, ResultType, string.Empty)
            };

            if (LeftExpression.ExpressionConstant.IsAssigned && RightExpression.ExpressionConstant.IsAssigned)
            {
                ILanguageConstant LeftConstant = LeftExpression.ExpressionConstant.Item;
                ILanguageConstant RightConstant = RightExpression.ExpressionConstant.Item;

                if (LeftConstant.IsCompatibleWith(RightConstant))
                {
                    switch (node.Comparison)
                    {
                        case BaseNode.ComparisonType.Equal:
                            expressionConstant = new BooleanLanguageConstant(LeftConstant.IsConstantEqual(RightConstant));
                            break;

                        case BaseNode.ComparisonType.Different:
                            expressionConstant = new BooleanLanguageConstant(!LeftConstant.IsConstantEqual(RightConstant));
                            break;
                    }

                    Debug.Assert(expressionConstant != null);
                }
            }

            if (LeftExpression.ResolvedExceptions.IsAssigned && RightExpression.ResolvedExceptions.IsAssigned)
            {
                IList<IIdentifier> LeftException = LeftExpression.ResolvedExceptions.Item;
                IList<IIdentifier> RightException = RightExpression.ResolvedExceptions.Item;

                resolvedExceptions = new List<IIdentifier>();
                Expression.MergeExceptions(resolvedExceptions, LeftException);
                Expression.MergeExceptions(resolvedExceptions, RightException);
            }

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IEqualityExpression node, object data)
        {
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, IBooleanLanguageConstant>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, IBooleanLanguageConstant>)data).Item2;
            IBooleanLanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, IBooleanLanguageConstant>)data).Item3;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.SetExpressionConstant(ExpressionConstant);
        }
        #endregion
    }
}
