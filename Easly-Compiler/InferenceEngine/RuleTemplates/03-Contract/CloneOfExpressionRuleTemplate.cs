﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="ICloneOfExpression"/>.
    /// </summary>
    public interface ICloneOfExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ICloneOfExpression"/>.
    /// </summary>
    public class CloneOfExpressionRuleTemplate : RuleTemplate<ICloneOfExpression, CloneOfExpressionRuleTemplate>, ICloneOfExpressionRuleTemplate
    {
        #region Init
        static CloneOfExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<ICloneOfExpression, IList<IExpressionType>>(nameof(ICloneOfExpression.Source) + Dot + nameof(IExpression.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<ICloneOfExpression, IList<IExpressionType>>(nameof(ICloneOfExpression.ResolvedResult)),
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
        public override bool CheckConsistency(ICloneOfExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= CloneOfExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions);

            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>>(ResolvedResult, ResolvedExceptions);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="ICloneOfExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        public static bool ResolveCompilerReferences(ICloneOfExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions)
        {
            resolvedResult = null;
            resolvedExceptions = null;

            IExpression Source = (IExpression)node.Source;
            IList<IExpressionType> ResolvedSourceResult = Source.ResolvedResult.Item;

            foreach (IExpressionType Item in ResolvedSourceResult)
            {
                if (Item.ValueType is IClassType AsClassType)
                    AsClassType.MarkAsCloned();
                else
                {
                    errorList.AddError(new ErrorClassTypeRequired(node));
                    return false;
                }
            }

            resolvedResult = ResolvedSourceResult;

            if (Source.ResolvedExceptions.IsAssigned)
                resolvedExceptions = Source.ResolvedExceptions.Item;

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(ICloneOfExpression node, object data)
        {
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>>)data).Item2;

            node.ResolvedResult.Item = ResolvedResult;
        }
        #endregion
    }
}