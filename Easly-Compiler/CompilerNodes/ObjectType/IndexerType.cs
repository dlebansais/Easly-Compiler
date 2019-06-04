namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IIndexerType.
    /// </summary>
    public interface IIndexerType : BaseNode.IIndexerType, IObjectType, INodeWithReplicatedBlocks, ICompiledType
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.IndexParameterBlocks"/>.
        /// </summary>
        IList<IEntityDeclaration> IndexParameterList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.GetRequireBlocks"/>.
        /// </summary>
        IList<IAssertion> GetRequireList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.GetEnsureBlocks"/>.
        /// </summary>
        IList<IAssertion> GetEnsureList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.GetExceptionIdentifierBlocks"/>.
        /// </summary>
        IList<IIdentifier> GetExceptionIdentifierList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.SetRequireBlocks"/>.
        /// </summary>
        IList<IAssertion> SetRequireList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.SetEnsureBlocks"/>.
        /// </summary>
        IList<IAssertion> SetEnsureList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.SetExceptionIdentifierBlocks"/>.
        /// </summary>
        IList<IIdentifier> SetExceptionIdentifierList { get; }

        /// <summary>
        /// The type name of the resolved base type.
        /// </summary>
        OnceReference<ITypeName> ResolvedBaseTypeName { get; }

        /// <summary>
        /// The type name of the resolved base type.
        /// </summary>
        OnceReference<IClassType> ResolvedBaseType { get; }

        /// <summary>
        /// The type name of the resolved result type.
        /// </summary>
        OnceReference<ITypeName> ResolvedEntityTypeName { get; }

        /// <summary>
        /// The type of the resolved result type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedEntityType { get; }

        /// <summary>
        /// Table of resolved parameters.
        /// </summary>
        SealableList<IParameter> ParameterTable { get; }
    }

    /// <summary>
    /// Compiler IIndexerType.
    /// </summary>
    public class IndexerType : BaseNode.IndexerType, IIndexerType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexerType"/> class.
        /// This constructor is required for deserialization.
        /// </summary>
        public IndexerType()
        {
            FeatureTable.Seal();
            DiscreteTable.Seal();
            ConformanceTable.Seal();
            ExportTable.Seal();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexerType"/> class.
        /// </summary>
        /// <param name="baseTypeName">The type name of the resolved base type.</param>
        /// <param name="baseType">The type of the resolved base type.</param>
        /// <param name="entityTypeName">The type name of the resolved result type.</param>
        /// <param name="entityType">The type of the resolved result type.</param>
        /// <param name="indexerKind">Type of indexer.</param>
        /// <param name="indexParameterList">The list of parameters.</param>
        /// <param name="parameterEnd">The indexer parameter end type.</param>
        /// <param name="getRequireList">The list of require assertions for the getter.</param>
        /// <param name="getEnsureList">The list of ensure assertions for the getter.</param>
        /// <param name="getExceptionIdentifierList">The list of known exceptions thrown for the getter.</param>
        /// <param name="setRequireList">The list of require assertions for the setter.</param>
        /// <param name="setEnsureList">The list of ensure assertions for the setter.</param>
        /// <param name="setExceptionIdentifierList">The list of known exceptions thrown for the setter.</param>
        public IndexerType(ITypeName baseTypeName, IClassType baseType, ITypeName entityTypeName, ICompiledType entityType, BaseNode.UtilityType indexerKind, IList<IEntityDeclaration> indexParameterList, BaseNode.ParameterEndStatus parameterEnd, IList<IAssertion> getRequireList, IList<IAssertion> getEnsureList, IList<IIdentifier> getExceptionIdentifierList, IList<IAssertion> setRequireList, IList<IAssertion> setEnsureList, IList<IIdentifier> setExceptionIdentifierList)
            : this()
        {
            BaseType = baseType;
            EntityType = null;
            ParameterEnd = parameterEnd;
            IndexerKind = indexerKind;

            ResolvedBaseTypeName.Item = baseTypeName;
            ResolvedBaseType.Item = baseType;
            ResolvedEntityTypeName.Item = entityTypeName;
            ResolvedEntityType.Item = entityType;
            IndexParameterList = indexParameterList;
            GetRequireList = getRequireList;
            GetEnsureList = getEnsureList;
            GetExceptionIdentifierList = getExceptionIdentifierList;
            SetRequireList = setRequireList;
            SetEnsureList = setEnsureList;
            SetExceptionIdentifierList = setExceptionIdentifierList;

            foreach (IEntityDeclaration Item in indexParameterList)
            {
                IName ParameterName = (IName)Item.EntityName;
                string ValidText = ParameterName.ValidText.Item;
                IScopeAttributeFeature ParameterFeature = Item.ValidEntity.Item;
                ParameterTable.Add(new Parameter(ValidText, ParameterFeature));
            }
            ParameterTable.Seal();
        }
        #endregion

        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.IndexParameterBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> IndexParameterList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.GetRequireBlocks"/>.
        /// </summary>
        public IList<IAssertion> GetRequireList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.GetEnsureBlocks"/>.
        /// </summary>
        public IList<IAssertion> GetEnsureList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.GetExceptionIdentifierBlocks"/>.
        /// </summary>
        public IList<IIdentifier> GetExceptionIdentifierList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.SetRequireBlocks"/>.
        /// </summary>
        public IList<IAssertion> SetRequireList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.SetEnsureBlocks"/>.
        /// </summary>
        public IList<IAssertion> SetEnsureList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexerType.SetExceptionIdentifierBlocks"/>.
        /// </summary>
        public IList<IIdentifier> SetExceptionIdentifierList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyTypeArgument">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyTypeArgument, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyTypeArgument)
            {
                case nameof(IndexParameterBlocks):
                    TargetList = (IList)IndexParameterList;
                    break;

                case nameof(GetRequireBlocks):
                    TargetList = (IList)GetRequireList;
                    break;

                case nameof(GetEnsureBlocks):
                    TargetList = (IList)GetEnsureList;
                    break;

                case nameof(GetExceptionIdentifierBlocks):
                    TargetList = (IList)GetExceptionIdentifierList;
                    break;

                case nameof(SetRequireBlocks):
                    TargetList = (IList)SetRequireList;
                    break;

                case nameof(SetEnsureBlocks):
                    TargetList = (IList)SetEnsureList;
                    break;

                case nameof(SetExceptionIdentifierBlocks):
                    TargetList = (IList)SetExceptionIdentifierList;
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Contract || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                ResolvedBaseTypeName = new OnceReference<ITypeName>();
                ResolvedBaseType = new OnceReference<IClassType>();
                ResolvedEntityTypeName = new OnceReference<ITypeName>();
                ResolvedEntityType = new OnceReference<ICompiledType>();
                ParameterTable = new SealableList<IParameter>();
                ResolvedTypeName = new OnceReference<ITypeName>();
                ResolvedType = new OnceReference<ICompiledType>();
                DiscreteTable = new SealableDictionary<IFeatureName, IDiscrete>();
                FeatureTable = new SealableDictionary<IFeatureName, IFeatureInstance>();
                ExportTable = new SealableDictionary<IFeatureName, ISealableDictionary<string, IClass>>();
                ConformanceTable = new SealableDictionary<ITypeName, ICompiledType>();
                InstancingRecordList = new List<TypeInstancingRecord>();
                OriginatingTypedef = new OnceReference<ITypedef>();
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Contract || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                Debug.Assert(ResolvedTypeName.IsAssigned == ResolvedType.IsAssigned);
                IsResolved = ParameterTable.IsSealed;
                Debug.Assert(ResolvedType.IsAssigned == IsResolved);
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// The type name of the resolved base type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedBaseTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The type of the resolved base type.
        /// </summary>
        public OnceReference<IClassType> ResolvedBaseType { get; private set; } = new OnceReference<IClassType>();

        /// <summary>
        /// The type name of the resolved result type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedEntityTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The type of the resolved result type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedEntityType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// Table of resolved parameters.
        /// </summary>
        public SealableList<IParameter> ParameterTable { get; private set; } = new SealableList<IParameter>();
        #endregion

        #region Implementation of IObjectType
        /// <summary>
        /// The resolved type name.
        /// </summary>
        public OnceReference<ITypeName> ResolvedTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion

        #region Implementation of ICompiledType
        /// <summary>
        /// Discretes available in this type.
        /// </summary>
        public ISealableDictionary<IFeatureName, IDiscrete> DiscreteTable { get; private set; } = new SealableDictionary<IFeatureName, IDiscrete>();

        /// <summary>
        /// Features available in this type.
        /// </summary>
        public ISealableDictionary<IFeatureName, IFeatureInstance> FeatureTable { get; private set; } = new SealableDictionary<IFeatureName, IFeatureInstance>();

        /// <summary>
        /// Exports available in this type.
        /// </summary>
        public ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> ExportTable { get; private set; } = new SealableDictionary<IFeatureName, ISealableDictionary<string, IClass>>();

        /// <summary>
        /// Table of conforming types.
        /// </summary>
        public ISealableDictionary<ITypeName, ICompiledType> ConformanceTable { get; private set; } = new SealableDictionary<ITypeName, ICompiledType>();

        /// <summary>
        /// List of type instancing.
        /// </summary>
        public IList<TypeInstancingRecord> InstancingRecordList { get; private set; } = new List<TypeInstancingRecord>();

        /// <summary>
        /// Type friendly name, unique.
        /// </summary>
        public string TypeFriendlyName
        {
            get
            {
                string BaseTypeName = ResolvedBaseType.Item.TypeFriendlyName;
                string EntityTypeName = ResolvedEntityType.Item.TypeFriendlyName;

                string ParameterDeclarationList = string.Empty;

                foreach (IEntityDeclaration ParameterItem in IndexParameterList)
                {
                    if (ParameterDeclarationList.Length > 0)
                        ParameterDeclarationList += ", ";

                    IScopeAttributeFeature ParameterAttribute = ParameterItem.ValidEntity.Item;
                    ParameterDeclarationList += ParameterAttribute.ValidFeatureName.Item.Name + " :" + ParameterAttribute.ResolvedFeatureTypeName.Item;
                }

                string Result = $"{BaseTypeName}.indexer: {EntityTypeName}({ParameterDeclarationList})";

                bool IsHandled = false;
                switch (IndexerKind)
                {
                    case BaseNode.UtilityType.ReadOnly:
                        Result += ", readonly";
                        IsHandled = true;
                        break;
                    case BaseNode.UtilityType.WriteOnly:
                        Result += ", writeonly";
                        IsHandled = true;
                        break;
                    case BaseNode.UtilityType.ReadWrite:
                        Result += ", readwrite";
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);

                return Result;
            }
        }

        /// <summary>
        /// True if the type is a reference type.
        /// </summary>
        public bool IsReference
        {
            get { return false; }
        }

        /// <summary>
        /// True if the type is a value type.
        /// </summary>
        public bool IsValue
        {
            get { return true; }
        }

        /// <summary>
        /// The typedef this type comes from, if assigned.
        /// </summary>
        public OnceReference<ITypedef> OriginatingTypedef { get; private set; } = new OnceReference<ITypedef>();

        /// <summary>
        /// Creates an instance of a class type, or reuse an existing instance.
        /// </summary>
        /// <param name="instancingClassType">The class type to instanciate.</param>
        /// <param name="resolvedTypeName">The proposed type instance name.</param>
        /// <param name="resolvedType">The proposed type instance.</param>
        public void InstanciateType(IClassType instancingClassType, ref ITypeName resolvedTypeName, ref ICompiledType resolvedType)
        {
            bool IsNewInstance = false;

            ITypeName InstancedBaseTypeName = ResolvedBaseTypeName.Item;

            ICompiledType InstancedBaseType = ResolvedBaseType.Item;
            InstancedBaseType.InstanciateType(instancingClassType, ref InstancedBaseTypeName, ref InstancedBaseType);
            IsNewInstance |= InstancedBaseType != ResolvedBaseType.Item;

            ITypeName InstancedEntityTypeName = ResolvedEntityTypeName.Item;
            ICompiledType InstancedEntityType = ResolvedEntityType.Item;
            InstancedEntityType.InstanciateType(instancingClassType, ref InstancedEntityTypeName, ref InstancedEntityType);
            IsNewInstance |= InstancedEntityType != ResolvedEntityType.Item;

            IList<IEntityDeclaration> InstancedIndexParameterList = new List<IEntityDeclaration>();
            foreach (IEntityDeclaration Parameter in IndexParameterList)
            {
                ITypeName InstancedParameterTypeName = Parameter.ValidEntity.Item.ResolvedFeatureTypeName.Item;
                ICompiledType InstancedParameterType = Parameter.ValidEntity.Item.ResolvedFeatureType.Item;
                InstancedParameterType.InstanciateType(instancingClassType, ref InstancedParameterTypeName, ref InstancedParameterType);

                IEntityDeclaration InstancedParameter = new EntityDeclaration(Parameter, InstancedParameterTypeName, InstancedParameterType);
                IName ParameterName = (Name)Parameter.EntityName;

                IScopeAttributeFeature NewEntity;
                if (Parameter.DefaultValue.IsAssigned)
                {
                    // The default value has already been checked and validated.
                    bool IsCreated = ScopeAttributeFeature.Create(Parameter, ParameterName.ValidText.Item, InstancedParameterTypeName, InstancedParameterType, (IExpression)Parameter.DefaultValue.Item, ErrorList.Ignored, out NewEntity);
                    Debug.Assert(IsCreated);
                }
                else
                    NewEntity = ScopeAttributeFeature.Create(Parameter, ParameterName.ValidText.Item, InstancedParameterTypeName, InstancedParameterType);

                IsNewInstance |= InstancedParameterType != Parameter.ValidEntity.Item.ResolvedFeatureType.Item;

                InstancedParameter.ValidEntity.Item = NewEntity;
                InstancedIndexParameterList.Add(InstancedParameter);
            }

            if (IsNewInstance)
            {
                Debug.Assert(InstancedBaseType is IClassType);
                ResolveType(instancingClassType.BaseClass.TypeTable, InstancedBaseTypeName, (IClassType)InstancedBaseType, InstancedEntityTypeName, InstancedEntityType, IndexerKind, InstancedIndexParameterList, ParameterEnd, GetRequireList, GetEnsureList, GetExceptionIdentifierList, SetRequireList, SetEnsureList, SetExceptionIdentifierList, out resolvedTypeName, out resolvedType);
            }
        }
        #endregion

        #region Locate type
        /// <summary>
        /// Locates, or creates, a resolved function type.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="baseTypeName">The type name of the resolved base type.</param>
        /// <param name="baseType">The type of the resolved base type.</param>
        /// <param name="entityTypeName">The type name of the resolved result type.</param>
        /// <param name="entityType">The type of the resolved result type.</param>
        /// <param name="indexerKind">Type of indexer.</param>
        /// <param name="indexParameterList">The list of parameters.</param>
        /// <param name="parameterEnd">The indexer parameter end type.</param>
        /// <param name="getRequireList">The list of require assertions for the getter.</param>
        /// <param name="getEnsureList">The list of ensure assertions for the getter.</param>
        /// <param name="getExceptionIdentifierList">The list of known exceptions thrown for the getter.</param>
        /// <param name="setRequireList">The list of require assertions for the setter.</param>
        /// <param name="setEnsureList">The list of ensure assertions for the setter.</param>
        /// <param name="setExceptionIdentifierList">The list of known exceptions thrown for the setter.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void ResolveType(ISealableDictionary<ITypeName, ICompiledType> typeTable, ITypeName baseTypeName, IClassType baseType, ITypeName entityTypeName, ICompiledType entityType, BaseNode.UtilityType indexerKind, IList<IEntityDeclaration> indexParameterList, BaseNode.ParameterEndStatus parameterEnd, IList<IAssertion> getRequireList, IList<IAssertion> getEnsureList, IList<IIdentifier> getExceptionIdentifierList, IList<IAssertion> setRequireList, IList<IAssertion> setEnsureList, IList<IIdentifier> setExceptionIdentifierList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            if (!TypeTableContaining(typeTable, baseType, entityType, indexerKind, indexParameterList, parameterEnd, getRequireList, getEnsureList, getExceptionIdentifierList, setRequireList, setEnsureList, setExceptionIdentifierList, out resolvedTypeName, out resolvedType))
            {
                BuildType(baseTypeName, baseType, entityTypeName, entityType, indexerKind, indexParameterList, parameterEnd, getRequireList, getEnsureList, getExceptionIdentifierList, setRequireList, setEnsureList, setExceptionIdentifierList, out resolvedTypeName, out resolvedType);
                typeTable.Add(resolvedTypeName, resolvedType);
            }
        }

        /// <summary>
        /// Checks if a matching function type exists in a type table.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="baseType">The type of the resolved base type.</param>
        /// <param name="entityType">The type of the resolved result type.</param>
        /// <param name="indexerKind">Type of indexer.</param>
        /// <param name="indexParameterList">The list of parameters.</param>
        /// <param name="parameterEnd">The indexer parameter end type.</param>
        /// <param name="getRequireList">The list of require assertions for the getter.</param>
        /// <param name="getEnsureList">The list of ensure assertions for the getter.</param>
        /// <param name="getExceptionIdentifierList">The list of known exceptions thrown for the getter.</param>
        /// <param name="setRequireList">The list of require assertions for the setter.</param>
        /// <param name="setEnsureList">The list of ensure assertions for the setter.</param>
        /// <param name="setExceptionIdentifierList">The list of known exceptions thrown for the setter.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static bool TypeTableContaining(ISealableDictionary<ITypeName, ICompiledType> typeTable, ICompiledType baseType, ICompiledType entityType, BaseNode.UtilityType indexerKind, IList<IEntityDeclaration> indexParameterList, BaseNode.ParameterEndStatus parameterEnd, IList<IAssertion> getRequireList, IList<IAssertion> getEnsureList, IList<IIdentifier> getExceptionIdentifierList, IList<IAssertion> setRequireList, IList<IAssertion> setEnsureList, IList<IIdentifier> setExceptionIdentifierList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            resolvedTypeName = null;
            resolvedType = null;
            bool Result = false;

            foreach (KeyValuePair<ITypeName, ICompiledType> Entry in typeTable)
                if (Entry.Value is IIndexerType AsIndexerType)
                {
                    bool IsSameIndexer = true;

                    IsSameIndexer &= IsSameTypes(AsIndexerType, baseType, entityType, indexerKind);
                    IsSameIndexer &= IsSameParameters(AsIndexerType, indexParameterList, parameterEnd);
                    IsSameIndexer &= IsSameContract(AsIndexerType, getRequireList, getEnsureList, getExceptionIdentifierList, setRequireList, setEnsureList, setExceptionIdentifierList);

                    if (IsSameIndexer)
                    {
                        Debug.Assert(!Result);

                        resolvedTypeName = Entry.Key;
                        resolvedType = AsIndexerType;
                        Result = true;
                    }
                }

            return Result;
        }

        private static bool IsSameTypes(IIndexerType indexerType, ICompiledType baseType, ICompiledType entityType, BaseNode.UtilityType indexerKind)
        {
            bool IsSame = true;

            IsSame &= indexerType.ResolvedBaseType.Item == baseType;
            IsSame &= indexerType.ResolvedEntityType.Item == entityType;
            IsSame &= indexerType.IndexerKind == indexerKind;

            return IsSame;
        }

        private static bool IsSameParameters(IIndexerType indexerType, IList<IEntityDeclaration> indexParameterList, BaseNode.ParameterEndStatus parameterEnd)
        {
            bool IsSame = true;

            IsSame &= indexerType.IndexParameterList.Count == indexParameterList.Count;

            for (int i = 0; i < indexerType.IndexParameterList.Count && i < indexParameterList.Count; i++)
                IsSame &= indexParameterList[i].ValidEntity.Item.ResolvedFeatureType.Item == indexerType.IndexParameterList[i].ValidEntity.Item.ResolvedFeatureType.Item;

            IsSame &= indexerType.ParameterEnd == parameterEnd;

            return IsSame;
        }

        private static bool IsSameContract(IIndexerType indexerType, IList<IAssertion> getRequireList, IList<IAssertion> getEnsureList, IList<IIdentifier> getExceptionIdentifierList, IList<IAssertion> setRequireList, IList<IAssertion> setEnsureList, IList<IIdentifier> setExceptionIdentifierList)
        {
            bool IsSame = true;

            IsSame &= Assertion.IsAssertionListEqual(indexerType.GetRequireList, getRequireList);
            IsSame &= Assertion.IsAssertionListEqual(indexerType.GetEnsureList, getEnsureList);
            IsSame &= ExceptionHandler.IdenticalExceptionSignature(indexerType.GetExceptionIdentifierList, getExceptionIdentifierList);
            IsSame &= Assertion.IsAssertionListEqual(indexerType.SetRequireList, setRequireList);
            IsSame &= Assertion.IsAssertionListEqual(indexerType.SetEnsureList, setEnsureList);
            IsSame &= ExceptionHandler.IdenticalExceptionSignature(indexerType.SetExceptionIdentifierList, setExceptionIdentifierList);

            return IsSame;
        }

        /// <summary>
        /// Creates a function type with resolved arguments.
        /// </summary>
        /// <param name="baseTypeName">The type name of the resolved base type.</param>
        /// <param name="baseType">The type of the resolved base type.</param>
        /// <param name="entityTypeName">The type name of the resolved result type.</param>
        /// <param name="entityType">The type of the resolved result type.</param>
        /// <param name="indexerKind">Type of indexer.</param>
        /// <param name="indexParameterList">The list of parameters.</param>
        /// <param name="parameterEnd">The indexer parameter end type.</param>
        /// <param name="getRequireList">The list of require assertions for the getter.</param>
        /// <param name="getEnsureList">The list of ensure assertions for the getter.</param>
        /// <param name="getExceptionIdentifierList">The list of known exceptions thrown for the getter.</param>
        /// <param name="setRequireList">The list of require assertions for the setter.</param>
        /// <param name="setEnsureList">The list of ensure assertions for the setter.</param>
        /// <param name="setExceptionIdentifierList">The list of known exceptions thrown for the setter.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void BuildType(ITypeName baseTypeName, IClassType baseType, ITypeName entityTypeName, ICompiledType entityType, BaseNode.UtilityType indexerKind, IList<IEntityDeclaration> indexParameterList, BaseNode.ParameterEndStatus parameterEnd, IList<IAssertion> getRequireList, IList<IAssertion> getEnsureList, IList<IIdentifier> getExceptionIdentifierList, IList<IAssertion> setRequireList, IList<IAssertion> setEnsureList, IList<IIdentifier> setExceptionIdentifierList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            IIndexerType ResolvedIndexerType = new IndexerType(baseTypeName, baseType, entityTypeName, entityType, indexerKind, indexParameterList, parameterEnd, getRequireList, getEnsureList, getExceptionIdentifierList, setRequireList, setEnsureList, setExceptionIdentifierList);

