namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IManifestCharacterExpression"/>.
    /// </summary>
    public interface IManifestCharacterExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IManifestCharacterExpression"/>.
    /// </summary>
    public class ManifestCharacterExpressionRuleTemplate : RuleTemplate<IManifestCharacterExpression, ManifestCharacterExpressionRuleTemplate>, IManifestCharacterExpressionRuleTemplate
    {
        #region Init
        static ManifestCharacterExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IManifestCharacterExpression, IList<IExpressionType>>(nameof(IManifestCharacterExpression.ResolvedResult)),
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
        public override bool CheckConsistency(IManifestCharacterExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= ManifestCharacterExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ILanguageConstant ExpressionConstant);

            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ILanguageConstant>(ResolvedResult, ResolvedExceptions, ExpressionConstant);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IManifestCharacterExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        public static bool ResolveCompilerReferences(IManifestCharacterExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ILanguageConstant expressionConstant)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            expressionConstant = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            string ValidText = node.ValidText.Item;
            Debug.Assert(ValidText.Length == 1);
            char ValidChar = ValidText[0];

            if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Character.Guid, node, out ITypeName CharacterTypeName, out ICompiledType CharacterType))
            {
                errorList.AddError(new ErrorCharacterTypeMissing(node));
                return false;
            }

            resolvedResult = new List<IExpressionType>()
            {
                new ExpressionType(CharacterTypeName, CharacterType, string.Empty)
            };

            resolvedExceptions = new List<IIdentifier>();
            expressionConstant = new CharacterLanguageConstant(ValidChar);

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IManifestCharacterExpression node, object data)
        {
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ILanguageConstant>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ILanguageConstant>)data).Item2;
            ILanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ILanguageConstant>)data).Item3;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.SetExpressionConstant(ExpressionConstant);
        }
        #endregion
    }
}
