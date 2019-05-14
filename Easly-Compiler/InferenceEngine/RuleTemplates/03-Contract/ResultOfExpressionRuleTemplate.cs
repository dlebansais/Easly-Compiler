namespace EaslyCompiler
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
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IResultOfExpression, IList<IExpressionType>>(nameof(IResultOfExpression.ResolvedResult)),
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

            Success &= ResultOfExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out ILanguageConstant ResultNumberConstant, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions);

            if (Success)
                data = new Tuple<ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>(ResultNumberConstant, ResolvedResult, ResolvedExceptions);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IResultOfExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resultNumberConstant">The expression constant upon return.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        public static bool ResolveCompilerReferences(IResultOfExpression node, IErrorList errorList, out ILanguageConstant resultNumberConstant, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions)
        {
            resultNumberConstant = null;
            resolvedResult = null;
            resolvedExceptions = null;

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

            if (Source.NumberConstant.IsAssigned)
                resultNumberConstant = Source.NumberConstant.Item;

            if (Source.ResolvedExceptions.IsAssigned)
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
            ILanguageConstant ResultNumberConstant = ((Tuple<ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item1;
            IList<IExpressionType> ResolvedResult = ((Tuple<ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item2;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item3;

            node.ResolvedResult.Item = ResolvedResult;
            //node.SetIsConstant(IsResultConstant, ResultNumberConstant);
        }
        #endregion
    }
}
