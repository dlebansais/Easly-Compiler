namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler-only IClassType.
    /// </summary>
    public interface IClassType : BaseNode.IShareableType, ICompiledTypeWithFeature
    {
        /// <summary>
        /// The source from which this type is issued.
        /// </summary>
        IObjectType SourceType { get; }

        /// <summary>
        /// The class used to instanciate this type.
        /// </summary>
        IClass BaseClass { get; }

        /// <summary>
        /// Arguments if the class is generic.
        /// </summary>
        ISealableDictionary<string, ICompiledType> TypeArgumentTable { get; }

        /// <summary>
        /// Typedefs available in this type.
        /// </summary>
       ISealableDictionary<IFeatureName, ITypedefType> TypedefTable { get; }

        /// <summary>
        /// Creates a clone of this type with renamed identifiers.
        /// </summary>
        /// <param name="renamedExportTable">The rename table for exports.</param>
        /// <param name="renamedTypedefTable">The rename table for typedefs.</param>
        /// <param name="renamedDiscreteTable">The rename table for discretes.</param>
        /// <param name="renamedFeatureTable">The rename table for features.</param>
        /// <param name="instancingClassType">The type that is requesting cloning.</param>
        IClassType CloneWithRenames(ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> renamedExportTable, ISealableDictionary<IFeatureName, ITypedefType> renamedTypedefTable, ISealableDictionary<IFeatureName, IDiscrete> renamedDiscreteTable, ISealableDictionary<IFeatureName, IFeatureInstance> renamedFeatureTable, IClassType instancingClassType);

        /// <summary>
        /// True if an instance of the class is cloned at some point.
        /// </summary>
        bool IsUsedInCloneOf { get; }

        /// <summary>
        /// Sets the <see cref="IsUsedInCloneOf"/> flag.
        /// </summary>
        void MarkAsUsedInCloneOf();
    }

    /// <summary>
    /// Compiler-only IClassType.
    /// </summary>
    public class ClassType : BaseNode.ShareableType, IClassType
    {
        #region Init
        static ClassType()
        {
            ClassAnyType = AssignTypeToClassAny(Class.ClassAny);
            ClassAnyReferenceType = AssignTypeToClassAny(Class.ClassAnyReference);
            ClassAnyValueType = AssignTypeToClassAny(Class.ClassAnyValue);
        }

        private static IClassType AssignTypeToClassAny(IClass classAny)
        {
            IClassType Result = new ClassType(classAny, new SealableDictionary<string, ICompiledType>(), null, null);
            classAny.ResolvedClassTypeName.Item = new TypeName(classAny.ValidClassName);
            classAny.ResolvedClassType.Item = Result;
            classAny.ResolvedAsCompiledType.Item = Result;

            Debug.Assert(classAny.FeatureTable.IsSealed);
            Debug.Assert(classAny.FeatureTable.Count == 0);
            Debug.Assert(classAny.DiscreteTable.IsSealed);
            Debug.Assert(classAny.DiscreteTable.Count == 0);
            Debug.Assert(classAny.DiscreteWithValueTable.IsSealed);
            Debug.Assert(classAny.DiscreteWithValueTable.Count == 0);
            Debug.Assert(classAny.ExportTable.IsSealed);
            Debug.Assert(classAny.ExportTable.Count == 0);
            Debug.Assert(classAny.TypedefTable.IsSealed);
            Debug.Assert(classAny.TypedefTable.Count == 0);

            Result.FeatureTable.Seal();
            Result.DiscreteTable.Seal();
            Result.ExportTable.Seal();
            Result.TypedefTable.Seal();

            if (classAny != Class.ClassAny)
                Result.ConformanceTable.Add(Class.ClassAny.ResolvedClassTypeName.Item, Class.ClassAny.ResolvedClassType.Item);
            Result.ConformanceTable.Seal();

            return Result;
        }

        /// <summary>
        /// Compiler 'Any' type.
        /// </summary>
        public static IClassType ClassAnyType { get; }

        /// <summary>
        /// Compiler 'Any Reference' type.
        /// </summary>
        public static IClassType ClassAnyReferenceType { get; }

        /// <summary>
        /// Compiler 'Any Value' type.
        /// </summary>
        public static IClassType ClassAnyValueType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassType"/> class.
        /// </summary>
        /// <param name="sourceType">The source from which this type is issued.</param>
        /// <param name="baseClass">The class used to instanciate this type.</param>
        /// <param name="typeArgumentTable">Arguments if the class is generic.</param>
        public ClassType(IObjectType sourceType, IClass baseClass, ISealableDictionary<string, ICompiledType> typeArgumentTable)
        {
            Debug.Assert(!baseClass.ResolvedClassType.IsAssigned);

            SourceType = sourceType;
            BaseClass = baseClass;
            TypeArgumentTable = typeArgumentTable;
        }

        /// <summary>
        /// Creates a <see cref="ClassType"/>.
        /// </summary>
        /// <param name="baseClass">The class used to instanciate this type.</param>
        /// <param name="typeArgumentTable">Arguments if the class is generic.</param>
        /// <param name="instancingClassType">The class type if this instance is a derivation (such as renaming).</param>
        public static IClassType Create(IClass baseClass, ISealableDictionary<string, ICompiledType> typeArgumentTable, ICompiledTypeWithFeature instancingClassType)
        {
            ISealableDictionary<ITypeName, ICompiledType> ConformanceTable = new SealableDictionary<ITypeName, ICompiledType>();

            if (baseClass.ResolvedClassType.IsAssigned)
            {
                IClassType ResolvedClassType = baseClass.ResolvedClassType.Item;

                if (ResolvedClassType.ConformanceTable.IsSealed)
                {
                    foreach (IInheritance InheritanceItem in baseClass.InheritanceList)
                        if (InheritanceItem.Conformance == BaseNode.ConformanceType.Conformant)
                        {
                            ITypeName ParentTypeName = InheritanceItem.ResolvedParentTypeName.Item;
                            ICompiledType ParentType = InheritanceItem.ResolvedParentType.Item;
                            ParentType.InstanciateType(instancingClassType, ref ParentTypeName, ref ParentType);
                            ConformanceTable.Add(ParentTypeName, ParentType);
                        }

                    ConformanceTable.Seal();
                }
            }

            IClassType ClassType = new ClassType(baseClass, typeArgumentTable, instancingClassType, ConformanceTable);
            return ClassType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassType"/> class.
        /// </summary>
        /// <param name="baseClass">The class used to instanciate this type.</param>
        /// <param name="typeArgumentTable">Arguments if the class is generic.</param>
        /// <param name="instancingClassType">The class type if this instance is a derivation (such as renaming).</param>
        /// <param name="conformanceTable">The initialized conformance table.</param>
        private ClassType(IClass baseClass, ISealableDictionary<string, ICompiledType> typeArgumentTable, ICompiledTypeWithFeature instancingClassType, ISealableDictionary<ITypeName, ICompiledType> conformanceTable)
        {
            BaseClass = baseClass;
            TypeArgumentTable = typeArgumentTable;

            if (BaseClass.ResolvedClassType.IsAssigned)
            {
                if (conformanceTable.IsSealed)
                {
                    ConformanceTable.Merge(conformanceTable);
                    ConformanceTable.Seal();
                }
                else
                {
                    TypeInstancingRecord NewRecord = new TypeInstancingRecord();
                    NewRecord.InstancingClassType = instancingClassType;
                    NewRecord.ResolvedTypeName = BaseClass.ResolvedClassTypeName.Item;
                    NewRecord.ResolvedType = this;
                    BaseClass.ResolvedClassType.Item.InstancingRecordList.Add(NewRecord);
                }
            }
        }
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
                string Result;

                if (TypeArgumentTable.Count > 0)
                {
                    string ActualGenericList = string.Empty;
                    foreach (KeyValuePair<string, ICompiledType> Entry in TypeArgumentTable)
                    {
                        if (ActualGenericList.Length > 0)
                            ActualGenericList += ", ";

                        string FormalGeneric = Entry.Key;
                        ICompiledType ActualGeneric = Entry.Value;

                        if (ActualGeneric is IFormalGenericType AsFormalGenericType)
                            ActualGenericList += FormalGeneric;
                        else
                            ActualGenericList += $"{FormalGeneric}={ActualGeneric.TypeFriendlyName}";
                    }

                    Result = $"{BaseClass.ValidClassName}[{ActualGenericList}]";
                }
                else
                    Result = BaseClass.ValidClassName;

                return Result;
            }
        }

        /// <summary>
        /// True if the type is a reference type.
        /// </summary>
        public bool IsReference
        {
            get { return BaseClass.CopySpecification == BaseNode.CopySemantic.Reference; }
        }

        /// <summary>
        /// True if the type is a value type.
        /// </summary>
        public bool IsValue
        {
            get { return BaseClass.CopySpecification == BaseNode.CopySemantic.Value; }
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
            return BaseClass.TypeTable;
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Merge inheritance clauses.
        /// </summary>
        /// <param name="baseClass">The class with inheritance to merge.</param>
        /// <param name="resolvedClassType">The type from the class.</param>
        public static void MergeConformingParentTypes(IClass baseClass, IClassType resolvedClassType)
        {
            foreach (IInheritance InheritanceItem in baseClass.InheritanceList)
                if (InheritanceItem.Conformance == BaseNode.ConformanceType.Conformant)
                {
                    ITypeName ParentTypeName = InheritanceItem.ResolvedParentTypeName.Item;
                    ICompiledType ParentType = InheritanceItem.ResolvedParentType.Item;
                    resolvedClassType.ConformanceTable.Add(ParentTypeName, ParentType);
                }

            resolvedClassType.ConformanceTable.Seal();

            foreach (TypeInstancingRecord Record in resolvedClassType.InstancingRecordList)
            {
                ICompiledTypeWithFeature InstancingClassType = Record.InstancingClassType;
                ITypeName ResolvedTypeName = Record.ResolvedTypeName;
                ICompiledType ResolvedType = Record.ResolvedType;

                ResolvedType.InstanciateType(InstancingClassType, ref ResolvedTypeName, ref ResolvedType);
                Record.ResolvedTypeName = ResolvedTypeName;
                Record.ResolvedType = ResolvedType;

                if (!Record.ResolvedType.ConformanceTable.IsSealed)
                {
                    IClassType ResolvedInstancingClassType = (IClassType)Record.ResolvedType;

                    foreach (Inheritance InheritanceItem in baseClass.InheritanceList)
                        if (InheritanceItem.Conformance == BaseNode.ConformanceType.Conformant)
                        {
                            ITypeName ParentTypeName = InheritanceItem.ResolvedParentTypeName.Item;
                            ICompiledType ParentType = InheritanceItem.ResolvedParentType.Item;
                            ParentType.InstanciateType(ResolvedInstancingClassType, ref ParentTypeName, ref ParentType);

                            Record.ResolvedType.ConformanceTable.Add(ParentTypeName, ParentType);
                        }

                    Record.ResolvedType.ConformanceTable.Seal();
                }
            }
        }

        /// <summary>
        /// Creates an instance of a class type, or reuse an existing instance.
        /// </summary>
        /// <param name="instancingClassType">The class type to instanciate.</param>
        /// <param name="resolvedTypeName">The proposed type instance name.</param>
        /// <param name="resolvedType">The proposed type instance.</param>
        public void InstanciateType(ICompiledTypeWithFeature instancingClassType, ref ITypeName resolvedTypeName, ref ICompiledType resolvedType)
        {
            bool IsNewInstance = false;

            ISealableDictionary<string, ICompiledType> InstancedTypeArgumentTable = new SealableDictionary<string, ICompiledType>();
            foreach (KeyValuePair<string, ICompiledType> TypeArgument in TypeArgumentTable)
            {
                ITypeName InstancedTypeArgumentName = null;
                ICompiledType InstancedTypeArgument = TypeArgument.Value;
                InstancedTypeArgument.InstanciateType(instancingClassType, ref InstancedTypeArgumentName, ref InstancedTypeArgument);

                InstancedTypeArgumentTable.Add(TypeArgument.Key, InstancedTypeArgument);

                if (InstancedTypeArgument != TypeArgument.Value)
                    IsNewInstance = true;
            }

            if (IsNewInstance)
            {
                ISealableDictionary<ITypeName, ICompiledType> InstancingTypeTable = instancingClassType.GetTypeTable();
                ResolveType(InstancingTypeTable, BaseClass, InstancedTypeArgumentTable, instancingClassType, out resolvedTypeName, out resolvedType);
            }
        }
        #endregion

        #region Locate type
        /// <summary>
        /// Locates, or creates, a resolved class type.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="baseClass">The class this is from.</param>
        /// <param name="typeArgumentTable">The generic arguments used when creating the class type.</param>
        /// <param name="instancingClassType">The class type to instanciate.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void ResolveType(ISealableDictionary<ITypeName, ICompiledType> typeTable, IClass baseClass, ISealableDictionary<string, ICompiledType> typeArgumentTable, ICompiledTypeWithFeature instancingClassType, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            resolvedTypeName = null;
            resolvedType = null;

            if (!TypeTableContaining(typeTable, baseClass, typeArgumentTable, out resolvedTypeName, out resolvedType))
            {
                BuildType(baseClass, typeArgumentTable, instancingClassType, out resolvedTypeName, out resolvedType);
                typeTable.Add(resolvedTypeName, resolvedType);
            }
        }

        /// <summary>
        /// Checks if a matching class type exists in a type table.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="baseClass">The class this is from.</param>
        /// <param name="typeArgumentTable">The generic arguments used when creating the class type.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static bool TypeTableContaining(ISealableDictionary<ITypeName, ICompiledType> typeTable, IClass baseClass, ISealableDictionary<string, ICompiledType> typeArgumentTable, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            resolvedTypeName = null;
            resolvedType = null;
            bool Result = false;

            foreach (KeyValuePair<ITypeName, ICompiledType> Entry in typeTable)
                if (Entry.Value is IClassType AsClassType)
                    if (AsClassType.BaseClass == baseClass)
                    {
                        ISealableDictionary<string, ICompiledType> ResolvedTypeArgumentTable = AsClassType.TypeArgumentTable;
                        bool AllArgumentsEqual = true;

                        foreach (KeyValuePair<string, ICompiledType> TypeArgumentEntry in typeArgumentTable)
                        {
                            string GenericName = TypeArgumentEntry.Key;
                            ICompiledType TypeArgument = TypeArgumentEntry.Value;

                            Debug.Assert(ResolvedTypeArgumentTable.ContainsKey(GenericName));
                            ICompiledType ResolvedTypeArgument = ResolvedTypeArgumentTable[GenericName];

                            AllArgumentsEqual &= TypeArgument == ResolvedTypeArgument;
                        }

                        if (AllArgumentsEqual)
                        {
                            Debug.Assert(!Result);

                            resolvedTypeName = Entry.Key;
                            resolvedType = AsClassType;
                            Result = true;
                        }
                    }

            return Result;
        }

        /// <summary>
        /// Creates a class type with resolved arguments.
        /// </summary>
        /// <param name="baseClass">The class this is from.</param>
        /// <param name="typeArgumentTable">The generic arguments used when creating the class type.</param>
        /// <param name="instancingClassType">The class type to instanciate.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static void BuildType(IClass baseClass, ISealableDictionary<string, ICompiledType> typeArgumentTable, ICompiledTypeWithFeature instancingClassType, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            resolvedTypeName = null;
            resolvedType = null;

            IClassType ResolvedClassType = Create(baseClass, typeArgumentTable, instancingClassType);

