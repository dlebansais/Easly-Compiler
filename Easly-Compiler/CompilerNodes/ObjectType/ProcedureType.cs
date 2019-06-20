namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IProcedureType.
    /// </summary>
    public interface IProcedureType : BaseNode.IProcedureType, IObjectType, INodeWithReplicatedBlocks, ICompiledType
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.ProcedureType.OverloadBlocks"/>.
        /// </summary>
        IList<ICommandOverloadType> OverloadList { get; }

        /// <summary>
        /// Resolved type name for the base type.
        /// </summary>
        OnceReference<ITypeName> ResolvedBaseTypeName { get; }

        /// <summary>
        /// Resolved type for the base type.
        /// </summary>
        OnceReference<ICompiledTypeWithFeature> ResolvedBaseType { get; }
    }

    /// <summary>
    /// Compiler IProcedureType.
    /// </summary>
    public class ProcedureType : BaseNode.ProcedureType, IProcedureType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcedureType"/> class.
        /// This constructor is required for deserialization.
        /// </summary>
        public ProcedureType()
        {
            FeatureTable.Seal();
            DiscreteTable.Seal();
            ConformanceTable.Seal();
            ExportTable.Seal();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcedureType"/> class.
        /// </summary>
        /// <param name="baseTypeName">Name of the resolved base type.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="resolvedBaseType">The resolved base type.</param>
        /// <param name="overloadList">The list of resolved overloads.</param>
        public ProcedureType(ITypeName baseTypeName, IObjectType baseType, ICompiledTypeWithFeature resolvedBaseType, IList<ICommandOverloadType> overloadList)
            : this()
        {
            BaseType = baseType;

            ResolvedBaseTypeName.Item = baseTypeName;
            ResolvedBaseType.Item = resolvedBaseType;
            OverloadList = overloadList;
        }
        #endregion

        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.ProcedureType.OverloadBlocks"/>.
        /// </summary>
        public IList<ICommandOverloadType> OverloadList { get; } = new List<ICommandOverloadType>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyOverload">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyOverload, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyOverload)
            {
                case nameof(OverloadBlocks):
                    TargetList = (IList)OverloadList;
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
                ResolvedBaseType = new OnceReference<ICompiledTypeWithFeature>();
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
                IsResolved = ResolvedType.IsAssigned;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Resolved type name for the base type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedBaseTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// Resolved type for the base type.
        /// </summary>
        public OnceReference<ICompiledTypeWithFeature> ResolvedBaseType { get; private set; } = new OnceReference<ICompiledTypeWithFeature>();
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
                string Result = ResolvedBaseType.Item.TypeFriendlyName;

                for (int i = 0; i < OverloadList.Count; i++)
                {
                    ICommandOverloadType Item = OverloadList[i];
                    Result += $".command#{i}{Item.TypeName}";
                }

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
        /// The type to use instead of this type for a source or destination type, for the purpose of path searching.
        /// </summary>
        public ICompiledType TypeAsDestinationOrSource { get { return this; } }

        /// <summary>
        /// Creates an instance of a class type, or reuse an existing instance.
        /// </summary>
        /// <param name="instancingClassType">The class type to instanciate.</param>
        /// <param name="resolvedTypeName">The proposed type instance name.</param>
        /// <param name="resolvedType">The proposed type instance.</param>
        public void InstanciateType(ICompiledTypeWithFeature instancingClassType, ref ITypeName resolvedTypeName, ref ICompiledType resolvedType)
        {
            bool IsNewInstance = false;

            ITypeName InstancedBaseTypeName = ResolvedBaseTypeName.Item;
            ICompiledType InstancedBaseType = ResolvedBaseType.Item;
            InstancedBaseType.InstanciateType(instancingClassType, ref InstancedBaseTypeName, ref InstancedBaseType);
            IsNewInstance |= InstancedBaseType != ResolvedBaseType.Item;

            IList<ICommandOverloadType> InstancedOverloadList = new List<ICommandOverloadType>();
            foreach (ICommandOverloadType Overload in OverloadList)
            {
                ICommandOverloadType InstancedOverload = Overload;
                CommandOverloadType.InstanciateCommandOverloadType(instancingClassType, ref InstancedOverload);
                IsNewInstance |= InstancedOverload != Overload;

                InstancedOverloadList.Add(InstancedOverload);
            }

            if (IsNewInstance)
            {
                ISealableDictionary<ITypeName, ICompiledType> TypeTable = instancingClassType.GetTypeTable();
                ResolveType(TypeTable, InstancedBaseTypeName, (IObjectType)BaseType, (ICompiledTypeWithFeature)InstancedBaseType, InstancedOverloadList, out resolvedTypeName, out IProcedureType ResolvedProcedureType);
                resolvedType = ResolvedProcedureType;
            }
        }
        #endregion

        #region Locate type
        /// <summary>
        /// Locates, or creates, a resolved procedure type.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="baseTypeName">Name of the resolved base type.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="resolvedBaseType">The resolved base type.</param>
        /// <param name="overloadList">The list of resolved overloads.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void ResolveType(ISealableDictionary<ITypeName, ICompiledType> typeTable, ITypeName baseTypeName, IObjectType baseType, ICompiledTypeWithFeature resolvedBaseType, IList<ICommandOverloadType> overloadList, out ITypeName resolvedTypeName, out IProcedureType resolvedType)
        {
            if (!TypeTableContaining(typeTable, resolvedBaseType, overloadList, out resolvedTypeName, out resolvedType))
            {
                BuildType(baseTypeName, baseType, resolvedBaseType, overloadList, out resolvedTypeName, out resolvedType);
                typeTable.Add(resolvedTypeName, resolvedType);
            }
        }

        /// <summary>
        /// Checks if a matching procedure type exists in a type table.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="baseType">The resolved base type.</param>
        /// <param name="overloadList">The list of resolved overloads.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static bool TypeTableContaining(ISealableDictionary<ITypeName, ICompiledType> typeTable, ICompiledTypeWithFeature baseType, IList<ICommandOverloadType> overloadList, out ITypeName resolvedTypeName, out IProcedureType resolvedType)
        {
            resolvedTypeName = null;
            resolvedType = null;
            bool Result = false;

            foreach (KeyValuePair<ITypeName, ICompiledType> Entry in typeTable)
                if (Entry.Value is IProcedureType AsProcedureType)
                    if (AsProcedureType.ResolvedBaseType.Item == baseType && AsProcedureType.OverloadList.Count == overloadList.Count)
                    {
                        bool AllOverloadsEqual = true;
                        foreach (ICommandOverloadType OverloadItem in overloadList)
                            AllOverloadsEqual &= IsCommandOverloadMatching(typeTable, OverloadItem, AsProcedureType.OverloadList);

                        if (AllOverloadsEqual)
                        {
                            Debug.Assert(!Result);

                            resolvedTypeName = Entry.Key;
                            resolvedType = AsProcedureType;
                            Result = true;
                        }
                    }

            return Result;
        }

        /// <summary>
        /// Checks if a matching procedure and overload exists in a type table.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="overload">The overload to check.</param>
        /// <param name="overloadList">The list of other overloads in the candidate procedure type.</param>
        public static bool IsCommandOverloadMatching(ISealableDictionary<ITypeName, ICompiledType> typeTable, ICommandOverloadType overload, IList<ICommandOverloadType> overloadList)
        {
            bool IsOverloadFound = false;

            foreach (ICommandOverloadType Item in overloadList)
            {
                bool IsMatching = true;

                IsMatching &= overload.ParameterList.Count == Item.ParameterList.Count;

                for (int i = 0; i < overload.ParameterList.Count && i < Item.ParameterList.Count; i++)
                {
                    IsMatching &= overload.ParameterList[i].ValidEntity.Item.ResolvedEffectiveType.Item == Item.ParameterList[i].ValidEntity.Item.ResolvedEffectiveType.Item;
                    IsMatching &= overload.ParameterList[i].ValidEntity.Item.DefaultValue.IsAssigned == Item.ParameterList[i].ValidEntity.Item.DefaultValue.IsAssigned;
                    IsMatching &= IsDefaultValueMatching(overload.ParameterList[i].ValidEntity.Item.DefaultValue, Item.ParameterList[i].ValidEntity.Item.DefaultValue);

                    if (IsMatching)
                    {
                        IName overloadName = (IName)overload.ParameterList[i].EntityName;
                        IName ItemName = (IName)Item.ParameterList[i].EntityName;

                        Debug.Assert(overloadName != null);
                        Debug.Assert(ItemName != null);

                        IsMatching &= overloadName.ValidText.Item == ItemName.ValidText.Item;
                    }
                }

                IsMatching &= overload.ParameterEnd == Item.ParameterEnd;
                IsMatching &= Assertion.IsAssertionListEqual(overload.RequireList, Item.RequireList);
                IsMatching &= Assertion.IsAssertionListEqual(overload.EnsureList, Item.EnsureList);
                IsMatching &= ExceptionHandler.IdenticalExceptionSignature(overload.ExceptionIdentifierList, Item.ExceptionIdentifierList);

                IsOverloadFound |= IsMatching;
            }

            return IsOverloadFound;
        }

        private static bool IsDefaultValueMatching(IOptionalReference<IExpression> value1, IOptionalReference<IExpression> value2)
        {
            return value1.IsAssigned && value2.IsAssigned && Expression.IsExpressionEqual(value1.Item, value2.Item);
        }

        /// <summary>
        /// Creates a procedure type with resolved arguments.
        /// </summary>
        /// <param name="baseTypeName">Name of the resolved base type.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="resolvedBaseType">The resolved base type.</param>
        /// <param name="overloadList">The list of resolved overloads.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void BuildType(ITypeName baseTypeName, IObjectType baseType, ICompiledTypeWithFeature resolvedBaseType, IList<ICommandOverloadType> overloadList, out ITypeName resolvedTypeName, out IProcedureType resolvedType)
        {
            IProcedureType ResolvedProcedureType = new ProcedureType(baseTypeName, baseType, resolvedBaseType, overloadList);

            resolvedTypeName = new TypeName(ResolvedProcedureType.TypeFriendlyName);
            resolvedType = ResolvedProcedureType;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Compares two types.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        public static bool TypesHaveIdenticalSignature(IProcedureType type1, IProcedureType type2)
        {
            bool IsIdentical = true;

            IsIdentical &= ObjectType.TypesHaveIdenticalSignature(type1.ResolvedBaseType.Item, type2.ResolvedBaseType.Item);
            IsIdentical &= type1.OverloadList.Count == type2.OverloadList.Count;

            foreach (ICommandOverloadType Overload1 in type1.OverloadList)
            {
                bool MatchingOverload = false;

                foreach (ICommandOverloadType Overload2 in type2.OverloadList)
                    MatchingOverload |= CommandOverloadType.CommandOverloadsHaveIdenticalSignature(Overload1, Overload2);

                IsIdentical &= MatchingOverload;
            }

            return IsIdentical;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string TypeToString { get { return $"procedure {{{((IObjectType)BaseType).TypeToString}}}"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Procedure Type '{TypeToString}'";
        }
        #endregion
    }
}
