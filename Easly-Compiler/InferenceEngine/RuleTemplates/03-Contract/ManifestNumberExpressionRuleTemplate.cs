namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IManifestNumberExpression"/>.
    /// </summary>
    public interface IManifestNumberExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IManifestNumberExpression"/>.
    /// </summary>
    public class ManifestNumberExpressionRuleTemplate : RuleTemplate<IManifestNumberExpression, ManifestNumberExpressionRuleTemplate>, IManifestNumberExpressionRuleTemplate
    {
        #region Init
        static ManifestNumberExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IManifestNumberExpression, IList<IExpressionType>>(nameof(IManifestNumberExpression.ResolvedResult)),
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
        public override bool CheckConsistency(IManifestNumberExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= ManifestNumberExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out INumberLanguageConstant ExpressionConstant);

            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, INumberLanguageConstant>(ResolvedResult, ResolvedExceptions, ExpressionConstant);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IManifestNumberExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="expressionConstant">The constant type upon return.</param>
        public static bool ResolveCompilerReferences(IManifestNumberExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out INumberLanguageConstant expressionConstant)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            expressionConstant = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            string NumberText = node.ValidText.Item;
            IHashtableEx<ITypeName, ICompiledType> TypeTable = EmbeddingClass.TypeTable;

            if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, out ITypeName NumberTypeName, out ICompiledType NumberType))
            {
                errorList.AddError(new ErrorNumberTypeMissing(node));
                return false;
            }

            resolvedResult = new List<IExpressionType>()
            {
                new ExpressionType(NumberTypeName, NumberType, string.Empty)
            };

            resolvedExceptions = new List<IIdentifier>();

            BaseNodeHelper.IFormattedNumber FormattedNumber = BaseNodeHelper.FormattedNumber.Parse(NumberText, false);
            Debug.Assert(string.IsNullOrEmpty(FormattedNumber.InvalidText));

            expressionConstant = new NumberLanguageConstant(FormattedNumber.Canonical);

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IManifestNumberExpression node, object data)
        {
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, INumberLanguageConstant>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, INumberLanguageConstant>)data).Item2;
            INumberLanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, INumberLanguageConstant>)data).Item3;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.SetExpressionConstant(ExpressionConstant);
        }
        #endregion
    }
}
