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
            new ExportChangeRuleTemplate(),
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
            // No dependency rules.
            new AllInheritancesClassGroupRuleTemplate(),
            new AllInheritancesGroupRuleTemplate(),
            new AllGenericsRuleTemplate(),
            new AllImportedClassRuleTemplate(),
            new AllInheritancesInstancedRuleTemplate(),
            new AllInheritancesRuleTemplate(),
            new AllLocalDiscretesRuleTemplate(),
            new AllDiscretesRuleTemplate(),
            new AllLocalExportsRuleTemplate(),
            new AllExportsRuleTemplate(),
            new AllLocalFeaturesRuleTemplate(),
            new AllFeaturesRuleTemplate(),
            new AllLocalTypedefsRuleTemplate(),
            new AllTypedefsRuleTemplate(),
            new AssignmentInstructionRuleTemplate(),
            new CheckInstructionRuleTemplate(),
            new CommandInstructionRuleTemplate(),
            new CommandOverloadRuleTemplate(),
            new ConstraintRenameRuleTemplate(),
            new CommandOverloadTypeRuleTemplate(),
            new CreateInstructionRuleTemplate(),
            new EffectiveBodyRuleTemplate<IEffectiveBody>(),
            new EffectiveBodyRuleTemplate<IGetterEffectiveBody>(),
            new EffectiveBodyRuleTemplate<IOverloadEffectiveBody>(),
            new EffectiveBodyRuleTemplate<ISetterEffectiveBody>(),
            new ExportRuleTemplate(),
            new FeatureRuleTemplate<IAttributeFeature>(),
            new FeatureRuleTemplate<IConstantFeature>(),
            new FeatureRuleTemplate<ICreationFeature>(),
            new FeatureRuleTemplate<IFunctionFeature>(),
            new FeatureRuleTemplate<IProcedureFeature>(),
            new FeatureRuleTemplate<IPropertyFeature>(),
            new ForLoopInstructionRuleTemplate(),
            new GenericDefaultTypeRuleTemplate(),
            new GenericRuleTemplate(),
            new GenericConstraintsRuleTemplate(),
            new IndexAssignmentInstructionRuleTemplate(),
            new IndexerRuleTemplate(),
            new InheritanceClassGroupRuleTemplate(),
            new KeywordAssignmentInstructionRuleTemplate(),
            new LocalNamespaceRuleTemplate(),
            new NamespaceRuleTemplate(),
            new PrecursorIndexAssignmentInstructionRuleTemplate(),
            new PrecursorInstructionRuleTemplate(),
            new ProcedureFeatureRuleTemplate(),
            new RaiseEventInstructionRuleTemplate(),
            new ReleaseInstructionRuleTemplate(),
            new ScopeRuleTemplate(),
            new SimpleTypeClassRuleTemplate(),
            new SimpleTypeSourceRuleTemplate(),
            new ThrowInstructionRuleTemplate(),
            new TypedefRuleTemplate(),
            new WithRuleTemplate(),

            // At least one dependency rules.
            new AnchoredTypeRuleTemplate(),
            new AsLongAsInstructionRuleTemplate(),
            new AssignmentTypeArgumentRuleTemplate(),
            new AttachmentInstructionRuleTemplate(),
            new AttachmentRuleTemplate(),
            new AttributeFeatureRuleTemplate(),
            new ConditionalRuleTemplate(),
            new ConstantFeatureRuleTemplate(),
            new ConstraintConformanceRuleTemplate(),
            new ConstraintParentTypeRuleTemplate(),
            new ConstraintRuleTemplate(),
            new ContinuationRuleTemplate(),
            new CreationFeatureRuleTemplate(),
            new DebugInstructionRuleTemplate(),
            new DiscreteRuleTemplate(),
            new EntityDeclarationRuleTemplate(),
            new FunctionFeatureRuleTemplate(),
            new FunctionTypeRuleTemplate(),
            new GenericTypeArgumentsRuleTemplate(),
            new GenericTypeRuleTemplate(),
            new IfThenElseInstructionRuleTemplate(),
            new IndexerFeatureRuleTemplate(),
            new IndexerTypeRuleTemplate(),
            new InheritanceClassParentRuleTemplate(),
            new InheritanceParentTypeRuleTemplate(),
            new InheritanceRenameRuleTemplate(),
            new InspectInstructionRuleTemplate(),
            new KeywordAnchoredTypeCurrentRuleTemplate(),
            new KeywordAnchoredTypeRuleTemplate(),
            new OverLoopInstructionRuleTemplate(),
            new PositionalTypeArgumentRuleTemplate(),
            new ProcedureTypeRuleTemplate(),
            new PropertyFeatureRuleTemplate(),
            new PropertyTypeRuleTemplate(),
            new QueryOverloadResultRuleTemplate(),
            new QueryOverloadRuleTemplate(),
            new QueryOverloadTypeRuleTemplate(),
            new SimpleTypeInheritanceRuleTemplate(),
            new TupleTypeRuleTemplate(),
            new TypedefSourceRuleTemplate(),
        };
    }
}
