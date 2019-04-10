namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IManifestStringExpression"/>.
    /// </summary>
    public interface IManifestStringTextRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IManifestStringExpression"/>.
    /// </summary>
    public class ManifestStringTextRuleTemplate : RuleTemplate<IManifestStringExpression, ManifestStringTextRuleTemplate>, IManifestStringTextRuleTemplate
    {
        #region Init
        static ManifestStringTextRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IManifestStringExpression, string>(nameof(IManifestStringExpression.ValidText)),
            };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        public override bool CheckConsistency(IManifestStringExpression node, out object data)
        {
            bool Success = true;
            data = null;

            if (!StringValidation.IsValidManifestString(node, node.Text, out string ValidText, out IErrorStringValidity StringError))
            {
                Success = false;
                AddSourceError(StringError);
            }
            else
                data = ValidText;

            return Success;
        }

        #endregion

        #region Application
        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IManifestStringExpression node, object data)
        {
            string ValidText = data as string;
            Debug.Assert(StringValidation.IsValidIdentifier(ValidText));

            node.ValidText.Item = ValidText;
        }
        #endregion
    }
}
