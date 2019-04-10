namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IIdentifier"/>.
    /// </summary>
    public interface IIdentifierRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIdentifier"/>.
    /// </summary>
    /// <typeparam name="TIdentifier">The identifier type on which the rule applies.</typeparam>
    public interface IIdentifierRuleTemplate<TIdentifier> : IRuleTemplate<TIdentifier, IdentifierRuleTemplate<TIdentifier>>, IRuleTemplate
        where TIdentifier : IIdentifier
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIdentifier"/>.
    /// </summary>
    /// <typeparam name="TIdentifier">The identifier type on which the rule applies.</typeparam>
    public class IdentifierRuleTemplate<TIdentifier> : RuleTemplate<TIdentifier, IdentifierRuleTemplate<TIdentifier>>, IIdentifierRuleTemplate<TIdentifier>, IIdentifierRuleTemplate
        where TIdentifier : IIdentifier
    {
        #region Init
        static IdentifierRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new StringSourceTemplate<TIdentifier>(nameof(IIdentifier.Text)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<TIdentifier, string>(nameof(IIdentifier.ValidText)),
            };
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        public override bool CheckConsistency(TIdentifier node, out object data)
        {
            bool Success = true;
            data = null;

            if (!StringValidation.IsValidIdentifier(node, node.Text, out string ValidText, out IErrorStringValidity StringError))
            {
                Success = false;
                AddSourceError(StringError);
            }
            else
                data = ValidText;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(TIdentifier node, object data)
        {
            string ValidText = data as string;
            Debug.Assert(StringValidation.IsValidIdentifier(ValidText));

            node.ValidText.Item = ValidText;
        }
        #endregion
    }
}
