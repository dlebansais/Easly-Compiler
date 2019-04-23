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
            new AnchoredTypeRuleTemplate(),
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
            new ConstraintRuleTemplate(),
            new ContinuationRuleTemplate(),
            new CreateInstructionRuleTemplate(),
            new CreationFeatureRuleTemplate(),
            new DebugInstructionRuleTemplate(),
            new DiscreteRuleTemplate(),
            new EffectiveBodyRuleTemplate<IEffectiveBody>(),
            new EffectiveBodyRuleTemplate<IGetterEffectiveBody>(),
            new EffectiveBodyRuleTemplate<IOverloadEffectiveBody>(),
            new EffectiveBodyRuleTemplate<ISetterEffectiveBody>(),
            new EntityDeclarationRuleTemplate(),
            new ExportRuleTemplate(),
            new FeatureRuleTemplate<IAttributeFeature>(),
            new FeatureRuleTemplate<IConstantFeature>(),
            new FeatureRuleTemplate<ICreationFeature>(),
            new FeatureRuleTemplate<IFunctionFeature>(),
            new FeatureRuleTemplate<IProcedureFeature>(),
            new FeatureRuleTemplate<IPropertyFeature>(),
            new ForLoopInstructionRuleTemplate(),
            new FunctionFeatureRuleTemplate(),
            new FunctionTypeRuleTemplate(),
            new GenericConstraintsRuleTemplate(),
            new GenericDefaultTypeRule(),
            new GenericRuleTemplate(),
            new GenericTypeArgumentsRuleTemplate(),
            new GenericTypeRuleTemplate(),
            new IfThenElseInstructionRuleTemplate(),
            // ?? new ImportedClassRuleTemplate(),
            new IndexAssignmentInstructionRuleTemplate(),
            new IndexerFeatureRuleTemplate(),
            new IndexerRuleTemplate(),
            new IndexerTypeRuleTemplate(),
            new InheritanceClassParentRuleTemplate(),
            new InheritanceParentTypeRuleTemplate(),
            new InheritanceRenameRuleTemplate(),
            new InspectInstructionRuleTemplate(),
            new KeywordAnchoredTypeRuleTemplate(),
            new KeywordAssignmentInstructionRuleTemplate(),
            new LocalNamespaceRuleTemplate(),
            new NamespaceRuleTemplate(),
            new OverLoopInstructionRuleTemplate(),
            new PositionalTypeArgumentRuleTemplate(),
            new PrecursorIndexAssignmentInstructionRuleTemplate(),
            new PrecursorInstructionRuleTemplate(),
            new ProcedureFeatureRuleTemplate(),
            new ProcedureTypeRuleTemplate(),
            new PropertyFeatureRuleTemplate(),
            new PropertyTypeRuleTemplate(),
            new QueryOverloadRuleTemplate(),
            new QueryOverloadTypeRuleTemplate(),
            new RaiseEventInstructionRuleTemplate(),
            new ReleaseInstructionRuleTemplate(),
            new ScopeRuleTemplate(),
            new SimpleTypeClassRuleTemplate(),
            new SimpleTypeInheritanceRuleTemplate(),
            new SimpleTypeSourceRuleTemplate(),
            new ThrowInstructionRuleTemplate(),
            new TupleTypeRuleTemplate(),
            new TypedefRuleTemplate(),
            new TypedefSourceRuleTemplate(),
            new WithRuleTemplate(),
        };
    }
}