#if COVERAGE
            string TypeString = ResolvedIndexerType.ToString();
            Debug.Assert(!ResolvedIndexerType.IsReference);
            Debug.Assert(ResolvedIndexerType.IsValue);
#endif

            resolvedTypeName = new TypeName(ResolvedIndexerType.TypeFriendlyName);
            resolvedType = ResolvedIndexerType;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Compares two types.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        public static bool TypesHaveIdenticalSignature(IIndexerType type1, IIndexerType type2)
        {
            bool IsIdentical = true;

            IsIdentical &= ObjectType.TypesHaveIdenticalSignature(type1.ResolvedBaseType.Item, type2.ResolvedBaseType.Item);
            IsIdentical &= ObjectType.TypesHaveIdenticalSignature(type1.ResolvedEntityType.Item, type2.ResolvedEntityType.Item);
            IsIdentical &= type1.IndexerKind == type2.IndexerKind;
            IsIdentical &= type1.IndexParameterList.Count == type2.IndexParameterList.Count;
            IsIdentical &= type1.ParameterEnd == type2.ParameterEnd;

            for (int i = 0; i < type1.IndexParameterList.Count && i < type2.IndexParameterList.Count; i++)
            {
                Debug.Assert(type1.IndexParameterList[i].ValidEntity.IsAssigned);
                Debug.Assert(type1.IndexParameterList[i].ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(type2.IndexParameterList[i].ValidEntity.IsAssigned);
                Debug.Assert(type2.IndexParameterList[i].ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                IsIdentical &= ObjectType.TypesHaveIdenticalSignature(type1.IndexParameterList[i].ValidEntity.Item.ResolvedFeatureType.Item, type2.IndexParameterList[i].ValidEntity.Item.ResolvedFeatureType.Item);
            }

            IsIdentical &= Assertion.IsAssertionListEqual(type1.GetRequireList, type2.GetRequireList);
            IsIdentical &= Assertion.IsAssertionListEqual(type1.GetEnsureList, type2.GetEnsureList);
            IsIdentical &= ExceptionHandler.IdenticalExceptionSignature(type1.GetExceptionIdentifierList, type2.GetExceptionIdentifierList);
            IsIdentical &= Assertion.IsAssertionListEqual(type1.SetRequireList, type2.SetRequireList);
            IsIdentical &= Assertion.IsAssertionListEqual(type1.SetEnsureList, type2.SetEnsureList);
            IsIdentical &= ExceptionHandler.IdenticalExceptionSignature(type1.SetExceptionIdentifierList, type2.SetExceptionIdentifierList);

            return IsIdentical;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string TypeToString
        {
            get
            {
                string Result = null;

                switch (BaseType)
                {
                    case IObjectType AsObjectType:
                        Result = $"indexer {{{AsObjectType.TypeToString}}}";
                        break;

                    case IClassType AsClassType:
                        Result = $"indexer {{{AsClassType.BaseClass.EntityName.Text}}}";
                        break;
                }

                Debug.Assert(Result != null);
                return Result;
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Indexer Type '{TypeToString}'";
        }
        #endregion
    }
}
