namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IFunctionType.
    /// </summary>
    public interface IFunctionType : BaseNode.IFunctionType, IObjectType, INodeWithReplicatedBlocks, ICompiledType
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.FunctionType.OverloadBlocks"/>.
        /// </summary>
        IList<IQueryOverloadType> OverloadList { get; }

        /// <summary>
        /// Resolved type name for the base type.
        /// </summary>
        OnceReference<ITypeName> ResolvedBaseTypeName { get; }

        /// <summary>
        /// Resolved type for the base type.
        /// </summary>
        OnceReference<IClassType> ResolvedBaseType { get; }

        /// <summary>
        /// Resolved type name for the most common result of all overloads.
        /// </summary>
        OnceReference<ITypeName> MostCommonTypeName { get; }

        /// <summary>
        /// Resolved type for the most common result of all overloads.
        /// </summary>
        OnceReference<ICompiledType> MostCommonType { get; }
    }

    /// <summary>
    /// Compiler IFunctionType.
    /// </summary>
    public class FunctionType : BaseNode.FunctionType, IFunctionType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionType"/> class.
        /// This constructor is required for deserialization.
        /// </summary>
        public FunctionType()
        {
            FeatureTable.Seal();
            DiscreteTable.Seal();
            ConformanceTable.Seal();
            ExportTable.Seal();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionType"/> class.
        /// </summary>
        /// <param name="baseTypeName">Name of the resolved base type.</param>
        /// <param name="baseType">The resolved base type.</param>
        /// <param name="overloadList">The list of resolved overloads.</param>
        public FunctionType(ITypeName baseTypeName, IClassType baseType, IList<IQueryOverloadType> overloadList)
            : this()
        {
            ResolvedBaseTypeName.Item = baseTypeName;
            ResolvedBaseType.Item = baseType;
            OverloadList = overloadList;
        }
        #endregion

        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.FunctionType.OverloadBlocks"/>.
        /// </summary>
        public IList<IQueryOverloadType> OverloadList { get; } = new List<IQueryOverloadType>();

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
                MostCommonTypeName = new OnceReference<ITypeName>();
                MostCommonType = new OnceReference<ICompiledType>();
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
                Debug.Assert(MostCommonTypeName.IsAssigned == MostCommonType.IsAssigned);
                IsResolved = ResolvedType.IsAssigned;
                Debug.Assert(MostCommonType.IsAssigned == IsResolved);
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

        /// <summary>
        /// Resolved type name for the most common result of all overloads.
        /// </summary>
        public OnceReference<ITypeName> MostCommonTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// Resolved type for the most common result of all overloads.
        /// </summary>
        public OnceReference<ICompiledType> MostCommonType { get; private set; } = new OnceReference<ICompiledType>();
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
                    IQueryOverloadType Item = OverloadList[i];
                    Result += $".query#{i}{Item.TypeName}";
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

            IsNewInstance |= InstancedBaseType != ResolvedBaseType.Item;

            IList<IQueryOverloadType> InstancedOverloadList = new List<IQueryOverloadType>();
            foreach (IQueryOverloadType Overload in OverloadList)
            {
                IQueryOverloadType InstancedOverload = Overload;
                QueryOverloadType.InstanciateQueryOverloadType(instancingClassType, ref InstancedOverload, errorList);

                InstancedOverloadList.Add(InstancedOverload);
                IsNewInstance |= InstancedOverload != Overload;
            }

            if (IsNewInstance)
                ResolveType(instancingClassType.BaseClass.TypeTable, InstancedBaseTypeName, InstancedBaseType, InstancedOverloadList, out resolvedTypeName, out resolvedType);

            return Success;
        }
        #endregion

        #region Locate type
        /// <summary>
        /// Locates, or creates, a resolved function type.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="baseTypeName">Name of the resolved base type.</param>
        /// <param name="baseType">The resolved base type.</param>
        /// <param name="overloadList">The list of resolved overloads.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void ResolveType(IHashtableEx<ITypeName, ICompiledType> typeTable, ITypeName baseTypeName, ICompiledType baseType, IList<IQueryOverloadType> overloadList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            if (!TypeTableContaining(typeTable, baseType, overloadList, out resolvedTypeName, out resolvedType))
            {
                BuildType(baseTypeName, baseType, overloadList, out resolvedTypeName, out resolvedType);
                typeTable.Add(resolvedTypeName, resolvedType);
            }
        }

        /// <summary>
        /// Checks if a matching function type exists in a type table.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="baseType">The resolved base type.</param>
        /// <param name="overloadList">The list of resolved overloads.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static bool TypeTableContaining(IHashtableEx<ITypeName, ICompiledType> typeTable, ICompiledType baseType, IList<IQueryOverloadType> overloadList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            resolvedTypeName = null;
            resolvedType = null;
            bool Result = false;

            foreach (KeyValuePair<ITypeName, ICompiledType> Entry in typeTable)
                if (Entry.Value is IFunctionType AsFunctionType)
                    if (AsFunctionType.ResolvedBaseType.Item == baseType && AsFunctionType.OverloadList.Count == overloadList.Count)
                    {
                        bool AllOverloadsEqual = true;
                        foreach (IQueryOverloadType OverloadItem in overloadList)
                            AllOverloadsEqual &= IsQueryOverloadMatching(typeTable, OverloadItem, AsFunctionType.OverloadList);

                        if (AllOverloadsEqual)
                        {
                            resolvedTypeName = Entry.Key;
                            resolvedType = AsFunctionType;
                            Result = true;
                        }
                    }

            return Result;
        }

        /// <summary>
        /// Checks if a matching function and overload exists in a type table.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="overload">The overload to check.</param>
        /// <param name="overloadList">The list of other overloads in the candidate function type.</param>
        public static bool IsQueryOverloadMatching(IHashtableEx<ITypeName, ICompiledType> typeTable, IQueryOverloadType overload, IList<IQueryOverloadType> overloadList)
        {
            bool IsMatching = false;

            for (int i = 0; i < overloadList.Count && !IsMatching; i++)
            {
                IQueryOverloadType Item = overloadList[i];

                IsMatching = true;
                IsMatching &= IsParametersMatching(overload, Item);
                IsMatching &= IsResultsMatching(overload, Item);
                IsMatching &= Assertion.IsAssertionListEqual(overload.RequireList, Item.RequireList);
                IsMatching &= Assertion.IsAssertionListEqual(overload.EnsureList, Item.EnsureList);
                IsMatching &= ExceptionHandler.IdenticalExceptionSignature(overload.ExceptionIdentifierList, Item.ExceptionIdentifierList);
            }

            return IsMatching;
        }

        private static bool IsParametersMatching(IQueryOverloadType overload1, IQueryOverloadType overload2)
        {
            bool IsMatching = true;

            IsMatching &= overload1.ParameterList.Count == overload2.ParameterList.Count;
            IsMatching &= overload1.ParameterEnd == overload2.ParameterEnd;

            for (int i = 0; i < overload1.ParameterList.Count && i < overload2.ParameterList.Count; i++)
            {
                IScopeAttributeFeature OverloadAttribute1 = overload1.ParameterList[i].ValidEntity.Item;
                IScopeAttributeFeature OverloadAttribute2 = overload2.ParameterList[i].ValidEntity.Item;

                IsMatching &= OverloadAttribute1.ResolvedFeatureType.Item == OverloadAttribute2.ResolvedFeatureType.Item;
            }

            return IsMatching;
        }

        private static bool IsResultsMatching(IQueryOverloadType overload1, IQueryOverloadType overload2)
        {
            bool IsMatching = true;

            IsMatching &= overload1.ResultList.Count == overload2.ResultList.Count;

            for (int i = 0; i < overload1.ResultList.Count && i < overload2.ResultList.Count; i++)
            {
                IScopeAttributeFeature OverloadAttribute1 = overload1.ResultList[i].ValidEntity.Item;
                IScopeAttributeFeature OverloadAttribute2 = overload2.ResultList[i].ValidEntity.Item;

                IsMatching &= OverloadAttribute1.ResolvedFeatureType.Item == OverloadAttribute2.ResolvedFeatureType.Item;
            }

            return IsMatching;
        }

        /// <summary>
        /// Creates a function type with resolved arguments.
        /// </summary>
        /// <param name="baseTypeName">Name of the resolved base type.</param>
        /// <param name="baseType">The resolved base type.</param>
        /// <param name="overloadList">The list of resolved overloads.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void BuildType(ITypeName baseTypeName, ICompiledType baseType, IList<IQueryOverloadType> overloadList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            FunctionType ResolvedFunctionType = new FunctionType(baseTypeName, (IClassType)baseType, overloadList);

            foreach (IQueryOverloadType Item in overloadList)
                foreach (IEntityDeclaration Entity in Item.ResultList)
                {
                    ITypeName EntityTypeName = Entity.ValidEntity.Item.ResolvedFeatureTypeName.Item;
                    ICompiledType EntityType = Entity.ValidEntity.Item.ResolvedFeatureType.Item;
                    string EntityName = Entity.ValidEntity.Item.ValidFeatureName.Item.Name;

                    IExpressionType ResultExpressionType = new ExpressionType(EntityTypeName, EntityType, EntityName);
                    Item.Result.Add(ResultExpressionType);
                }

            resolvedTypeName = new TypeName(ResolvedFunctionType.TypeFriendlyName);
            resolvedType = ResolvedFunctionType;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Compares two types.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        public static bool TypesHaveIdenticalSignature(IFunctionType type1, IFunctionType type2)
        {
            bool IsIdentical = true;

            IsIdentical &= ObjectType.TypesHaveIdenticalSignature(type1.ResolvedBaseType.Item, type2.ResolvedBaseType.Item);
            IsIdentical &= type1.OverloadList.Count != type2.OverloadList.Count;

            foreach (IQueryOverloadType Overload1 in type1.OverloadList)
            {
                bool MatchingOverload = false;

                foreach (IQueryOverloadType Overload2 in type2.OverloadList)
                    MatchingOverload |= QueryOverloadType.QueryOverloadsHaveIdenticalSignature(Overload1, Overload2);

                IsIdentical &= MatchingOverload;
            }

            return IsIdentical;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string TypeToString { get { return $"function {{{((IObjectType)BaseType).TypeToString}}}"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Function Type '{TypeToString}'";
        }
        #endregion
    }
}
