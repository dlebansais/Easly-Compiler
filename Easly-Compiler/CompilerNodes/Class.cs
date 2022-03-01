namespace CompilerNode
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IClass.
    /// </summary>
    public interface IClass : INode, INodeWithReplicatedBlocks, ISource, IScopeHolder
    {
        /// <summary>
        /// Gets or sets the class name.
        /// </summary>
        BaseNode.Name EntityName { get; }

        /// <summary>
        /// Gets or sets the set this class is from.
        /// </summary>
        IOptionalReference<BaseNode.Identifier> FromIdentifier { get; }

        /// <summary>
        /// Gets or sets the class copy semantic.
        /// </summary>
        BaseNode.CopySemantic CopySpecification { get; }

        /// <summary>
        /// Gets or sets whether the class is cloneable.
        /// </summary>
        BaseNode.CloneableStatus Cloneable { get; }

        /// <summary>
        /// Gets or sets whether the class is comparable.
        /// </summary>
        BaseNode.ComparableStatus Comparable { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the class is abstract.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Gets or sets the class imports.
        /// </summary>
        BaseNode.IBlockList<BaseNode.Import> ImportBlocks { get; }

        /// <summary>
        /// Gets or sets the class generics.
        /// </summary>
        BaseNode.IBlockList<BaseNode.Generic> GenericBlocks { get; }

        /// <summary>
        /// Gets or sets the class exports.
        /// </summary>
        BaseNode.IBlockList<BaseNode.Export> ExportBlocks { get; }

        /// <summary>
        /// Gets or sets the class typedefs.
        /// </summary>
        BaseNode.IBlockList<BaseNode.Typedef> TypedefBlocks { get; }

        /// <summary>
        /// Gets or sets the class inheritances.
        /// </summary>
        BaseNode.IBlockList<BaseNode.Inheritance> InheritanceBlocks { get; }

        /// <summary>
        /// Gets or sets the class discrete values.
        /// </summary>
        BaseNode.IBlockList<BaseNode.Discrete> DiscreteBlocks { get; }

        /// <summary>
        /// Gets or sets the class replicates.
        /// </summary>
        BaseNode.IBlockList<BaseNode.ClassReplicate> ClassReplicateBlocks { get; }

        /// <summary>
        /// Gets or sets the class features.
        /// </summary>
        BaseNode.IBlockList<BaseNode.Feature> FeatureBlocks { get; }

        /// <summary>
        /// Gets or sets the class conversions.
        /// </summary>
        BaseNode.IBlockList<BaseNode.Identifier> ConversionBlocks { get; }

        /// <summary>
        /// Gets or sets the class invariants.
        /// </summary>
        BaseNode.IBlockList<BaseNode.Assertion> InvariantBlocks { get; }

        /// <summary>
        /// Gets or sets the class unique ID.
        /// </summary>
        Guid ClassGuid { get; }

        /// <summary>
        /// Gets or sets the class path.
        /// </summary>
        string ClassPath { get; }

        /// <summary>
        /// The class path with replication info.
        /// </summary>
        string FullClassPath { get; }

        /// <summary>
        /// Initializes the class path.
        /// </summary>
        void SetFullClassPath();

        /// <summary>
        /// Initializes the class path with replication info.
        /// </summary>
        /// <param name="replicationPattern">The replication pattern used.</param>
        /// <param name="source">The source text.</param>
        void SetFullClassPath(string replicationPattern, string source);

        /// <summary>
        /// The class-specific counter, for the <see cref="BaseNode.PreprocessorMacro.Counter"/> macro.
        /// </summary>
        int ClassCounter { get; }

        /// <summary>
        /// Increments <see cref="ClassCounter"/>.
        /// </summary>
        void IncrementClassCounter();

        /// <summary>
        /// The type for a pre-compiled class.
        /// </summary>
        OnceReference<ICompiledType> ResolvedAsCompiledType { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.InvariantBlocks"/>.
        /// </summary>
        IList<IAssertion> InvariantList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ConversionBlocks"/>.
        /// </summary>
        IList<IIdentifier> ConversionList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.FeatureBlocks"/>.
        /// </summary>
        IList<IFeature> FeatureList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ClassReplicateBlocks"/>.
        /// </summary>
        IList<IClassReplicate> ClassReplicateList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.DiscreteBlocks"/>.
        /// </summary>
        IList<IDiscrete> DiscreteList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.InheritanceBlocks"/>.
        /// </summary>
        IList<IInheritance> InheritanceList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.TypedefBlocks"/>.
        /// </summary>
        IList<ITypedef> TypedefList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ExportBlocks"/>.
        /// </summary>
        IList<IExport> ExportList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ImportBlocks"/>.
        /// </summary>
        IList<IImport> ImportList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.GenericBlocks"/>.
        /// </summary>
        IList<IGeneric> GenericList { get; }
        /// <summary>
        /// The class name, verified as valid.
        /// </summary>
        string ValidClassName { get; }

        /// <summary>
        /// The class source name, verified as valid.
        /// </summary>
        string ValidSourceName { get; }

        /// <summary>
        /// The list of imported libraries.
        /// </summary>
        IList<ILibrary> ImportedLibraryList { get; }

        /// <summary>
        /// The table of imported classes.
        /// </summary>
        ISealableDictionary<string, IImportedClass> ImportedClassTable { get; }

        /// <summary>
        /// True if the class is an enumeration (inherits from one of the enumeration language classes).
        /// </summary>
        bool IsEnumeration { get; }

        /// <summary>
        /// Validates the class name and class source name, and update <see cref="ValidClassName"/> and <see cref="ValidSourceName"/>.
        /// </summary>
        /// <param name="classTable">Table of valid class names and their sources, updated upon return.</param>
        /// <param name="validatedClassList">List of classes with valid names, updated upon return.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if class names are valid.</returns>
        bool CheckClassNames(ISealableDictionary<string, ISealableDictionary<string, IClass>> classTable, IList<IClass> validatedClassList, IErrorList errorList);

        /// <summary>
        /// Validate a class import clauses.
        /// </summary>
        /// <param name="libraryTable">Imported libraries.</param>
        /// <param name="classTable">Imported classes.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if imports are valid.</returns>
        bool CheckClassConsistency(ISealableDictionary<string, ISealableDictionary<string, ILibrary>> libraryTable, ISealableDictionary<string, ISealableDictionary<string, IClass>> classTable, IErrorList errorList);

        /// <summary>
        /// The table of resolved generics arguments for this class.
        /// </summary>
        ISealableDictionary<string, ICompiledType> LocalGenericTable { get; }

        /// <summary>
        /// Table of all resolved generics in this class, direct or inherited.
        /// </summary>
        ISealableDictionary<string, ICompiledType> GenericTable { get; }

        /// <summary>
        /// Table of resolved discretes defined in this class.
        /// </summary>
        ISealableDictionary<IFeatureName, IDiscrete> LocalDiscreteTable { get; }

        /// <summary>
        /// Table of all resolved discretes in this class, direct or inherited.
        /// </summary>
        ISealableDictionary<IFeatureName, IDiscrete> DiscreteTable { get; }

        /// <summary>
        /// Table of all resolved discretes in this class, direct or inherited.
        /// </summary>
        ISealableDictionary<IFeatureName, IExpression> DiscreteWithValueTable { get; }

        /// <summary>
        /// Table of resolved typedefs defined in this class.
        /// </summary>
        ISealableDictionary<IFeatureName, ITypedefType> LocalTypedefTable { get; }

        /// <summary>
        /// Table of all resolved typedefs in this class, direct or inherited.
        /// </summary>
        ISealableDictionary<IFeatureName, ITypedefType> TypedefTable { get; }

        /// <summary>
        /// Table of resolved features defined in this class.
        /// </summary>
        ISealableDictionary<IFeatureName, IFeatureInstance> LocalFeatureTable { get; }

        /// <summary>
        /// Table of all resolved features in this class, direct or inherited.
        /// </summary>
        ISealableDictionary<IFeatureName, IFeatureInstance> FeatureTable { get; }

        /// <summary>
        /// Table of inherited types by their class type.
        /// </summary>
        ISealableDictionary<IClassType, IObjectType> InheritedClassTypeTable { get; }

        /// <summary>
        /// Table of resolved namespaces defined in this class.
        /// </summary>
        ISealableDictionary<string, ISealableDictionary> LocalNamespaceTable { get; }

        /// <summary>
        /// Table of resolved exports defined in this class.
        /// </summary>
        ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> LocalExportTable { get; }

        /// <summary>
        /// Table of all resolved exports in this class, direct or inherited.
        /// </summary>
        ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> ExportTable { get; }

        /// <summary>
        /// Table of all resolved conversion procedures in this class, direct or inherited.
        /// </summary>
        ISealableDictionary<IFeatureName, ICreationFeature> ConversionFromTable { get; }

        /// <summary>
        /// Table of all resolved conversion queries in this class, direct or inherited.
        /// </summary>
        ISealableDictionary<IFeatureName, IFunctionFeature> ConversionToTable { get; }

        /// <summary>
        /// The type name for this class.
        /// </summary>
        OnceReference<ITypeName> ResolvedClassTypeName { get; }

        /// <summary>
        /// The type from this class.
        /// </summary>
        OnceReference<IClassType> ResolvedClassType { get; }

        /// <summary>
        /// List of types corresponding to each generic argument.
        /// </summary>
        IList<IClassType> GenericInstanceList { get; }

        /// <summary>
        /// Table of all types used in this class.
        /// </summary>
        ISealableDictionary<ITypeName, ICompiledType> TypeTable { get; }

        /// <summary>
        /// The class indexer, if any.
        /// </summary>
        OptionalReference<IIndexerFeature> ClassIndexer { get; }

        /// <summary>
        /// List of single classes accumulated as inheritance clauses are processed.
        /// </summary>
        ISealableList<IClass> ClassGroupList { get; }

        /// <summary>
        /// Combined list of all classes in the same group.
        /// </summary>
        OnceReference<SingleClassGroup> ClassGroup { get; }

        /// <summary>
        /// Adds one or more classes to a group.
        /// </summary>
        /// <param name="inheritedSingleClassList">The list of classes added.</param>
        void UpdateClassGroup(IList<IClass> inheritedSingleClassList);

        /// <summary>
        /// Table of inherited types.
        /// </summary>
        ISealableDictionary<ITypeName, ICompiledType> InheritanceTable { get; }

        /// <summary>
        /// The resolved table of imported classes.
        /// </summary>
        ISealableDictionary<ITypeName, IClassType> ResolvedImportedClassTable { get; }

        /// <summary>
        /// List of resolved bodies.
        /// </summary>
        IList<IBody> BodyList { get; }

        /// <summary>
        /// List of resolved command overloads.
        /// </summary>
        IList<ICommandOverload> CommandOverloadList { get; }

        /// <summary>
        /// List of resolved query overloads.
        /// </summary>
        IList<IQueryOverload> QueryOverloadList { get; }

        /// <summary>
        /// List of expressions that are default values of features of this class.
        /// </summary>
        IList<IExpression> NodeWithDefaultList { get; }

        /// <summary>
        /// List of expressions that are constant numbers in this class.
        /// </summary>
        IList<IExpression> NodeWithNumberConstantList { get; }

        /// <summary>
        /// Table of imported language types.
        /// </summary>
        Dictionary<Guid, Tuple<ITypeName, IClassType>> ImportedLanguageTypeTable { get; }

        /// <summary>
        /// All resolved names.
        /// </summary>
        ISealableDictionary<string, ISealableDictionary> NamespaceTable { get; }

        /// <summary>
        /// All resolved bodies.
        /// </summary>
        OnceReference<IList<IBody>> ResolvedBodyTagList { get; }

        /// <summary>
        /// List of resolved expressions that are default values of features of this class.
        /// </summary>
        OnceReference<IList<IExpression>> ResolvedNodeWithDefaultList { get; }

        /// <summary>
        /// List of resolved expressions that are constant numbers in this class.
        /// </summary>
        OnceReference<IList<IExpression>> ResolvedNodeWithNumberConstantList { get; }

        /// <summary>
        /// Table of inherited bodies.
        /// </summary>
        ISealableDictionary<IClassType, IList<IBody>> InheritedBodyTagListTable { get; }

        /// <summary>
        /// List of bodies with resolved instructions
        /// </summary>
        OnceReference<IList<IBody>> ResolvedBodyList { get; }

        /// <summary>
        /// Table of computer-assigned discrete values.
        /// </summary>
        ISealableDictionary<IDiscrete, string> AssignedDiscreteTable { get; }

        /// <summary>
        /// True if the class contains at least one extern body.
        /// </summary>
        bool HasExternBody { get; }

        /// <summary>
        /// List of initialized objects of this class.
        /// </summary>
        IList<IInitializedObjectExpression> InitializedObjectList { get; }

        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        void RestartNumberType(ref bool isChanged);

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        void CheckNumberType(ref bool isChanged);

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        void ValidateNumberType(IErrorList errorList);
    }

    /// <summary>
    /// Compiler IClass.
    /// </summary>
    public partial class Class : BaseNode.Class, IClass
    {
        #region Init
        static Class()
        {
            ClassAny = ClassForTypeAny(LanguageClasses.Any, BaseNode.CopySemantic.Any);
            ClassAnyReference = ClassForTypeAny(LanguageClasses.AnyReference, BaseNode.CopySemantic.Reference);
            ClassAnyValue = ClassForTypeAny(LanguageClasses.AnyValue, BaseNode.CopySemantic.Value);
        }

        private static IClass ClassForTypeAny(NameGuidPair languageClass, BaseNode.CopySemantic copySpecification)
        {
            Class BaseClass = new Class();
            BaseClass.CopySpecification = copySpecification;
            Name BaseClassName = new Name();
            BaseClassName.Text = languageClass.Name;
            BaseClassName.ValidText.Item = languageClass.Name;
            BaseClass.EntityName = BaseClassName;
            BaseClass.ClassGuid = languageClass.Guid;
            BaseClass.ValidClassName = languageClass.Name;
            BaseClass.ClassGroupList.Seal();
            BaseClass.ClassGroup.Item = new SingleClassGroup(BaseClass);
            BaseClass.DiscreteTable.Seal();
            BaseClass.DiscreteWithValueTable.Seal();
            BaseClass.FeatureTable.Seal();
            BaseClass.ClassPath = $"Compiler.{CompilerPathGuid}.Language.{languageClass.Name}";
            BaseClass.SetFullClassPath();
            BaseClass.GenericTable.Seal();
            BaseClass.InheritanceTable.Seal();
            BaseClass.InheritedClassTypeTable.Seal();
            BaseClass.ExportTable.Seal();
            BaseClass.ConversionFromTable.Seal();
            BaseClass.ConversionToTable.Seal();
            BaseClass.TypedefTable.Seal();
            BaseClass.ValidSourceName = string.Empty;
            BaseClass.ResolvedBodyTagList.Item = new List<IBody>();
            BaseClass.AssignedDiscreteTable.Seal();

            return BaseClass;
        }

        /// <summary>
        /// Guid to use int the path of compiler classes.
        /// </summary>
        public static readonly Guid CompilerPathGuid = new Guid("E45340ED-7C93-44E5-BF34-10368129BF68");

        /// <summary>
        /// Compiler class 'Any'.
        /// </summary>
        public static IClass ClassAny { get; }

        /// <summary>
        /// Compiler class 'Any Reference'.
        /// </summary>
        public static IClass ClassAnyReference { get; }

        /// <summary>
        /// Compiler class 'Any Value'.
        /// </summary>
        public static IClass ClassAnyValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Class"/> class.
        /// </summary>
        public Class()
        {
            LocalScope.Seal();
        }
        #endregion

        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.InvariantBlocks"/>.
        /// </summary>
        public IList<IAssertion> InvariantList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ConversionBlocks"/>.
        /// </summary>
        public IList<IIdentifier> ConversionList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.FeatureBlocks"/>.
        /// </summary>
        public IList<IFeature> FeatureList { get; } = new List<IFeature>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ClassReplicateBlocks"/>.
        /// </summary>
        public IList<IClassReplicate> ClassReplicateList { get; } = new List<IClassReplicate>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.DiscreteBlocks"/>.
        /// </summary>
        public IList<IDiscrete> DiscreteList { get; } = new List<IDiscrete>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.InheritanceBlocks"/>.
        /// </summary>
        public IList<IInheritance> InheritanceList { get; } = new List<IInheritance>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.TypedefBlocks"/>.
        /// </summary>
        public IList<ITypedef> TypedefList { get; } = new List<ITypedef>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ExportBlocks"/>.
        /// </summary>
        public IList<IExport> ExportList { get; } = new List<IExport>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.ImportBlocks"/>.
        /// </summary>
        public IList<IImport> ImportList { get; } = new List<IImport>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Class.GenericBlocks"/>.
        /// </summary>
        public IList<IGeneric> GenericList { get; } = new List<IGeneric>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyName">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyName, List<BaseNode.Node> nodeList)
        {
            IList TargetList = null;

            switch (propertyName)
            {
                case nameof(InvariantBlocks):
                    TargetList = (IList)InvariantList;
                    break;

                case nameof(ConversionBlocks):
                    TargetList = (IList)ConversionList;
                    break;

                case nameof(FeatureBlocks):
                    TargetList = (IList)FeatureList;
                    break;

                case nameof(ClassReplicateBlocks):
                    TargetList = (IList)ClassReplicateList;
                    break;

                case nameof(DiscreteBlocks):
                    TargetList = (IList)DiscreteList;
                    break;

                case nameof(InheritanceBlocks):
                    TargetList = (IList)InheritanceList;
                    break;

                case nameof(TypedefBlocks):
                    TargetList = (IList)TypedefList;
                    break;

                case nameof(ExportBlocks):
                    TargetList = (IList)ExportList;
                    break;

                case nameof(ImportBlocks):
                    TargetList = (IList)ImportList;
                    break;

                case nameof(GenericBlocks):
                    TargetList = (IList)GenericList;
                    break;
            }

            Debug.Assert(TargetList != null);
            Debug.Assert(TargetList.Count == 0);

            foreach (BaseNode.Node Node in nodeList)
                TargetList.Add(Node);
        }
        #endregion

        #region Implementation of ISource
        /// <summary>
        /// The parent node, null if root.
        /// </summary>
        public ISource ParentSource { get; private set; }

        /// <summary>
        /// The parent class, null if none.
        /// </summary>
        public IClass EmbeddingClass { get; private set; }

        /// <summary>
        /// The parent feature, null if none.
        /// </summary>
        public IFeature EmbeddingFeature { get; private set; }

        /// <summary>
        /// The parent overload, null if none.
        /// </summary>
        public IOverload EmbeddingOverload { get; private set; }

        /// <summary>
        /// The parent body, null if none.
        /// </summary>
        public IBody EmbeddingBody { get; private set; }

        /// <summary>
        /// The parent assertion, null if none.
        /// </summary>
        public IAssertion EmbeddingAssertion { get; private set; }

        /// <summary>
        /// Initializes parents based on the provided <paramref name="parentSource"/> node.
        /// </summary>
        /// <param name="parentSource">The parent node.</param>
        public virtual void InitializeSource(ISource parentSource)
        {
            ParentSource = parentSource;

            EmbeddingClass = parentSource is IClass AsClass ? AsClass : parentSource?.EmbeddingClass;
            EmbeddingFeature = parentSource is IFeature AsFeature ? AsFeature : parentSource?.EmbeddingFeature;
            EmbeddingOverload = parentSource is IOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
            EmbeddingBody = parentSource is IBody AsBody ? AsBody : parentSource?.EmbeddingBody;
            EmbeddingAssertion = parentSource is IAssertion AsAssertion ? AsAssertion : parentSource?.EmbeddingAssertion;
        }

        /// <summary>
        /// Reset some intermediate results.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to reset.</param>
        public void Reset(IRuleTemplateList ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                LocalGenericTable = new SealableDictionary<string, ICompiledType>();
                GenericTable = new SealableDictionary<string, ICompiledType>();
                LocalDiscreteTable = new SealableDictionary<IFeatureName, IDiscrete>();
                DiscreteTable = new SealableDictionary<IFeatureName, IDiscrete>();
                DiscreteWithValueTable = new SealableDictionary<IFeatureName, IExpression>();
                LocalTypedefTable = new SealableDictionary<IFeatureName, ITypedefType>();
                TypedefTable = new SealableDictionary<IFeatureName, ITypedefType>();
                LocalFeatureTable = new SealableDictionary<IFeatureName, IFeatureInstance>();
                FeatureTable = new SealableDictionary<IFeatureName, IFeatureInstance>();
                InheritedClassTypeTable = new SealableDictionary<IClassType, IObjectType>();
                LocalNamespaceTable = new SealableDictionary<string, ISealableDictionary>();
                LocalExportTable = new SealableDictionary<IFeatureName, ISealableDictionary<string, IClass>>();
                ExportTable = new SealableDictionary<IFeatureName, ISealableDictionary<string, IClass>>();
                ConversionFromTable = new SealableDictionary<IFeatureName, ICreationFeature>();
                ConversionToTable = new SealableDictionary<IFeatureName, IFunctionFeature>();
                ResolvedClassTypeName = new OnceReference<ITypeName>();
                ResolvedClassType = new OnceReference<IClassType>();
                GenericInstanceList = new List<IClassType>();
                TypeTable = new SealableDictionary<ITypeName, ICompiledType>();
                ClassIndexer = new OptionalReference<IIndexerFeature>();
                ClassGroupList = new SealableList<IClass>();
                ClassGroup = new OnceReference<SingleClassGroup>();
                InheritanceTable = new SealableDictionary<ITypeName, ICompiledType>();
                ResolvedImportedClassTable = new SealableDictionary<ITypeName, IClassType>();
                ResolvedAsCompiledType = new OnceReference<ICompiledType>();
                BodyList = new List<IBody>();
                CommandOverloadList = new List<ICommandOverload>();
                QueryOverloadList = new List<IQueryOverload>();
                NodeWithDefaultList = new List<IExpression>();
                NodeWithNumberConstantList = new List<IExpression>();
                ImportedLanguageTypeTable = new Dictionary<Guid, Tuple<ITypeName, IClassType>>();
                NamespaceTable = new SealableDictionary<string, ISealableDictionary>();
                LocalScope = new SealableDictionary<string, IScopeAttributeFeature>();
                LocalScope.Seal();
                InnerScopes = new List<IScopeHolder>();
                FullScope = new SealableDictionary<string, IScopeAttributeFeature>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                AdditionalScope = new SealableDictionary<string, IScopeAttributeFeature>();
                AdditionalScope.Seal();
                ResolvedBodyTagList = new OnceReference<IList<IBody>>();
                ResolvedNodeWithDefaultList = new OnceReference<IList<IExpression>>();
                ResolvedNodeWithNumberConstantList = new OnceReference<IList<IExpression>>();
                InheritedBodyTagListTable = new SealableDictionary<IClassType, IList<IBody>>();
                InitializedObjectList = new List<IInitializedObjectExpression>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                ResolvedBodyList = new OnceReference<IList<IBody>>();
                AssignedDiscreteTable = new SealableDictionary<IDiscrete, string>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IRuleTemplateList ruleTemplateList)
        {
            bool IsResolved = false;

            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = NamespaceTable.IsSealed;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ResolvedBodyTagList.IsAssigned && ResolvedNodeWithDefaultList.IsAssigned && ResolvedNodeWithNumberConstantList.IsAssigned && InheritedBodyTagListTable.IsSealed;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = ResolvedBodyList.IsAssigned && AssignedDiscreteTable.IsSealed;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IScopeHolder
        /// <summary>
        /// Entities local to a scope.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> LocalScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// Additional entities such as loop indexer.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> AdditionalScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// List of scopes containing the current instance.
        /// </summary>
        public IList<IScopeHolder> InnerScopes { get; private set; } = new List<IScopeHolder>();

        /// <summary>
        /// All reachable entities.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> FullScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return $"Class '{EntityName.Text}'";
        }
        #endregion
    }
}
