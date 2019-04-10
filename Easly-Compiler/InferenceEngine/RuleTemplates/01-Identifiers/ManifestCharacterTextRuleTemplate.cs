namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IManifestCharacterExpression"/>.
    /// </summary>
    public interface IManifestCharacterTextRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IManifestCharacterExpression"/>.
    /// </summary>
    public class ManifestCharacterTextRuleTemplate : RuleTemplate<IManifestCharacterExpression, ManifestCharacterTextRuleTemplate>, IManifestCharacterTextRuleTemplate
    {
        #region Init
        static ManifestCharacterTextRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IManifestCharacterExpression, string>(nameof(IManifestCharacterExpression.ValidText)),
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
        public override bool CheckConsistency(IManifestCharacterExpression node, out object data)
        {
            bool Success = true;
            data = null;

            if (!StringValidation.IsValidManifestString(node, node.Text, out string ValidText, out IErrorStringValidity StringError))
            {
                Success = false;
                AddSourceError(StringError);
            }
            else
            {
                if (ValidText.Length != 1)
                {
                    Success = false;
                    AddSourceError(new ErrorInvalidManifestChraracter(node, ValidText));
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
        public override void Apply(IManifestCharacterExpression node, object data)
        {
            string ValidText = data as string;
            Debug.Assert(StringValidation.IsValidIdentifier(ValidText));
            Debug.Assert(ValidText.Length == 1);

            node.ValidText.Item = ValidText;
        }
        #endregion
    }
}
