namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IAssertionTagExpression"/>.
    /// </summary>
    public interface IAssertionTagExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAssertionTagExpression"/>.
    /// </summary>
    public class AssertionTagExpressionRuleTemplate : RuleTemplate<IAssertionTagExpression, AssertionTagExpressionRuleTemplate>, IAssertionTagExpressionRuleTemplate
    {
        #region Init
        static AssertionTagExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IAssertionTagExpression, IClassType, IList<IBody>>(nameof(IClass.InheritedBodyTagListTable), TemplateClassStart<IAssertionTagExpression>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAssertionTagExpression, IList<IExpressionType>>(nameof(IAssertionTagExpression.ResolvedResult)),
                new UnsealedListDestinationTemplate<IAssertionTagExpression, IExpression>(nameof(IAssertionTagExpression.ConstantSourceList)),
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
        public override bool CheckConsistency(IAssertionTagExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= AssertionTagExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out IExpression ResolvedBooleanExpression);
            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IExpression>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant, ResolvedBooleanExpression);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IAssertionTagExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The constant value upon return, if any.</param>
        /// <param name="resolvedBooleanExpression">The expression found upon return.</param>
        public static bool ResolveCompilerReferences(IAssertionTagExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out IExpression resolvedBooleanExpression)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            resolvedBooleanExpression = null;

            IIdentifier TagIdentifier = (IIdentifier)node.TagIdentifier;
            IBody InnerBody = node.EmbeddingBody;

            if (InnerBody == null)
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }

            IAssertion InnerAssertion = node.EmbeddingAssertion;
            if (InnerAssertion == null)
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }

            if (!InnerBody.ResolvedRequireList.IsAssigned || !InnerBody.ResolvedEnsureList.IsAssigned)
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }

            bool Found = false;
            foreach (IAssertion Assertion in InnerBody.ResolvedRequireList.Item)
                if (InnerAssertion == Assertion)
                {
                    Found = true;
                    break;
                }
            foreach (IAssertion Assertion in InnerBody.ResolvedEnsureList.Item)
                if (InnerAssertion == Assertion)
                {
                    Found = true;
                    break;
                }

            Debug.Assert(Found);

            resolvedBooleanExpression = (IExpression)InnerAssertion.BooleanExpression;
            resolvedResult = resolvedBooleanExpression.ResolvedResult.Item;

            constantSourceList.Add(resolvedBooleanExpression);

            if (resolvedBooleanExpression.ResolvedExceptions.IsAssigned)
                resolvedExceptions = resolvedBooleanExpression.ResolvedExceptions.Item;

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAssertionTagExpression node, object data)
        {
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IExpression>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IExpression>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IExpression>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IExpression>)data).Item4;
            IExpression ResolvedBooleanExpression = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, IExpression>)data).Item5;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.ResolvedBooleanExpression.Item = ResolvedBooleanExpression;
        }
        #endregion
    }
}
