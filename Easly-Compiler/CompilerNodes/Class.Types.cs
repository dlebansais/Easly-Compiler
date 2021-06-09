namespace CompilerNode
{
    using Easly;
    using EaslyCompiler;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    /// <summary>
    /// Compiler IClass.
    /// </summary>
    public partial class Class : BaseNode.Class, IClass
    {
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
        /// Table of computer-assigned discrete values.
        /// </summary>
        public ISealableDictionary<IDiscrete, string> AssignedDiscreteTable { get; private set; } = new SealableDictionary<IDiscrete, string>();

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

        /// <summary>
        /// List of initialized objects of this class.
        /// </summary>
        public IList<IInitializedObjectExpression> InitializedObjectList { get; private set; } = new List<IInitializedObjectExpression>();
    }
}
