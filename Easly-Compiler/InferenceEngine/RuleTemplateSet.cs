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
            new QualifiedNameRuleTemplate(),
            new RenameRuleTemplate(),
        };

        /// <summary>
        /// Rules for the types inference pass.
        /// </summary>
        public static IList<IRuleTemplate> Types { get; } = new List<IRuleTemplate>()
        {
            new AllDiscretesRuleTemplate(),
            new AllExportsRuleTemplate(),
            new AllFeaturesRuleTemplate(),
            new AllGenericsRuleTemplate(),
            new AllImportedClassRuleTemplate(),
            new AllInheritancesInstancedRuleTemplate(),
            new AllInheritancesRuleTemplate(),
            new AllLocalDiscretesRuleTemplate(),
            new AllLocalExportsRuleTemplate(),
            new AllLocalFeaturesRuleTemplate(),
            new AllLocalTypedefsRuleTemplate(),
            new AllTypedefsRuleTemplate(),
            new AnchroredTypeRuleTemplate(),
            new AsLongAsInstructionRuleTemplate(),
            new AssignmentInstructionRuleTemplate(),
            new AssignmentTypeArgumentRuleTemplate(),
            new AttachmentInstructionRuleTemplate(),
            new AttachmentRuleTemplate(),
            new AttributeFeatureRuleTemplate(),
            new CheckInstructionRuleTemplate(),
            new CommandInstructionRuleTemplate(),
            new CommandOverloadRuleTemplate(),
            new CommandOverloadTypeRuleTemplate(),
            new ConditionalRuleTemplate(),
            new ConstantFeatureRuleTemplate(),
            new ConstraintConformanceRuleTemplate(),
            new ConstraintParentTypeRuleTemplate(),
            new ConstraintRenameRuleTemplate(),
            new ContinuationRuleTemplate(),

            new GenericRuleTemplate(),
        };
    }
}
