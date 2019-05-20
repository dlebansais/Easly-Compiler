namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IUnaryNotExpression"/>.
    /// </summary>
    public interface IUnaryNotExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IUnaryNotExpression"/>.
    /// </summary>
    public class UnaryNotExpressionRuleTemplate : RuleTemplate<IUnaryNotExpression, UnaryNotExpressionRuleTemplate>, IUnaryNotExpressionRuleTemplate
    {
        #region Init
        static UnaryNotExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IUnaryNotExpression, IList<IExpressionType>>(nameof(IUnaryNotExpression.RightExpression) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceSourceTemplate<IUnaryNotExpression, IList<IIdentifier>>(nameof(IUnaryNotExpression.RightExpression) + Dot + nameof(IExpression.ResolvedExceptions)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IUnaryNotExpression, IList<IExpressionType>>(nameof(IUnaryNotExpression.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IUnaryNotExpression, IList<IIdentifier>>(nameof(IUnaryNotExpression.ResolvedExceptions)),
                new UnsealedListDestinationTemplate<IUnaryNotExpression, IExpression>(nameof(IUnaryNotExpression.ConstantSourceList)),
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
        public override bool CheckConsistency(IUnaryNotExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= UnaryNotExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant);
            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IUnaryNotExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The constant value upon return, if any.</param>
        public static bool ResolveCompilerReferences(IUnaryNotExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;

            IExpression RightExpression = (IExpression)node.RightExpression;
            IClass EmbeddingClass = node.EmbeddingClass;

            bool IsRightClassType = Expression.GetClassTypeOfExpression(RightExpression, errorList, out IClassType RightExpressionClassType);
            if (!IsRightClassType)
                return false;

            Expression.IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, node, out ITypeName BooleanTypeName, out ICompiledType BooleanType);
            Expression.IsLanguageTypeAvailable(LanguageClasses.Event.Guid, node, out ITypeName EventTypeName, out ICompiledType EventType);

            if (RightExpressionClassType != BooleanType && RightExpressionClassType != EventType)
            {
                errorList.AddError(new ErrorBooleanTypeMissing(node));
                return false;
            }

            constantSourceList.Add(RightExpression);

            resolvedResult = new List<IExpressionType>()
            {
                new ExpressionType(BooleanTypeName, BooleanType, string.Empty)
            };

            List<IIdentifier> MergedExceptionList = new List<IIdentifier>();
            MergedExceptionList.AddRange(RightExpression.ResolvedExceptions.Item);
            resolvedExceptions = MergedExceptionList;

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IUnaryNotExpression node, object data)
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
