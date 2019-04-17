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
    public interface ITupleType : BaseNode.ITupleType, IObjectType, INodeWithReplicatedBlocks, ICompiledType
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.TupleType.EntityDeclarationBlocks"/>.
        /// </summary>
        IList<IEntityDeclaration> EntityDeclarationList { get; }
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TupleType"/> class.
        /// </summary>
        /// <param name="entityDeclarationList">The resolved list of fields.</param>
        /// <param name="sharing">The type sharing.</param>
        public TupleType(IList<IEntityDeclaration> entityDeclarationList, BaseNode.SharingType sharing)
        {
            EntityDeclarationList = entityDeclarationList;
            Sharing = sharing;
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
        public void FillReplicatedList(string propertyEntityDeclaration, List<BaseNode.INode> nodeList)
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
        public IQueryOverload EmbeddingOverload { get; private set; }

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
            EmbeddingOverload = parentSource is IQueryOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
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
                ResolvedTypeName = new OnceReference<ITypeName>();
                ResolvedType = new OnceReference<ICompiledType>();
                DiscreteTable = new HashtableEx<IFeatureName, IDiscrete>();
                FeatureTable = new HashtableEx<IFeatureName, IFeatureInstance>();
                ExportTable = new HashtableEx<IFeatureName, IHashtableEx<string, IClass>>();
                ConformanceTable = new HashtableEx<ITypeName, ICompiledType>();
                InstancingRecordList = new List<TypeInstancingRecord>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
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

            IList<IEntityDeclaration> InstancedFieldList = new List<IEntityDeclaration>();
            foreach (IEntityDeclaration Field in EntityDeclarationList)
            {
                ITypeName InstancedFieldTypeName = Field.ResolvedEntityTypeName.Item;
                ICompiledType InstancedFieldType = Field.ResolvedEntityType.Item;
                Success &= InstancedFieldType.InstanciateType(instancingClassType, ref InstancedFieldTypeName, ref InstancedFieldType, errorList);

                IEntityDeclaration InstancedField = new EntityDeclaration();
                InstancedField.ResolvedEntityTypeName.Item = InstancedFieldTypeName;
                InstancedField.ResolvedEntityType.Item = InstancedFieldType;

                InstancedFieldList.Add(InstancedField);

                if (InstancedFieldType != Field.ResolvedEntityType.Item)
                    IsNewInstance = true;
            }

            if (IsNewInstance)
                ResolveType(instancingClassType.BaseClass.TypeTable, EntityDeclarationList, Sharing, out resolvedTypeName, out resolvedType);

            return Success;
        }
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
        public static void ResolveType(IHashtableEx<ITypeName, ICompiledType> typeTable, IList<IEntityDeclaration> entityDeclarationList, BaseNode.SharingType sharing, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
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
        public static bool TypeTableContaining(IHashtableEx<ITypeName, ICompiledType> typeTable, IList<IEntityDeclaration> entityDeclarationList, BaseNode.SharingType sharing, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
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
                            if (entityDeclarationList[i].ResolvedEntityType.Item != AsTupleType.EntityDeclarationList[i].ResolvedEntityType.Item)
                            {
                                AllFieldsEqual = false;
                                break;
                            }

                        if (AllFieldsEqual)
                        {
                            resolvedTypeName = Entry.Key;
                            resolvedType = AsTupleType;
                            Result = true;
                            break;
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
            ITupleType ResolvedTupleType = new TupleType(entityDeclarationList, sharing);

            resolvedTypeName = new TypeName(ResolvedTupleType.TypeFriendlyName);
            resolvedType = ResolvedTupleType;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Compares two types.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        public static bool TypesHaveIdenticalSignature(ITupleType type1, ITupleType type2)
        {
            for (int i = 0; i < type1.EntityDeclarationList.Count && i < type2.EntityDeclarationList.Count; i++)
                if (!ObjectType.TypesHaveIdenticalSignature(type1.EntityDeclarationList[i].ResolvedEntityType.Item, type2.EntityDeclarationList[i].ResolvedEntityType.Item))
                    return false;

            return true;
        }
        #endregion
    }
}
