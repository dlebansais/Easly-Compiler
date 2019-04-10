namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IQualifiedName"/>.
    /// </summary>
    public interface IQualifiedNameRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQualifiedName"/>.
    /// </summary>
    public class QualifiedNameRuleTemplate : RuleTemplate<IQualifiedName, QualifiedNameRuleTemplate>, IQualifiedNameRuleTemplate
    {
        #region Init
        static QualifiedNameRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IQualifiedName, IList<IIdentifier>>(nameof(IQualifiedName.ValidPath)),
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
        public override bool CheckConsistency(IQualifiedName node, out object data)
        {
            bool Success = true;
            data = null;

            Debug.Assert(node.Path.Count > 0);

            List<string> ValidPath = new List<string>();

            foreach (IIdentifier Identifier in node.Path)
                if (!StringValidation.IsValidIdentifier(node, Identifier.Text, out string ValidText, out IErrorStringValidity StringError))
                {
                    Success = false;
                    AddSourceError(StringError);
                }
                else
                    ValidPath.Add(ValidText);

            if (Success)
                data = ValidPath;

            return Success;
        }

        #endregion

        #region Application
        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IQualifiedName node, object data)
        {
            List<string> StringPath = data as List<string>;
            Debug.Assert(StringPath.TrueForAll((string validText) => StringValidation.IsValidIdentifier(validText)));
            Debug.Assert(StringPath.Count == node.Path.Count);

            List<IIdentifier> ValidPath = new List<IIdentifier>();
            for (int i = 0; i < node.Path.Count; i++)
            {
                string ValidText = StringPath[i];
                IIdentifier Identifier = (IIdentifier)node.Path[i];

                Identifier.ValidText.Item = ValidText;
                ValidPath.Add(Identifier);
            }

            Debug.Assert(ValidPath.Count == node.Path.Count);

            node.ValidPath.Item = ValidPath;
        }
        #endregion
    }
}