#if COVERAGE
            string TypeString = ResolvedClassType.ToString();
#endif

            resolvedTypeName = new TypeName(ResolvedClassType.TypeFriendlyName);
            resolvedType = ResolvedClassType;

            if (baseClass.DiscreteTable.IsSealed)
            {
                ResolvedClassType.DiscreteTable.Merge(baseClass.DiscreteTable);
                ResolvedClassType.DiscreteTable.Seal();
            }

            if (baseClass.ExportTable.IsSealed)
            {
                ResolvedClassType.ExportTable.Merge(baseClass.ExportTable);
                ResolvedClassType.ExportTable.Seal();
            }

            if (baseClass.FeatureTable.IsSealed)
            {
                ResolvedClassType.FeatureTable.Merge(baseClass.FeatureTable);
                ResolvedClassType.FeatureTable.Seal();
            }

            if (baseClass.TypedefTable.IsSealed)
            {
                ResolvedClassType.TypedefTable.Merge(baseClass.TypedefTable);
                ResolvedClassType.TypedefTable.Seal();
            }

            baseClass.GenericInstanceList.Add(ResolvedClassType);
        }
        #endregion

        #region Compiler
        /// <summary>
        /// The source from which this type is issued.
        /// </summary>
        public IObjectType SourceType { get; }

        /// <summary>
        /// The class used to instanciate this type.
        /// </summary>
        public IClass BaseClass { get; }

        /// <summary>
        /// Arguments if the class is generic.
        /// </summary>
        public ISealableDictionary<string, ICompiledType> TypeArgumentTable { get; }

        /// <summary>
        /// Typedefs available in this type.
        /// </summary>
        public ISealableDictionary<IFeatureName, ITypedefType> TypedefTable { get; private set; } = new SealableDictionary<IFeatureName, ITypedefType>();

        /// <summary>
        /// True if an instance of the class is cloned at some point.
        /// </summary>
        public bool IsUsedInCloneOf { get; private set; }

        /// <summary>
        /// Compares two types.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        public static bool TypesHaveIdenticalSignature(IClassType type1, IClassType type2)
        {
            bool IsIdentical = true;

            IsIdentical &= type1.BaseClass == type2.BaseClass;
            IsIdentical &= type1.TypeArgumentTable.Count == type2.TypeArgumentTable.Count;

            IList<ICompiledType> TypeArgumentList1 = new List<ICompiledType>();
            IList<ICompiledType> TypeArgumentList2 = new List<ICompiledType>();

            foreach (KeyValuePair<string, ICompiledType> Entry in type1.TypeArgumentTable)
            {
                ICompiledType TypeArgument = Entry.Value;
                TypeArgumentList1.Add(TypeArgument);
            }

            foreach (KeyValuePair<string, ICompiledType> Entry in type2.TypeArgumentTable)
            {
                ICompiledType TypeArgument = Entry.Value;
                TypeArgumentList2.Add(TypeArgument);
            }

            for (int i = 0; i < TypeArgumentList1.Count && i < TypeArgumentList2.Count; i++)
                IsIdentical &= ObjectType.TypesHaveIdenticalSignature(TypeArgumentList1[i], TypeArgumentList2[i]);

            return IsIdentical;
        }

        /// <summary>
        /// Creates a clone of this type with renamed identifiers.
        /// </summary>
        /// <param name="renamedExportTable">The rename table for exports.</param>
        /// <param name="renamedTypedefTable">The rename table for typedefs.</param>
        /// <param name="renamedDiscreteTable">The rename table for discretes.</param>
        /// <param name="renamedFeatureTable">The rename table for features.</param>
        /// <param name="instancingClassType">The type that is requesting cloning.</param>
        public IClassType CloneWithRenames(ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> renamedExportTable, ISealableDictionary<IFeatureName, ITypedefType> renamedTypedefTable, ISealableDictionary<IFeatureName, IDiscrete> renamedDiscreteTable, ISealableDictionary<IFeatureName, IFeatureInstance> renamedFeatureTable, IClassType instancingClassType)
        {
            IClassType ClonedType = Create(BaseClass, TypeArgumentTable, instancingClassType);

            ClonedType.ExportTable.Merge(renamedExportTable);
            ClonedType.ExportTable.Seal();
            ClonedType.TypedefTable.Merge(renamedTypedefTable);
            ClonedType.TypedefTable.Seal();
            ClonedType.DiscreteTable.Merge(renamedDiscreteTable);
            ClonedType.DiscreteTable.Seal();
            ClonedType.FeatureTable.Merge(renamedFeatureTable);
            ClonedType.FeatureTable.Seal();

            return ClonedType;
        }

        /// <summary>
        /// Sets the <see cref="IsUsedInCloneOf"/> flag.
        /// </summary>
        public void MarkAsUsedInCloneOf()
        {
            IsUsedInCloneOf = true;
        }
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return $"Class Type '{BaseClass.EntityName.Text}'";
        }
        #endregion
    }
}
