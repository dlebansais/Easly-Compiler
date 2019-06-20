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
    public interface IClass : BaseNode.IClass, INode, INodeWithReplicatedBlocks, ISource, IScopeHolder
    {
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
        /// True if the class contains at least one extern body.
        /// </summary>
        bool HasExternBody { get; }
    }

    /// <summary>
    /// Compiler IClass.
    /// </summary>
    public class Class : BaseNode.Class, IClass
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

        #region Compiler
        /// <summary>
        /// The class path with replication info.
        /// </summary>
        public string FullClassPath { get; private set; }

        /// <summary>
        /// Initializes the class path.
        /// </summary>
        public void SetFullClassPath()
        {
            FullClassPath = ClassPath;
        }

        /// <summary>
        /// Initializes the class path with replication info.
        /// </summary>
        /// <param name="replicationPattern">The replication pattern used.</param>
        /// <param name="source">The source text.</param>
        public void SetFullClassPath(string replicationPattern, string source)
        {
            FullClassPath = $"{ClassPath};{replicationPattern}={source}";
        }

        /// <summary>
        /// The class-specific counter, for the <see cref="BaseNode.PreprocessorMacro.Counter"/> macro.
        /// </summary>
        public int ClassCounter { get; private set; }

        /// <summary>
        /// Increments <see cref="ClassCounter"/>.
        /// </summary>
        public virtual void IncrementClassCounter()
        {
            ClassCounter++;
        }

        /// <summary>
        /// True if the class is an enumeration (inherits from one of the enumeration language classes).
        /// </summary>
        public bool IsEnumeration
        {
            get
            {
                bool IsInheritingEnumeration = false;

                if (GenericTable.Count == 0 && ExportTable.Count == 0 && TypedefTable.Count == 0 && DiscreteTable.Count > 0)
                {
                    ISealableDictionary<ITypeName, ICompiledType> ConformanceTable = ResolvedClassType.Item.ConformanceTable;
                    foreach (KeyValuePair<ITypeName, ICompiledType> Entry in ConformanceTable)
                        if (Entry.Value is IClassType AsClassType)
                            if (AsClassType.BaseClass.ClassGuid == LanguageClasses.Enumeration.Guid)
                                IsInheritingEnumeration |= ConformanceTable.Count == 1;
                }

                return IsInheritingEnumeration;
            }
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
        public void FillReplicatedList(string propertyName, List<BaseNode.INode> nodeList)
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

            foreach (BaseNode.INode Node in nodeList)
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
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                ResolvedBodyList = new OnceReference<IList<IBody>>();
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
                IsResolved = ResolvedBodyList.IsAssigned;
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

        #region Classes and Libraries name collision check
        /// <summary>
        /// The class name, verified as valid.
        /// </summary>
        public string ValidClassName { get; private set; }

        /// <summary>
        /// The class source name, verified as valid.
        /// </summary>
        public string ValidSourceName { get; private set; }

        /// <summary>
        /// The list of imported libraries.
        /// </summary>
        public IList<ILibrary> ImportedLibraryList { get; } = new List<ILibrary>();

        /// <summary>
        /// The table of imported classes.
        /// </summary>
        public ISealableDictionary<string, IImportedClass> ImportedClassTable { get; } = new SealableDictionary<string, IImportedClass>();

        /// <summary>
        /// Validates the class name and class source name, and update <see cref="ValidClassName"/> and <see cref="ValidSourceName"/>.
        /// </summary>
        /// <param name="classTable">Table of valid class names and their sources, updated upon return.</param>
        /// <param name="validatedClassList">List of classes with valid names, updated upon return.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if class names are valid.</returns>
        public virtual bool CheckClassNames(ISealableDictionary<string, ISealableDictionary<string, IClass>> classTable, IList<IClass> validatedClassList, IErrorList errorList)
        {
            IName ClassEntityName = (IName)EntityName;

            // Verify the class name is a valid string.
            if (!StringValidation.IsValidIdentifier(ClassEntityName, EntityName.Text, out string ValidEntityName, out IErrorStringValidity StringError))
            {
                errorList.AddError(StringError);
                return false;
            }

            ValidClassName = ValidEntityName;

            if (FromIdentifier.IsAssigned)
            {
                // Verify the class source name is a valid string.
                IIdentifier ClassFromIdentifier = (IIdentifier)FromIdentifier.Item;

                if (!StringValidation.IsValidIdentifier(ClassFromIdentifier, FromIdentifier.Item.Text, out string ValidFromIdentifier, out StringError))
                {
                    errorList.AddError(StringError);
                    return false;
                }

                ValidSourceName = ValidFromIdentifier;
            }
            else
                ValidSourceName = string.Empty;

            // Add this class with valid names to the list.
            validatedClassList.Add(this);

            if (classTable.ContainsKey(ValidClassName))
            {
                ISealableDictionary<string, IClass> SourceNameTable = classTable[ValidClassName];

                if (SourceNameTable.ContainsKey(ValidSourceName))
                {
                    // Report a source name collision if the class has one.
                    if (FromIdentifier.IsAssigned)
                    {
                        errorList.AddError(new ErrorDuplicateName(ClassEntityName, ValidClassName));
                        return false;
                    }
                }
                else
                    SourceNameTable.Add(ValidSourceName, this);
            }
            else
            {
                ISealableDictionary<string, IClass> SourceNameTable = new SealableDictionary<string, IClass>
                {
                    { ValidSourceName, this }
                };

                classTable.Add(ValidClassName, SourceNameTable);
            }

            return true;
        }

        /// <summary>
        /// Validates a class import clauses.
        /// </summary>
        /// <param name="libraryTable">Imported libraries.</param>
        /// <param name="classTable">Imported classes.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if imports are valid.</returns>
        public virtual bool CheckClassConsistency(ISealableDictionary<string, ISealableDictionary<string, ILibrary>> libraryTable, ISealableDictionary<string, ISealableDictionary<string, IClass>> classTable, IErrorList errorList)
        {
            bool Success = true;

            // Process all import clauses separately.
            foreach (IImport ImportItem in ImportList)
            {
                if (!ImportItem.CheckImportConsistency(libraryTable, out ILibrary MatchingLibrary, errorList))
                {
                    Success = false;
                    continue;
                }

                if (ImportedLibraryList.Contains(MatchingLibrary))
                {
                    Success = false;
                    errorList.AddError(new ErrorDuplicateImport((IIdentifier)ImportItem.LibraryIdentifier, MatchingLibrary.ValidLibraryName, MatchingLibrary.ValidSourceName));
                    continue;
                }

                if (!Library.MergeImports(ImportedClassTable, ImportItem, MatchingLibrary, errorList))
                {
                    Success = false;
                    continue;
                }

                ImportedLibraryList.Add(MatchingLibrary);
            }

            // Check import specifications.
            foreach (KeyValuePair<string, IImportedClass> Entry in ImportedClassTable)
            {
                IImportedClass Imported = Entry.Value;
                if (Imported.Item == this)
                {
                    string NewName = Entry.Key;

                    if (NewName != ValidClassName)
                    {
                        Success = false;
                        errorList.AddError(new ErrorNameChanged(Imported.ImportLocation, ValidClassName, NewName));
                    }

                    if (Imported.IsTypeAssigned && Imported.ImportType != BaseNode.ImportType.Latest)
                    {
                        Success = false;
                        errorList.AddError(new ErrorImportTypeConflict(Imported.ImportLocation, ValidClassName));
                    }

                    break;
                }
            }

            // If not referenced by an imported library, a class should be able to reference itself.
            if (!ImportedClassTable.ContainsKey(ValidClassName))
            {
                IImportedClass SelfImport = new ImportedClass(this, BaseNode.ImportType.Latest);
                ImportedClassTable.Add(ValidClassName, SelfImport);

#if COVERAGE
                string ImportString = SelfImport.ToString();
#endif
            }

            foreach (KeyValuePair<string, IImportedClass> Entry in ImportedClassTable)
            {
                IImportedClass Imported = Entry.Value;
                Imported.SetParentSource(this);
            }

            ImportedClassTable.Seal();

            Debug.Assert(Success || !errorList.IsEmpty);
            return Success;
        }

        /// <summary>
        /// Merges a class import with previous imports.
        /// </summary>
        /// <param name="importedClassTable">The already resolved imports.</param>
        /// <param name="mergedClassTable">The new classes to import.</param>
        /// <param name="importLocation">The import location.</param>
        /// <param name="mergedImportType">The import specification for all <paramref name="mergedClassTable"/>.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if the merge is successful.</returns>
        public static bool MergeClassTables(ISealableDictionary<string, IImportedClass> importedClassTable, ISealableDictionary<string, IImportedClass> mergedClassTable, IImport importLocation, BaseNode.ImportType mergedImportType, IErrorList errorList)
        {
            bool Success = true;

            foreach (KeyValuePair<string, IImportedClass> Entry in mergedClassTable)
            {
                string ClassName = Entry.Key;
                IImportedClass MergedClassItem = Entry.Value;

                // The merged class may have an import specification already, but if so it must match the one used here.
                // We can assume we use mergedImportType after this.
                if (MergedClassItem.IsTypeAssigned && MergedClassItem.ImportType != mergedImportType)
                {
                    errorList.AddError(new ErrorImportTypeConflict(importLocation, ClassName));
                    Success = false;
                }

                // If a class is already imported with this name somehow.
                if (importedClassTable.ContainsKey(ClassName))
                {
                    // It must be the same class.
                    IImportedClass ClassItem = importedClassTable[ClassName];
                    if (ClassItem.Item != MergedClassItem.Item)
                    {
                        errorList.AddError(new ErrorNameAlreadyUsed(importLocation, ClassName));
                        Success = false;
                        continue;
                    }

                    // If the already existing imported class has a specification, it must match the one use for merge.
                    if (ClassItem.IsTypeAssigned)
                    {
                        if (ClassItem.ImportType != mergedImportType)
                        {
                            errorList.AddError(new ErrorImportTypeConflict(importLocation, ClassName));
                            Success = false;
                            continue;
                        }
                    }
                    else
                        ClassItem.SetImportType(mergedImportType);

                    // If the import location isn't specified yet, use the imported library.
                    if (!ClassItem.IsLocationAssigned)
                        ClassItem.SetImportLocation(importLocation);
                }
                else
                {
                    // New class, at least by name. Make sure it's not an already imported class using a different name.
                    Success &= MergeClassTablesWithNewClass(importedClassTable, ClassName, MergedClassItem, importLocation, mergedImportType, errorList);
                }
            }

            Debug.Assert(Success || !errorList.IsEmpty);
            return Success;
        }

        private static bool MergeClassTablesWithNewClass(ISealableDictionary<string, IImportedClass> importedClassTable, string className, IImportedClass mergedClassItem, IImport importLocation, BaseNode.ImportType mergedImportType, IErrorList errorList)
        {
            bool Success = true;

            foreach (KeyValuePair<string, IImportedClass> ImportedEntry in importedClassTable)
            {
                IImportedClass ClassItem = ImportedEntry.Value;
                if (ClassItem.Item == mergedClassItem.Item)
                {
                    string OldName = ImportedEntry.Key;
                    errorList.AddError(new ErrorClassAlreadyImported(importLocation, OldName, className));
                    Success = false;
                    break;
                }
            }

            // First time this class is imported, use the merge import type specification and location since they are known.
            if (Success)
            {
                mergedClassItem.SetImportType(mergedImportType);
                mergedClassItem.SetImportLocation(importLocation);

                importedClassTable.Add(className, mergedClassItem);
            }

            return Success;
        }
        #endregion

        #region Types
        /// <summary>
        /// The table of resolved generics arguments for this class.
        /// </summary>
        public ISealableDictionary<string, ICompiledType> LocalGenericTable { get; private set; } = new SealableDictionary<string, ICompiledType>();

        /// <summary>
        /// Table of all resolved generics in this class, direct or inherited.
        /// </summary>
        public ISealableDictionary<string, ICompiledType> GenericTable { get; private set; } = new SealableDictionary<string, ICompiledType>();

        /// <summary>
        /// Table of resolved discretes defined in this class.
        /// </summary>
        public ISealableDictionary<IFeatureName, IDiscrete> LocalDiscreteTable { get; private set; } = new SealableDictionary<IFeatureName, IDiscrete>();

        /// <summary>
        /// Table of all resolved discretes in this class, direct or inherited.
        /// </summary>
        public ISealableDictionary<IFeatureName, IDiscrete> DiscreteTable { get; private set; } = new SealableDictionary<IFeatureName, IDiscrete>();

        /// <summary>
        /// Table of all resolved discretes in this class, direct or inherited.
        /// </summary>
        public ISealableDictionary<IFeatureName, IExpression> DiscreteWithValueTable { get; private set; } = new SealableDictionary<IFeatureName, IExpression>();

        /// <summary>
        /// Table of resolved typedefs defined in this class.
        /// </summary>
        public ISealableDictionary<IFeatureName, ITypedefType> LocalTypedefTable { get; private set; } = new SealableDictionary<IFeatureName, ITypedefType>();

        /// <summary>
        /// Table of all resolved typedefs in this class, direct or inherited.
        /// </summary>
        public ISealableDictionary<IFeatureName, ITypedefType> TypedefTable { get; private set; } = new SealableDictionary<IFeatureName, ITypedefType>();

        /// <summary>
        /// Table of resolved features defined in this class.
        /// </summary>
        public ISealableDictionary<IFeatureName, IFeatureInstance> LocalFeatureTable { get; private set; } = new SealableDictionary<IFeatureName, IFeatureInstance>();

        /// <summary>
        /// Table of all resolved features in this class, direct or inherited.
        /// </summary>
        public ISealableDictionary<IFeatureName, IFeatureInstance> FeatureTable { get; private set; } = new SealableDictionary<IFeatureName, IFeatureInstance>();

        /// <summary>
        /// Table of inherited types by their class type.
        /// </summary>
        public ISealableDictionary<IClassType, IObjectType> InheritedClassTypeTable { get; private set; } = new SealableDictionary<IClassType, IObjectType>();

        /// <summary>
        /// Table of resolved namespaces defined in this class.
        /// </summary>
        public ISealableDictionary<string, ISealableDictionary> LocalNamespaceTable { get; private set; } = new SealableDictionary<string, ISealableDictionary>();

        /// <summary>
        /// Table of resolved exports defined in this class.
        /// </summary>
        public ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> LocalExportTable { get; private set; } = new SealableDictionary<IFeatureName, ISealableDictionary<string, IClass>>();

        /// <summary>
        /// Table of all resolved exports in this class, direct or inherited.
        /// </summary>
        public ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> ExportTable { get; private set; } = new SealableDictionary<IFeatureName, ISealableDictionary<string, IClass>>();

        /// <summary>
        /// Table of all resolved conversion procedures in this class, direct or inherited.
        /// </summary>
        public ISealableDictionary<IFeatureName, ICreationFeature> ConversionFromTable { get; private set; } = new SealableDictionary<IFeatureName, ICreationFeature>();

        /// <summary>
        /// Table of all resolved conversion queries in this class, direct or inherited.
        /// </summary>
        public ISealableDictionary<IFeatureName, IFunctionFeature> ConversionToTable { get; private set; } = new SealableDictionary<IFeatureName, IFunctionFeature>();

        /// <summary>
        /// The type name for this class.
        /// </summary>
        public OnceReference<ITypeName> ResolvedClassTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The type from this class.
        /// </summary>
        public OnceReference<IClassType> ResolvedClassType { get; private set; } = new OnceReference<IClassType>();

        /// <summary>
        /// List of types corresponding to each generic argument.
        /// </summary>
        public IList<IClassType> GenericInstanceList { get; private set; } = new List<IClassType>();

        /// <summary>
        /// Table of all types used in this class.
        /// </summary>
        public ISealableDictionary<ITypeName, ICompiledType> TypeTable { get; private set; } = new SealableDictionary<ITypeName, ICompiledType>();

        /// <summary>
        /// The class indexer, if any.
        /// </summary>
        public OptionalReference<IIndexerFeature> ClassIndexer { get; private set; } = new OptionalReference<IIndexerFeature>();

        /// <summary>
        /// List of single classes accumulated as inheritance clauses are processed.
        /// </summary>
        public ISealableList<IClass> ClassGroupList { get; private set; } = new SealableList<IClass>();

        /// <summary>
        /// Combined list of all classes in the same group.
        /// </summary>
        public OnceReference<SingleClassGroup> ClassGroup { get; private set; } = new OnceReference<SingleClassGroup>();

        /// <summary>
        /// Adds one or more classes to a group.
        /// </summary>
        /// <param name="inheritedSingleClassList">The list of classes added.</param>
        public void UpdateClassGroup(IList<IClass> inheritedSingleClassList)
        {
            Debug.Assert(ClassGroup.IsAssigned);

            bool IsUpdated = false;
            foreach (IClass GroupClass in inheritedSingleClassList)
                ClassGroup.Item.AddClass(GroupClass, ref IsUpdated);

            if (IsUpdated)
                foreach (IClass GroupClass in ClassGroup.Item.GroupClassList)
                    if (GroupClass.ClassGroup.IsAssigned)
                        GroupClass.UpdateClassGroup(inheritedSingleClassList);
        }

        /// <summary>
        /// Table of inherited types.
        /// </summary>
        public ISealableDictionary<ITypeName, ICompiledType> InheritanceTable { get; private set; } = new SealableDictionary<ITypeName, ICompiledType>();

        /// <summary>
        /// The resolved table of imported classes.
        /// </summary>
        public ISealableDictionary<ITypeName, IClassType> ResolvedImportedClassTable { get; private set; } = new SealableDictionary<ITypeName, IClassType>();

        /// <summary>
        /// The type for a pre-compiled class.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedAsCompiledType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// List of resolved bodies.
        /// </summary>
        public IList<IBody> BodyList { get; private set; } = new List<IBody>();

        /// <summary>
        /// List of resolved command overloads.
        /// </summary>
        public IList<ICommandOverload> CommandOverloadList { get; private set; } = new List<ICommandOverload>();

        /// <summary>
        /// List of resolved query overloads.
        /// </summary>
        public IList<IQueryOverload> QueryOverloadList { get; private set; } = new List<IQueryOverload>();

        /// <summary>
        /// List of resolved expressions that are default values of features of this class.
        /// </summary>
        public IList<IExpression> NodeWithDefaultList { get; private set; } = new List<IExpression>();

        /// <summary>
        /// List of resolved expressions that are constant numbers in this class.
        /// </summary>
        public IList<IExpression> NodeWithNumberConstantList { get; private set; } = new List<IExpression>();

        /// <summary>
        /// Table of imported language types.
        /// </summary>
        public Dictionary<Guid, Tuple<ITypeName, IClassType>> ImportedLanguageTypeTable { get; private set; } = new Dictionary<Guid, Tuple<ITypeName, IClassType>>();

        /// <summary>
        /// All resolved names.
        /// </summary>
        public ISealableDictionary<string, ISealableDictionary> NamespaceTable { get; private set; } = new SealableDictionary<string, ISealableDictionary>();

        /// <summary>
        /// All resolved bodies.
        /// </summary>
        public OnceReference<IList<IBody>> ResolvedBodyTagList { get; private set; } = new OnceReference<IList<IBody>>();

        /// <summary>
        /// List of resolved expressions that are default values of features of this class.
        /// </summary>
        public OnceReference<IList<IExpression>> ResolvedNodeWithDefaultList { get; private set; } = new OnceReference<IList<IExpression>>();

        /// <summary>
        /// List of resolved expressions that are constant numbers in this class.
        /// </summary>
        public OnceReference<IList<IExpression>> ResolvedNodeWithNumberConstantList { get; private set; } = new OnceReference<IList<IExpression>>();

        /// <summary>
        /// Table of inherited bodies.
        /// </summary>
        public ISealableDictionary<IClassType, IList<IBody>> InheritedBodyTagListTable { get; private set; } = new SealableDictionary<IClassType, IList<IBody>>();

        /// <summary>
        /// List of bodies with resolved instructions
        /// </summary>
        public OnceReference<IList<IBody>> ResolvedBodyList { get; private set; } = new OnceReference<IList<IBody>>();

        /// <summary>
        /// True if the class contains at least one extern body.
        /// </summary>
        public bool HasExternBody
        {
            get
            {
                bool Result = false;

                foreach (KeyValuePair<IFeatureName, IFeatureInstance> Entry in FeatureTable)
                {
                    IFeatureInstance Instance = Entry.Value;
                    ICompiledFeature SourceFeature = Instance.Feature;

                    Result |= SourceFeature.HasExternBody;
                }

                return Result;
            }
        }
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
