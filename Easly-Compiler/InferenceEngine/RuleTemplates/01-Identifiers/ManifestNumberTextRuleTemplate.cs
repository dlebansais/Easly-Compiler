namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using BaseNodeHelper;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IManifestNumberExpression"/>.
    /// </summary>
    public interface IManifestNumberTextRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IManifestNumberExpression"/>.
    /// </summary>
    public class ManifestNumberTextRuleTemplate : RuleTemplate<IManifestNumberExpression, ManifestNumberTextRuleTemplate>, IManifestNumberTextRuleTemplate
    {
        #region Init
        static ManifestNumberTextRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IManifestNumberExpression, string>(nameof(IManifestNumberExpression.ValidText)),
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
        public override bool CheckConsistency(IManifestNumberExpression node, out object data)
        {
            bool Success = true;
            data = null;

            if (!StringValidation.IsValidIdentifier(node, node.Text, out string ValidText, out IErrorStringValidity StringError))
            {
                Success = false;
                AddSourceError(StringError);
            }
            else
            {
                IFormattedNumber fn = FormattedNumber.Parse(ValidText);
                if (!string.IsNullOrEmpty(fn.InvalidText))
                {
                    Success = false;
                    AddSourceError(new ErrorInvalidManifestNumber(node, ValidText));
                }
                else
                    data = ValidText;
            }

            return Success;
        }

        #endregion

        #region Application
        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IManifestNumberExpression node, object data)
        {
            string ValidText = data as string;
            Debug.Assert(StringValidation.IsValidIdentifier(ValidText));
            Debug.Assert(string.IsNullOrEmpty(FormattedNumber.Parse(ValidText).InvalidText));

            node.ValidText.Item = ValidText;
        }
        #endregion
    }
}
