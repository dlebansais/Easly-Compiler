﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

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

            Success &= AssertionTagExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IExpression ResolvedBooleanExpression, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions);
            if (Success)
                data = new Tuple<IExpression, IList<IExpressionType>, IList<IIdentifier>>(ResolvedBooleanExpression, ResolvedResult, ResolvedExceptions);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IAssertionTagExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedBooleanExpression">The expression found upon return.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        public static bool ResolveCompilerReferences(IAssertionTagExpression node, IErrorList errorList, out IExpression resolvedBooleanExpression, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions)
        {
            resolvedBooleanExpression = null;
            resolvedResult = null;
            resolvedExceptions = null;

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
            IExpression ResolvedBooleanExpression = ((Tuple<IExpression, IList<IExpressionType>, IList<IIdentifier>>)data).Item1;
            IList<IExpressionType> ResolvedResult = ((Tuple<IExpression, IList<IExpressionType>, IList<IIdentifier>>)data).Item2;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IExpression, IList<IExpressionType>, IList<IIdentifier>>)data).Item3;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedBooleanExpression.Item = ResolvedBooleanExpression;

            if (ResolvedExceptions != null)
                node.ResolvedExceptions.Item = ResolvedExceptions;
        }
        #endregion
    }
}