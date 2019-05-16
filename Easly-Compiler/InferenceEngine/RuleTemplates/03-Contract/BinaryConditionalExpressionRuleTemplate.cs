namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IBinaryConditionalExpression"/>.
    /// </summary>
    public interface IBinaryConditionalExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBinaryConditionalExpression"/>.
    /// </summary>
    public class BinaryConditionalExpressionRuleTemplate : RuleTemplate<IBinaryConditionalExpression, BinaryConditionalExpressionRuleTemplate>, IBinaryConditionalExpressionRuleTemplate
    {
        #region Init
        static BinaryConditionalExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IBinaryConditionalExpression, IList<IExpressionType>>(nameof(IBinaryConditionalExpression.LeftExpression) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceSourceTemplate<IBinaryConditionalExpression, IList<IIdentifier>>(nameof(IBinaryConditionalExpression.LeftExpression) + Dot + nameof(IExpression.ResolvedExceptions)),
                new OnceReferenceSourceTemplate<IBinaryConditionalExpression, IList<IExpressionType>>(nameof(IBinaryConditionalExpression.RightExpression) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceSourceTemplate<IBinaryConditionalExpression, IList<IIdentifier>>(nameof(IBinaryConditionalExpression.RightExpression) + Dot + nameof(IExpression.ResolvedExceptions)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IBinaryConditionalExpression, IList<IExpressionType>>(nameof(IBinaryConditionalExpression.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IBinaryConditionalExpression, IList<IIdentifier>>(nameof(IBinaryConditionalExpression.ResolvedExceptions)),
                new UnsealedListDestinationTemplate<IBinaryConditionalExpression, IExpression>(nameof(IBinaryConditionalExpression.ConstantSourceList)),
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
        public override bool CheckConsistency(IBinaryConditionalExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= BinaryConditionalExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant);
            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IBinaryConditionalExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The constant value upon return, if any.</param>
        public static bool ResolveCompilerReferences(IBinaryConditionalExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;

            IExpression LeftExpression = (IExpression)node.LeftExpression;
            BaseNode.ConditionalTypes Conditional = node.Conditional;
            IExpression RightExpression = (IExpression)node.RightExpression;
            IClass EmbeddingClass = node.EmbeddingClass;

            bool IsLeftClassType = Expression.GetClassTypeOfExpression(LeftExpression, errorList, out IClassType LeftExpressionClassType);
            bool IsRightClassType = Expression.GetClassTypeOfExpression(RightExpression, errorList, out IClassType RightExpressionClassType);
            if (!IsLeftClassType || !IsRightClassType)
                return false;

            if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, node, out ITypeName BooleanTypeName, out ICompiledType BooleanType))
            {
                errorList.AddError(new ErrorBooleanTypeMissing(node));
                return false;
            }

            if (LeftExpressionClassType != BooleanType)
            {
                errorList.AddError(new ErrorInvalidExpression(LeftExpression));
                return false;
            }

            if (RightExpressionClassType != BooleanType)
            {
                errorList.AddError(new ErrorInvalidExpression(RightExpression));
                return false;
            }

            constantSourceList.Add(LeftExpression);
            constantSourceList.Add(RightExpression);

            resolvedResult = new List<IExpressionType>()
            {
                new ExpressionType(BooleanTypeName, BooleanType, string.Empty)
            };

            List<IIdentifier> MergedExceptionList = new List<IIdentifier>();
            MergedExceptionList.AddRange(LeftExpression.ResolvedExceptions.Item);
            MergedExceptionList.AddRange(RightExpression.ResolvedExceptions.Item);
            resolvedExceptions = MergedExceptionList;

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IBinaryConditionalExpression node, object data)
        {
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant>)data).Item4;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();
        }
        #endregion
    }
}
