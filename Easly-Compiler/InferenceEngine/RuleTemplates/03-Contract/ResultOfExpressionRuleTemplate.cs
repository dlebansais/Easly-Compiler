﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IResultOfExpression"/>.
    /// </summary>
    public interface IResultOfExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IResultOfExpression"/>.
    /// </summary>
    public class ResultOfExpressionRuleTemplate : RuleTemplate<IResultOfExpression, ResultOfExpressionRuleTemplate>, IResultOfExpressionRuleTemplate
    {
        #region Init
        static ResultOfExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IResultOfExpression, IList<IExpressionType>>(nameof(IResultOfExpression.Source) + Dot + nameof(IExpression.ResolvedResult)),
                new OnceReferenceSourceTemplate<IResultOfExpression, IList<IIdentifier>>(nameof(IResultOfExpression.Source) + Dot + nameof(IExpression.ResolvedExceptions)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IResultOfExpression, IList<IExpressionType>>(nameof(IResultOfExpression.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IResultOfExpression, IList<IIdentifier>>(nameof(IResultOfExpression.ResolvedExceptions)),
                new UnsealedListDestinationTemplate<IResultOfExpression, IExpression>(nameof(IResultOfExpression.ConstantSourceList)),
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
        public override bool CheckConsistency(IResultOfExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= ResultOfExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant);

            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IResultOfExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        public static bool ResolveCompilerReferences(IResultOfExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;

            IExpression Source = (IExpression)node.Source;
            IList<IExpressionType> ResolvedSourceResult = Source.ResolvedResult.Item;

            resolvedResult = new List<IExpressionType>();

            foreach (IExpressionType Item in ResolvedSourceResult)
                if (Item.Name == nameof(BaseNode.Keyword.Result))
                {
                    resolvedResult.Add(Item);
                    break;
                }

            if (resolvedResult.Count == 0)
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }

            constantSourceList.Add(Source);

            resolvedExceptions = Source.ResolvedExceptions.Item;

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IResultOfExpression node, object data)
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
