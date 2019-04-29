namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IPropertyType.
    /// </summary>
    public interface IPropertyType : BaseNode.IPropertyType, IObjectType, INodeWithReplicatedBlocks, ICompiledType
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.PropertyType.GetEnsureBlocks"/>.
        /// </summary>
        IList<IAssertion> GetEnsureList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.PropertyType.GetExceptionIdentifierBlocks"/>.
        /// </summary>
        IList<IIdentifier> GetExceptionIdentifierList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.PropertyType.SetRequireBlocks"/>.
        /// </summary>
        IList<IAssertion> SetRequireList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.PropertyType.SetExceptionIdentifierBlocks"/>.
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
    }

    /// <summary>
    /// Compiler IPropertyType.
    /// </summary>
    public class PropertyType : BaseNode.PropertyType, IPropertyType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyType"/> class.
        /// This constructor is required for deserialization.
        /// </summary>
        public PropertyType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyType"/> class.
        /// </summary>
        /// <param name="baseTypeName">The type name of the resolved base type.</param>
        /// <param name="baseType">The type of the resolved base type.</param>
        /// <param name="entityTypeName">The type name of the resolved result type.</param>
        /// <param name="entityType">The type of the resolved result type.</param>
        /// <param name="propertyKind">The type of the property.</param>
        /// <param name="getEnsureList">The list of ensure assertions for the getter.</param>
        /// <param name="getExceptionIdentifierList">The list of known exceptions thrown for the getter.</param>
        /// <param name="setRequireList">The list of require assertions for the setter.</param>
        /// <param name="setExceptionIdentifierList">The list of known exceptions thrown for the setter.</param>
        public PropertyType(ITypeName baseTypeName, IClassType baseType, ITypeName entityTypeName, ICompiledType entityType, BaseNode.UtilityType propertyKind, IList<IAssertion> getEnsureList, IList<IIdentifier> getExceptionIdentifierList, IList<IAssertion> setRequireList, IList<IIdentifier> setExceptionIdentifierList)
        {
            ResolvedBaseTypeName.Item = baseTypeName;
            ResolvedBaseType.Item = baseType;
            ResolvedEntityTypeName.Item = entityTypeName;
            ResolvedEntityType.Item = entityType;
            PropertyKind = PropertyKind;
            GetEnsureList = GetEnsureList;
            GetExceptionIdentifierList = GetExceptionIdentifierList;
            SetRequireList = SetRequireList;
            SetExceptionIdentifierList = SetExceptionIdentifierList;
        }
        #endregion

        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.PropertyType.GetEnsureBlocks"/>.
        /// </summary>
        public IList<IAssertion> GetEnsureList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.PropertyType.GetExceptionIdentifierBlocks"/>.
        /// </summary>
        public IList<IIdentifier> GetExceptionIdentifierList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.PropertyType.SetRequireBlocks"/>.
        /// </summary>
        public IList<IAssertion> SetRequireList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.PropertyType.SetExceptionIdentifierBlocks"/>.
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
                case nameof(GetEnsureBlocks):
                    TargetList = (IList)GetEnsureList;
                    break;

                case nameof(GetExceptionIdentifierBlocks):
                    TargetList = (IList)GetExceptionIdentifierList;
                    break;

                case nameof(SetRequireBlocks):
                    TargetList = (IList)SetRequireList;
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
                IsResolved = ResolvedType.IsAssigned;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IObjectType
        /// <summary>
        /// The type name of the resolved base type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedBaseTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The type name of the resolved base type.
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

                string Result = $"{BaseTypeName}.property: {EntityTypeName}";

                bool IsHandled = false;
                switch (PropertyKind)
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

            if (IsNewInstance)
                ResolveType(instancingClassType.BaseClass.TypeTable, InstancedBaseTypeName, InstancedBaseType, InstancedEntityTypeName, InstancedEntityType, PropertyKind, GetEnsureList, GetExceptionIdentifierList, SetRequireList, SetExceptionIdentifierList, out resolvedTypeName, out resolvedType);

            return Success;
        }
        #endregion

        #region Locate type
        /// <summary>
        /// Locates, or creates, a resolved property type.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="baseTypeName">The type name of the resolved base type.</param>
        /// <param name="baseType">The type of the resolved base type.</param>
        /// <param name="entityTypeName">The type name of the resolved result type.</param>
        /// <param name="entityType">The type of the resolved result type.</param>
        /// <param name="propertyKind">The type of the property.</param>
        /// <param name="getEnsureList">The list of ensure assertions for the getter.</param>
        /// <param name="getExceptionIdentifierList">The list of known exceptions thrown for the getter.</param>
        /// <param name="setRequireList">The list of require assertions for the setter.</param>
        /// <param name="setExceptionIdentifierList">The list of known exceptions thrown for the setter.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void ResolveType(IHashtableEx<ITypeName, ICompiledType> typeTable, ITypeName baseTypeName, ICompiledType baseType, ITypeName entityTypeName, ICompiledType entityType, BaseNode.UtilityType propertyKind, IList<IAssertion> getEnsureList, IList<IIdentifier> getExceptionIdentifierList, IList<IAssertion> setRequireList, IList<IIdentifier> setExceptionIdentifierList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            if (!TypeTableContaining(typeTable, baseType, entityType, propertyKind, getEnsureList, getExceptionIdentifierList, setRequireList, setExceptionIdentifierList, out resolvedTypeName, out resolvedType))
            {
                BuildType(baseTypeName, baseType, entityTypeName, entityType, propertyKind, getEnsureList, getExceptionIdentifierList, setRequireList, setExceptionIdentifierList, out resolvedTypeName, out resolvedType);
                typeTable.Add(resolvedTypeName, resolvedType);
            }
        }

        /// <summary>
        /// Checks if a matching function type exists in a type table.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="baseType">The type of the resolved base type.</param>
        /// <param name="entityType">The type of the resolved result type.</param>
        /// <param name="propertyKind">The type of the property.</param>
        /// <param name="getEnsureList">The list of ensure assertions for the getter.</param>
        /// <param name="getExceptionIdentifierList">The list of known exceptions thrown for the getter.</param>
        /// <param name="setRequireList">The list of require assertions for the setter.</param>
        /// <param name="setExceptionIdentifierList">The list of known exceptions thrown for the setter.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static bool TypeTableContaining(IHashtableEx<ITypeName, ICompiledType> typeTable, ICompiledType baseType, ICompiledType entityType, BaseNode.UtilityType propertyKind, IList<IAssertion> getEnsureList, IList<IIdentifier> getExceptionIdentifierList, IList<IAssertion> setRequireList, IList<IIdentifier> setExceptionIdentifierList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            resolvedTypeName = null;
            resolvedType = null;
            bool Result = false;

            foreach (KeyValuePair<ITypeName, ICompiledType> Entry in typeTable)
                if (Entry.Value is IPropertyType AsPropertyType)
                {
                    if (AsPropertyType.ResolvedBaseType.Item != baseType)
                        continue;

                    if (AsPropertyType.ResolvedEntityType.Item != entityType)
                        continue;

                    if (AsPropertyType.PropertyKind != propertyKind)
                        continue;

                    if (!Assertion.IsAssertionListEqual(AsPropertyType.GetEnsureList, getEnsureList))
                        continue;

                    if (!ExceptionHandler.IdenticalExceptionSignature(AsPropertyType.GetExceptionIdentifierList, getExceptionIdentifierList))
                        continue;

                    if (!Assertion.IsAssertionListEqual(AsPropertyType.SetRequireList, setRequireList))
                        continue;

                    if (!ExceptionHandler.IdenticalExceptionSignature(AsPropertyType.SetExceptionIdentifierList, setExceptionIdentifierList))
                        continue;

                    resolvedTypeName = Entry.Key;
                    resolvedType = AsPropertyType;
                    Result = true;
                    break;
                }

            return Result;
        }

        /// <summary>
        /// Creates a function type with resolved arguments.
        /// </summary>
        /// <param name="baseTypeName">The type name of the resolved base type.</param>
        /// <param name="baseType">The type of the resolved base type.</param>
        /// <param name="entityTypeName">The type name of the resolved result type.</param>
        /// <param name="entityType">The type of the resolved result type.</param>
        /// <param name="propertyKind">The type of the property.</param>
        /// <param name="getEnsureList">The list of ensure assertions for the getter.</param>
        /// <param name="getExceptionIdentifierList">The list of known exceptions thrown for the getter.</param>
        /// <param name="setRequireList">The list of require assertions for the setter.</param>
        /// <param name="setExceptionIdentifierList">The list of known exceptions thrown for the setter.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void BuildType(ITypeName baseTypeName, ICompiledType baseType, ITypeName entityTypeName, ICompiledType entityType, BaseNode.UtilityType propertyKind, IList<IAssertion> getEnsureList, IList<IIdentifier> getExceptionIdentifierList, IList<IAssertion> setRequireList, IList<IIdentifier> setExceptionIdentifierList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            IPropertyType ResolvedPropertyType = new PropertyType(baseTypeName, (IClassType)baseType, entityTypeName, entityType, propertyKind, getEnsureList, getExceptionIdentifierList, setRequireList, setExceptionIdentifierList);

            resolvedTypeName = new TypeName(ResolvedPropertyType.TypeFriendlyName);
            resolvedType = ResolvedPropertyType;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Compares two types.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        public static bool TypesHaveIdenticalSignature(IPropertyType type1, IPropertyType type2)
        {
            bool IsIdentical = true;

            IsIdentical &= ObjectType.TypesHaveIdenticalSignature(type1.ResolvedBaseType.Item, type2.ResolvedBaseType.Item);
            IsIdentical &= ObjectType.TypesHaveIdenticalSignature(type1.ResolvedEntityType.Item, type2.ResolvedEntityType.Item);
            IsIdentical &= type1.PropertyKind == type2.PropertyKind;
            IsIdentical &= Assertion.IsAssertionListEqual(type1.GetEnsureList, type2.GetEnsureList);
            IsIdentical &= ExceptionHandler.IdenticalExceptionSignature(type1.GetExceptionIdentifierList, type2.GetExceptionIdentifierList);
            IsIdentical &= Assertion.IsAssertionListEqual(type1.SetRequireList, type2.SetRequireList);
            IsIdentical &= ExceptionHandler.IdenticalExceptionSignature(type1.SetExceptionIdentifierList, type2.SetExceptionIdentifierList);

            return IsIdentical;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string TypeToString { get { return $"property {{{((IObjectType)BaseType).TypeToString}}}"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Property Type '{TypeToString}'";
        }
        #endregion
    }
}
