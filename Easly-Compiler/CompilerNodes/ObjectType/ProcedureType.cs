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
        OnceReference<IClassType> ResolvedBaseType { get; }
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcedureType"/> class.
        /// </summary>
        /// <param name="baseTypeName">Name of the resolved base type.</param>
        /// <param name="baseType">The resolved base type.</param>
        /// <param name="overloadList">The list of resolved overloads.</param>
        public ProcedureType(ITypeName baseTypeName, IClassType baseType, IList<ICommandOverloadType> overloadList)
        {
            ResolvedBaseTypeName.Item = baseTypeName;
            ResolvedBaseType.Item = baseType;
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
                ResolvedBaseTypeName = new OnceReference<ITypeName>();
                ResolvedBaseType = new OnceReference<IClassType>();
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

        #region Compiler
        /// <summary>
        /// Resolved type name for the base type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedBaseTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// Resolved type for the base type.
        /// </summary>
        public OnceReference<IClassType> ResolvedBaseType { get; private set; } = new OnceReference<IClassType>();
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
                string Result = BaseType != null ? ((IObjectType)BaseType).ResolvedTypeName.Item.Name : ResolvedBaseType.Item.TypeFriendlyName;

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

            IList<ICommandOverloadType> InstancedOverloadList = new List<ICommandOverloadType>();
            foreach (ICommandOverloadType Overload in OverloadList)
            {
                ICommandOverloadType InstancedOverload = Overload;
                CommandOverloadType.InstanciateCommandOverloadType(instancingClassType, ref InstancedOverload, errorList);

                InstancedOverloadList.Add(InstancedOverload);

                if (InstancedOverload != Overload)
                    IsNewInstance = true;
            }

            if (IsNewInstance)
                ResolveType(instancingClassType.BaseClass.TypeTable, InstancedBaseTypeName, InstancedBaseType, InstancedOverloadList, out resolvedTypeName, out resolvedType);

            return Success;
        }
        #endregion

        #region Locate type
        /// <summary>
        /// Locates, or creates, a resolved procedure type.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="baseTypeName">Name of the resolved base type.</param>
        /// <param name="baseType">The resolved base type.</param>
        /// <param name="overloadList">The list of resolved overloads.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void ResolveType(IHashtableEx<ITypeName, ICompiledType> typeTable, ITypeName baseTypeName, ICompiledType baseType, IList<ICommandOverloadType> overloadList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            if (!TypeTableContaining(typeTable, baseType, overloadList, out resolvedTypeName, out resolvedType))
            {
                BuildType(baseTypeName, baseType, overloadList, out resolvedTypeName, out resolvedType);
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
        public static bool TypeTableContaining(IHashtableEx<ITypeName, ICompiledType> typeTable, ICompiledType baseType, IList<ICommandOverloadType> overloadList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
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
                            if (!IsCommandOverloadMatching(typeTable, OverloadItem, AsProcedureType.OverloadList))
                            {
                                AllOverloadsEqual = false;
                                break;
                            }

                        if (AllOverloadsEqual)
                        {
                            resolvedTypeName = Entry.Key;
                            resolvedType = AsProcedureType;
                            Result = true;
                            break;
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
        public static bool IsCommandOverloadMatching(IHashtableEx<ITypeName, ICompiledType> typeTable, ICommandOverloadType overload, IList<ICommandOverloadType> overloadList)
        {
            foreach (ICommandOverloadType Item in overloadList)
            {
                if (overload.ParameterList.Count != Item.ParameterList.Count)
                    continue;

                bool AllParametersMatch = true;
                for (int i = 0; i < overload.ParameterList.Count; i++)
                {
                    if (overload.ParameterList[i].ValidEntity.Item.ResolvedFeatureType.Item != Item.ParameterList[i].ValidEntity.Item.ResolvedFeatureType.Item)
                    {
                        AllParametersMatch = false;
                        break;
                    }

                    IName overloadName = (IName)overload.ParameterList[i].EntityName;
                    IName ItemName = (IName)Item.ParameterList[i].EntityName;
                    if (overloadName != null && ItemName != null && overloadName.ValidText.Item != ItemName.ValidText.Item)
                    {
                        AllParametersMatch = false;
                        break;
                    }
                }
                if (!AllParametersMatch)
                    continue;

                if (overload.ParameterEnd != Item.ParameterEnd)
                    continue;

                if (!Assertion.IsAssertionListEqual(overload.RequireList, Item.RequireList))
                    continue;

                if (!Assertion.IsAssertionListEqual(overload.EnsureList, Item.EnsureList))
                    continue;

                if (!ExceptionHandler.IdenticalExceptionSignature(overload.ExceptionIdentifierList, Item.ExceptionIdentifierList))
                    continue;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a procedure type with resolved arguments.
        /// </summary>
        /// <param name="baseTypeName">Name of the resolved base type.</param>
        /// <param name="baseType">The resolved base type.</param>
        /// <param name="overloadList">The list of resolved overloads.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void BuildType(ITypeName baseTypeName, ICompiledType baseType, IList<ICommandOverloadType> overloadList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            IProcedureType ResolvedProcedureType = new ProcedureType(baseTypeName, (IClassType)baseType, overloadList);

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
            if (!ObjectType.TypesHaveIdenticalSignature(type1.ResolvedBaseType.Item, type2.ResolvedBaseType.Item))
                return false;

            if (type1.OverloadList.Count != type2.OverloadList.Count)
                return false;

            foreach (ICommandOverloadType Overload1 in type1.OverloadList)
            {
                bool MatchingOverload = false;

                foreach (ICommandOverloadType Overload2 in type2.OverloadList)
                    if (CommandOverloadType.CommandOverloadsHaveIdenticalSignature(Overload1, Overload2))
                    {
                        MatchingOverload = true;
                        break;
                    }

                if (!MatchingOverload)
                    return false;
            }

            return true;
        }
        #endregion
    }
}
