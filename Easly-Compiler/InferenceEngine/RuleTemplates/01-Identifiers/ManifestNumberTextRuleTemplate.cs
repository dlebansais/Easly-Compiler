namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using FormattedNumber;

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
            bool Success = true;
            data = null;

            if (!StringValidation.IsValidIdentifier(node, node.Text, out string ValidText, out IErrorStringValidity StringError))
                Success = false;
            else
            {
                FormattedNumber fn = Parser.Parse(ValidText);
                if (!string.IsNullOrEmpty(fn.InvalidText))
                    Success = false;
            }

            if (!Success)
                AddSourceError(new ErrorInvalidManifestNumber(node, node.Text));
            else
                data = ValidText;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IManifestNumberExpression node, object data)
        {
            string ValidText = data as string;
            Debug.Assert(StringValidation.IsValidIdentifier(ValidText));
            Debug.Assert(string.IsNullOrEmpty(Parser.Parse(ValidText).InvalidText));

            node.ValidText.Item = ValidText;
        }
        #endregion
    }
}
