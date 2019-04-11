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
                new OnceReferenceCollectionSourceTemplate<IQualifiedName, IIdentifier>(nameof(IQualifiedName.Path), nameof(IIdentifier.ValidText)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IQualifiedName, IList<IIdentifier>>(nameof(IQualifiedName.ValidPath)),
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
        public override bool CheckConsistency(IQualifiedName node, out object data)
        {
            bool Success = true;
            data = null;

            Debug.Assert(node.Path.Count > 0);

            List<string> ValidPath = new List<string>();

            foreach (IIdentifier Identifier in node.Path)
            {
                Debug.Assert(StringValidation.IsValidIdentifier(node, Identifier.Text, out string ValidText, out IErrorStringValidity StringError));
                Debug.Assert(Identifier.ValidText.IsAssigned);
                Debug.Assert(Identifier.ValidText.Item == ValidText);

                ValidPath.Add(Identifier.ValidText.Item);
            }

            data = ValidPath;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IQualifiedName node, object data)
        {
            List<string> StringPath = data as List<string>;
            Debug.Assert(IsPathAssigned(StringPath, node.Path));

            IList<IIdentifier> ValidPath = new List<IIdentifier>();
            foreach (IIdentifier Identifier in node.Path)
                ValidPath.Add(Identifier);

            node.ValidPath.Item = ValidPath;
        }

        private bool IsPathAssigned(IList<string> stringPath, IList<BaseNode.IIdentifier> identifierPath)
        {
            bool Result = true;

            Result &= stringPath.Count == identifierPath.Count;

            for (int i = 0; i < stringPath.Count && i < identifierPath.Count; i++)
            {
                string StringText = stringPath[i];
                Debug.Assert(StringValidation.IsValidIdentifier(StringText));

                IIdentifier Identifier = (IIdentifier)identifierPath[i];

                Debug.Assert(Identifier.ValidText.IsAssigned);
                string IdentifierText = Identifier.ValidText.Item;
                Debug.Assert(StringValidation.IsValidIdentifier(IdentifierText));

                Result &= StringText == IdentifierText;
            }

            return Result;
        }
        #endregion
    }
}
