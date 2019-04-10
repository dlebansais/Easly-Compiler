namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// Set of <see cref="IRuleTemplate"/>.
    /// </summary>
    public static class RuleTemplateSet
    {
        /// <summary>
        /// Rules for the identifiers inference pass.
        /// </summary>
        public static IList<IRuleTemplate> Identifiers { get; } = new List<IRuleTemplate>()
        {
            // Create many derivations of IdentifierRuleTemplate to have separate static constructors, to ensure separate namespaces.
            new QualifiedNameRuleTemplate(),
            new IdentifierRuleTemplate<IIdentifier>(),
            new IdentifierRuleTemplate<IClassIdentifier>(),
            new IdentifierRuleTemplate<IClassOrExportIdentifier>(),
            new IdentifierRuleTemplate<IClassOrFeatureIdentifier>(),
            new IdentifierRuleTemplate<IExportIdentifier>(),
            new IdentifierRuleTemplate<IFeatureIdentifier>(),
            new IdentifierRuleTemplate<ILibraryIdentifier>(),
            new IdentifierRuleTemplate<IReplicateIdentifier>(),
            new IdentifierRuleTemplate<ISourceIdentifier>(),
            new IdentifierRuleTemplate<ITypeIdentifier>(),
            new ManifestCharacterTextRuleTemplate(),
            new ManifestNumberTextRuleTemplate(),
            new ManifestStringTextRuleTemplate(),
            new NameRuleTemplate(),
        };
    }
}
