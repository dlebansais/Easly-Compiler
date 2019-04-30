﻿namespace CompilerNode
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
        ListTableEx<IParameter> ParameterTable { get; }
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
        public void Reset(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                ResolvedBaseTypeName = new OnceReference<ITypeName>();
                ResolvedBaseType = new OnceReference<IClassType>();
                ResolvedEntityTypeName = new OnceReference<ITypeName>();
                ResolvedEntityType = new OnceReference<ICompiledType>();
                ParameterTable = new ListTableEx<IParameter>();
                ResolvedTypeName = new OnceReference<ITypeName>();
                ResolvedType = new OnceReference<ICompiledType>();
                DiscreteTable = new HashtableEx<IFeatureName, IDiscrete>();
                FeatureTable = new HashtableEx<IFeatureName, IFeatureInstance>();
                ExportTable = new HashtableEx<IFeatureName, IHashtableEx<string, IClass>>();
                ConformanceTable = new HashtableEx<ITypeName, ICompiledType>();
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
        public virtual bool IsResolved(IList<IRuleTemplate> ruleTemplateList)
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
        public ListTableEx<IParameter> ParameterTable { get; private set; } = new ListTableEx<IParameter>();
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
        public IHashtableEx<IFeatureName, IDiscrete> DiscreteTable { get; private set; } = new HashtableEx<IFeatureName, IDiscrete>();

        /// <summary>
        /// Features available in this type.
        /// </summary>
        public IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable { get; private set; } = new HashtableEx<IFeatureName, IFeatureInstance>();

        /// <summary>
        /// Exports available in this type.
        /// </summary>
        public IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> ExportTable { get; private set; } = new HashtableEx<IFeatureName, IHashtableEx<string, IClass>>();

        /// <summary>
        /// Table of conforming types.
        /// </summary>
        public IHashtableEx<ITypeName, ICompiledType> ConformanceTable { get; private set; } = new HashtableEx<ITypeName, ICompiledType>();

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
        /// <param name="errorList">The list of errors found.</param>
        public bool InstanciateType(IClassType instancingClassType, ref ITypeName resolvedTypeName, ref ICompiledType resolvedType, IList<IError> errorList)
        {
            bool Success = true;
            bool IsNewInstance = false;

            ITypeName InstancedBaseTypeName = ResolvedBaseTypeName.Item;
            ICompiledType InstancedBaseType = ResolvedBaseType.Item;
            Success &= InstancedBaseType.InstanciateType(instancingClassType, ref InstancedBaseTypeName, ref InstancedBaseType, errorList);
            if (InstancedBaseType != ResolvedBaseType.Item)
                IsNewInstance = true;

            ITypeName InstancedEntityTypeName = ResolvedEntityTypeName.Item;
            ICompiledType InstancedEntityType = ResolvedEntityType.Item;
            Success &= InstancedEntityType.InstanciateType(instancingClassType, ref InstancedEntityTypeName, ref InstancedEntityType, errorList);
            if (InstancedEntityType != ResolvedEntityType.Item)
                IsNewInstance = true;

            IList<IEntityDeclaration> InstancedIndexParameterList = new List<IEntityDeclaration>();
            foreach (IEntityDeclaration Parameter in IndexParameterList)
            {
                ITypeName InstancedParameterTypeName = Parameter.ValidEntity.Item.ResolvedFeatureTypeName.Item;
                ICompiledType InstancedParameterType = Parameter.ValidEntity.Item.ResolvedFeatureType.Item;
                Success &= InstancedParameterType.InstanciateType(instancingClassType, ref InstancedParameterTypeName, ref InstancedParameterType, errorList);

                IEntityDeclaration InstancedParameter = new EntityDeclaration();
                InstancedParameter.ResolvedEntityTypeName.Item = InstancedParameterTypeName;
                InstancedParameter.ResolvedEntityType.Item = InstancedParameterType;

                IName ParameterName = (Name)Parameter.EntityName;

                IScopeAttributeFeature NewEntity;
                if (Parameter.DefaultValue.IsAssigned)
                    Success &= ScopeAttributeFeature.Create(Parameter, ParameterName.ValidText.Item, InstancedParameterTypeName, InstancedParameterType, (IExpression)Parameter.DefaultValue.Item, errorList, out NewEntity);
                else
                    Success &= ScopeAttributeFeature.Create(Parameter, ParameterName.ValidText.Item, InstancedParameterTypeName, InstancedParameterType, errorList, out NewEntity);
                InstancedParameter.ValidEntity.Item = NewEntity;

                InstancedIndexParameterList.Add(InstancedParameter);

                if (InstancedParameterType != Parameter.ValidEntity.Item.ResolvedFeatureType.Item)
                    IsNewInstance = true;
            }

            if (IsNewInstance)
                ResolveType(instancingClassType.BaseClass.TypeTable, InstancedBaseTypeName, InstancedBaseType, InstancedEntityTypeName, InstancedEntityType, IndexerKind, InstancedIndexParameterList, ParameterEnd, GetRequireList, GetEnsureList, GetExceptionIdentifierList, SetRequireList, SetEnsureList, SetExceptionIdentifierList, out resolvedTypeName, out resolvedType);

            return Success;
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
        public static void ResolveType(IHashtableEx<ITypeName, ICompiledType> typeTable, ITypeName baseTypeName, ICompiledType baseType, ITypeName entityTypeName, ICompiledType entityType, BaseNode.UtilityType indexerKind, IList<IEntityDeclaration> indexParameterList, BaseNode.ParameterEndStatus parameterEnd, IList<IAssertion> getRequireList, IList<IAssertion> getEnsureList, IList<IIdentifier> getExceptionIdentifierList, IList<IAssertion> setRequireList, IList<IAssertion> setEnsureList, IList<IIdentifier> setExceptionIdentifierList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
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
        public static bool TypeTableContaining(IHashtableEx<ITypeName, ICompiledType> typeTable, ICompiledType baseType, ICompiledType entityType, BaseNode.UtilityType indexerKind, IList<IEntityDeclaration> indexParameterList, BaseNode.ParameterEndStatus parameterEnd, IList<IAssertion> getRequireList, IList<IAssertion> getEnsureList, IList<IIdentifier> getExceptionIdentifierList, IList<IAssertion> setRequireList, IList<IAssertion> setEnsureList, IList<IIdentifier> setExceptionIdentifierList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            resolvedTypeName = null;
            resolvedType = null;
            bool Result = false;

            foreach (KeyValuePair<ITypeName, ICompiledType> Entry in typeTable)
                if (Entry.Value is IIndexerType AsIndexerType)
                {
                    if (!IsSameTypes(AsIndexerType, baseType, entityType, indexerKind))
                        continue;

                    if (!IsSameParameters(AsIndexerType, indexParameterList, parameterEnd))
                        continue;

                    if (!IsSameContract(AsIndexerType, getRequireList, getEnsureList, getExceptionIdentifierList, setRequireList, setEnsureList, setExceptionIdentifierList))
                        continue;

                    resolvedTypeName = Entry.Key;
                    resolvedType = AsIndexerType;
                    Result = true;
                    break;
                }

            return Result;
        }

        private static bool IsSameTypes(IIndexerType indexerType, ICompiledType baseType, ICompiledType entityType, BaseNode.UtilityType indexerKind)
        {
            if (indexerType.ResolvedBaseType.Item != baseType)
                return false;

            if (indexerType.ResolvedEntityType.Item != entityType)
                return false;

            if (indexerType.IndexerKind != indexerKind)
                return false;

            return true;
        }

        private static bool IsSameParameters(IIndexerType indexerType, IList<IEntityDeclaration> indexParameterList, BaseNode.ParameterEndStatus parameterEnd)
        {
            if (indexerType.IndexParameterList.Count != indexParameterList.Count)
                return false;

            bool AllParametersEqual = true;
            for (int i = 0; i < indexParameterList.Count; i++)
                if (indexParameterList[i].ValidEntity.Item.ResolvedFeatureType.Item != indexerType.IndexParameterList[i].ValidEntity.Item.ResolvedFeatureType.Item)
                {
                    AllParametersEqual = false;
                    break;
                }

            if (!AllParametersEqual)
                return false;

            if (indexerType.ParameterEnd != parameterEnd)
                return false;

            return true;
        }

        private static bool IsSameContract(IIndexerType indexerType, IList<IAssertion> getRequireList, IList<IAssertion> getEnsureList, IList<IIdentifier> getExceptionIdentifierList, IList<IAssertion> setRequireList, IList<IAssertion> setEnsureList, IList<IIdentifier> setExceptionIdentifierList)
        {
            if (!Assertion.IsAssertionListEqual(indexerType.GetRequireList, getRequireList))
                return false;

            if (!Assertion.IsAssertionListEqual(indexerType.GetEnsureList, getEnsureList))
                return false;

            if (!ExceptionHandler.IdenticalExceptionSignature(indexerType.GetExceptionIdentifierList, getExceptionIdentifierList))
                return false;

            if (!Assertion.IsAssertionListEqual(indexerType.SetRequireList, setRequireList))
                return false;

            if (!Assertion.IsAssertionListEqual(indexerType.SetEnsureList, setEnsureList))
                return false;

            if (!ExceptionHandler.IdenticalExceptionSignature(indexerType.SetExceptionIdentifierList, setExceptionIdentifierList))
                return false;

            return true;
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
        public static void BuildType(ITypeName baseTypeName, ICompiledType baseType, ITypeName entityTypeName, ICompiledType entityType, BaseNode.UtilityType indexerKind, IList<IEntityDeclaration> indexParameterList, BaseNode.ParameterEndStatus parameterEnd, IList<IAssertion> getRequireList, IList<IAssertion> getEnsureList, IList<IIdentifier> getExceptionIdentifierList, IList<IAssertion> setRequireList, IList<IAssertion> setEnsureList, IList<IIdentifier> setExceptionIdentifierList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            IIndexerType ResolvedIndexerType = new IndexerType(baseTypeName, (IClassType)baseType, entityTypeName, entityType, indexerKind, indexParameterList, parameterEnd, getRequireList, getEnsureList, getExceptionIdentifierList, setRequireList, setEnsureList, setExceptionIdentifierList);

#if DEBUG
            // TODO: remove this code, for code coverage purpose only.
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
                if (EntityType != null)
                    return $"indexer {{{((IObjectType)BaseType).TypeToString}}}";
                else
                    return $"indexer {{{ResolvedBaseType.Item.BaseClass.EntityName.Text}}}";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Function Type '{TypeToString}'";
        }
        #endregion
    }
}
