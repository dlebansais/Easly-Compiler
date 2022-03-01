namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler ITupleType.
    /// </summary>
    public interface ITupleType : IObjectType, INodeWithReplicatedBlocks, ICompiledTypeWithFeature
    {
        /// <summary>
        /// Gets or sets how the type is shared.
        /// </summary>
        BaseNode.SharingType Sharing { get; }

        /// <summary>
        /// Gets or sets the list of elements in the tuple.
        /// </summary>
        BaseNode.IBlockList<BaseNode.EntityDeclaration> EntityDeclarationBlocks { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.TupleType.EntityDeclarationBlocks"/>.
        /// </summary>
        IList<IEntityDeclaration> EntityDeclarationList { get; }

        /// <summary>
        /// Table of resolved fields.
        /// </summary>
        ISealableDictionary<string, IScopeAttributeFeature> FieldTable { get; }

        /// <summary>
        /// Creates a clone of this type with renamed identifiers.
        /// </summary>
        /// <param name="renamedFieldTable">The rename table for fields.</param>
        ITupleType CloneWithRenames(ISealableDictionary<IFeatureName, IFeatureInstance> renamedFieldTable);
    }

    /// <summary>
    /// Compiler ITupleType.
    /// </summary>
    public class TupleType : BaseNode.TupleType, ITupleType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="TupleType"/> class.
        /// This constructor is required for deserialization.
        /// </summary>
        public TupleType()
        {
            FeatureTable.Seal();
            DiscreteTable.Seal();
            ConformanceTable.Seal();
            ExportTable.Seal();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TupleType"/> class.
        /// </summary>
        /// <param name="entityDeclarationList">The resolved list of fields.</param>
        /// <param name="sharing">The type sharing.</param>
        /// <param name="renamedFieldTable">The list of fields to rename.</param>
        public TupleType(IList<IEntityDeclaration> entityDeclarationList, BaseNode.SharingType sharing, ISealableDictionary<IFeatureName, IFeatureInstance> renamedFieldTable)
        {
            EntityDeclarationList = entityDeclarationList;
            Sharing = sharing;

            FeatureTable.Merge(renamedFieldTable);
            FeatureTable.Seal();
            DiscreteTable.Seal();
            ConformanceTable.Seal();
            ExportTable.Seal();
        }
        #endregion

        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.TupleType.EntityDeclarationBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> EntityDeclarationList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyEntityDeclaration">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyEntityDeclaration, List<BaseNode.Node> nodeList)
        {
            IList TargetList = null;

            switch (propertyEntityDeclaration)
            {
                case nameof(EntityDeclarationBlocks):
                    TargetList = (IList)EntityDeclarationList;
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Contract || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                ResolvedTypeName = new OnceReference<ITypeName>();
                ResolvedType = new OnceReference<ICompiledType>();
                DiscreteTable = new SealableDictionary<IFeatureName, IDiscrete>();
                FeatureTable = new SealableDictionary<IFeatureName, IFeatureInstance>();
                ExportTable = new SealableDictionary<IFeatureName, ISealableDictionary<string, IClass>>();
                ConformanceTable = new SealableDictionary<ITypeName, ICompiledType>();
                InstancingRecordList = new List<TypeInstancingRecord>();
                OriginatingTypedef = new OnceReference<ITypedef>();
                FieldTable = new SealableDictionary<string, IScopeAttributeFeature>();
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
                IsResolved = ResolvedType.IsAssigned;
                Debug.Assert(FieldTable.IsSealed == IsResolved);
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
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

        #region Implementation of ICompiledTypeWithFeature
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
                string Result = "Tuple";

                string FieldList = string.Empty;
                for (int i = 0; i < EntityDeclarationList.Count; i++)
                {
                    IEntityDeclaration Item = EntityDeclarationList[i];
                    IObjectType ItemType = (IObjectType)Item.EntityType;
                    ITypeName ItemResolvedTypeName = ItemType.ResolvedTypeName.Item;

                    FieldList += $".field#{i}: {ItemResolvedTypeName.Name}";
                }

                Result = $"Tuple{{{FieldList}}}";
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
        /// Gets the type table for this type.
        /// </summary>
        public ISealableDictionary<ITypeName, ICompiledType> GetTypeTable()
        {
            return TypeTable;
        }

        /// <summary>
        /// Creates an instance of a class type, or reuse an existing instance.
        /// </summary>
        /// <param name="instancingClassType">The class type to instanciate.</param>
        /// <param name="resolvedTypeName">The proposed type instance name.</param>
        /// <param name="resolvedType">The proposed type instance.</param>
        public void InstanciateType(ICompiledTypeWithFeature instancingClassType, ref ITypeName resolvedTypeName, ref ICompiledType resolvedType)
        {
            ISealableDictionary<ITypeName, ICompiledType> TypeTable = GetTypeTable();
            Debug.Assert(TypeTable.Count == 0);

            bool IsNewInstance = false;

            IList<IEntityDeclaration> InstancedFieldList = new List<IEntityDeclaration>();
            foreach (IEntityDeclaration Field in EntityDeclarationList)
            {
                Debug.Assert(Field.ValidEntity.IsAssigned);
                Debug.Assert(Field.ValidEntity.Item.ResolvedEffectiveTypeName.IsAssigned);
                Debug.Assert(Field.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                ITypeName InstancedFieldTypeName = Field.ValidEntity.Item.ResolvedEffectiveTypeName.Item;
                ICompiledType InstancedFieldType = Field.ValidEntity.Item.ResolvedEffectiveType.Item;

                InstancedFieldType.InstanciateType(instancingClassType, ref InstancedFieldTypeName, ref InstancedFieldType);
                IsNewInstance |= InstancedFieldType != Field.ValidEntity.Item.ResolvedEffectiveType.Item;

                IEntityDeclaration InstancedField = new EntityDeclaration(Field, InstancedFieldTypeName, InstancedFieldType);
                InstancedFieldList.Add(InstancedField);
            }

            if (IsNewInstance)
            {
                ISealableDictionary<ITypeName, ICompiledType> InstancingTypeTable = instancingClassType.GetTypeTable();
                ResolveType(InstancingTypeTable, EntityDeclarationList, Sharing, out resolvedTypeName, out resolvedType);
            }
        }

        private ISealableDictionary<ITypeName, ICompiledType> TypeTable { get; } = new SealableDictionary<ITypeName, ICompiledType>();
        #endregion

        #region Locate type
        /// <summary>
        /// Locates, or creates, a resolved tuple type.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="entityDeclarationList">The resolved list of fields.</param>
        /// <param name="sharing">The type sharing.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void ResolveType(ISealableDictionary<ITypeName, ICompiledType> typeTable, IList<IEntityDeclaration> entityDeclarationList, BaseNode.SharingType sharing, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            if (!TypeTableContaining(typeTable, entityDeclarationList, sharing, out resolvedTypeName, out resolvedType))
            {
                BuildType(entityDeclarationList, sharing, out resolvedTypeName, out resolvedType);
                typeTable.Add(resolvedTypeName, resolvedType);
            }
        }

        /// <summary>
        /// Checks if a matching tuple type exists in a type table.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="entityDeclarationList">The resolved list of fields.</param>
        /// <param name="sharing">The type sharing.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static bool TypeTableContaining(ISealableDictionary<ITypeName, ICompiledType> typeTable, IList<IEntityDeclaration> entityDeclarationList, BaseNode.SharingType sharing, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            resolvedTypeName = null;
            resolvedType = null;
            bool Result = false;

            foreach (KeyValuePair<ITypeName, ICompiledType> Entry in typeTable)
                if (Entry.Value is ITupleType AsTupleType)
                    if (AsTupleType.EntityDeclarationList.Count == entityDeclarationList.Count && AsTupleType.Sharing == sharing)
                    {
                        bool AllFieldsEqual = true;
                        for (int i = 0; i < entityDeclarationList.Count; i++)
                        {
                            Debug.Assert(entityDeclarationList[i].ValidEntity.IsAssigned);
                            Debug.Assert(entityDeclarationList[i].ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
                            Debug.Assert(AsTupleType.EntityDeclarationList[i].ValidEntity.IsAssigned);
                            Debug.Assert(AsTupleType.EntityDeclarationList[i].ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                            AllFieldsEqual &= entityDeclarationList[i].ValidEntity.Item.ResolvedEffectiveType.Item == AsTupleType.EntityDeclarationList[i].ValidEntity.Item.ResolvedEffectiveType.Item;
                        }

                        if (AllFieldsEqual)
                        {
                            Debug.Assert(!Result);

                            resolvedTypeName = Entry.Key;
                            resolvedType = AsTupleType;
                            Result = true;
                        }
                    }

            return Result;
        }

        /// <summary>
        /// Creates a tuple type with resolved arguments.
        /// </summary>
        /// <param name="entityDeclarationList">The resolved list of fields.</param>
        /// <param name="sharing">The type sharing.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void BuildType(IList<IEntityDeclaration> entityDeclarationList, BaseNode.SharingType sharing, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            ISealableDictionary<IFeatureName, IFeatureInstance> FeatureTable = new SealableDictionary<IFeatureName, IFeatureInstance>();
            foreach (IEntityDeclaration Item in entityDeclarationList)
            {
                Debug.Assert(Item.ValidEntity.IsAssigned);
                IScopeAttributeFeature ValidEntity = Item.ValidEntity.Item;

                Debug.Assert(ValidEntity.ValidFeatureName.IsAssigned);
                IFeatureName FeatureName = ValidEntity.ValidFeatureName.Item;

                IClass EmbeddingClass = Item.EmbeddingClass;
                IFeatureInstance FeatureInstance = new FeatureInstance(EmbeddingClass, ValidEntity);

                FeatureTable.Add(FeatureName, FeatureInstance);
            }

            ITupleType ResolvedTupleType = new TupleType(entityDeclarationList, sharing, FeatureTable);

            resolvedTypeName = new TypeName(ResolvedTupleType.TypeFriendlyName);
            resolvedType = ResolvedTupleType;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Table of resolved fields.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> FieldTable { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// Compares two types.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        public static bool TypesHaveIdenticalSignature(ITupleType type1, ITupleType type2)
        {
            bool IsIdentical = true;

            for (int i = 0; i < type1.EntityDeclarationList.Count && i < type2.EntityDeclarationList.Count; i++)
            {
                Debug.Assert(type1.EntityDeclarationList[i].ValidEntity.IsAssigned);
                Debug.Assert(type1.EntityDeclarationList[i].ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
                Debug.Assert(type2.EntityDeclarationList[i].ValidEntity.IsAssigned);
                Debug.Assert(type2.EntityDeclarationList[i].ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
                IsIdentical &= ObjectType.TypesHaveIdenticalSignature(type1.EntityDeclarationList[i].ValidEntity.Item.ResolvedEffectiveType.Item, type2.EntityDeclarationList[i].ValidEntity.Item.ResolvedEffectiveType.Item);
            }

            return IsIdentical;
        }

        /// <summary>
        /// Creates a clone of this type with renamed identifiers.
        /// </summary>
        /// <param name="renamedFieldTable">The rename table for fields.</param>
        public ITupleType CloneWithRenames(ISealableDictionary<IFeatureName, IFeatureInstance> renamedFieldTable)
        {
            ITupleType ClonedTupleType = new TupleType(EntityDeclarationList, Sharing, renamedFieldTable);

            return ClonedTupleType;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string TypeToString { get { return $"tuple[{EntityDeclaration.EntityDeclarationListToString(EntityDeclarationList)}]"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Tuple Type '{TypeToString}'";
        }
        #endregion
    }
}
