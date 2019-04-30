namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler-only IClassType.
    /// </summary>
    public interface IClassType : BaseNode.IShareableType, ICompiledType
    {
        /// <summary>
        /// The class used to instanciate this type.
        /// </summary>
        IClass BaseClass { get; }

        /// <summary>
        /// Arguments if the class is generic.
        /// </summary>
        IHashtableEx<string, ICompiledType> TypeArgumentTable { get; }

        /// <summary>
        /// Typedefs available in this type.
        /// </summary>
       IHashtableEx<IFeatureName, ITypedefType> TypedefTable { get; }

        /// <summary>
        /// Creates a clone of this type with renamed identifiers.
        /// </summary>
        /// <param name="renamedExportTable">The rename table for exports.</param>
        /// <param name="renamedTypedefTable">The rename table for typedefs.</param>
        /// <param name="renamedDiscreteTable">The rename table for discretes.</param>
        /// <param name="renamedFeatureTable">The rename table for features.</param>
        /// <param name="instancingClassType">The type that is requesting cloning.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="clonedType">The cloned type, if successful.</param>
        bool CloneWithRenames(IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> renamedExportTable, IHashtableEx<IFeatureName, ITypedefType> renamedTypedefTable, IHashtableEx<IFeatureName, IDiscrete> renamedDiscreteTable, IHashtableEx<IFeatureName, IFeatureInstance> renamedFeatureTable, IClassType instancingClassType, IList<IError> errorList, out IClassType clonedType);
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
            IClassType Result = new ClassType(classAny, new HashtableEx<string, ICompiledType>(), null, null);
            classAny.ResolvedClassTypeName.Item = new TypeName(classAny.ValidClassName);
            classAny.ResolvedClassType.Item = Result;
            classAny.ResolvedAsCompiledType.Item = Result;

            Debug.Assert(classAny.FeatureTable.IsSealed);
            Debug.Assert(classAny.FeatureTable.Count == 0);
            Debug.Assert(classAny.DiscreteTable.IsSealed);
            Debug.Assert(classAny.DiscreteTable.Count == 0);

            Result.FeatureTable.Seal();
            Result.DiscreteTable.Seal();

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
        /// <param name="baseClass">The class used to instanciate this type.</param>
        /// <param name="typeArgumentTable">Arguments if the class is generic.</param>
        public ClassType(IClass baseClass, IHashtableEx<string, ICompiledType> typeArgumentTable)
        {
            Debug.Assert(!baseClass.ResolvedClassType.IsAssigned);

            BaseClass = baseClass;
            TypeArgumentTable = typeArgumentTable;
        }

        /// <summary>
        /// Creates a <see cref="ClassType"/>.
        /// </summary>
        /// <param name="baseClass">The class used to instanciate this type.</param>
        /// <param name="typeArgumentTable">Arguments if the class is generic.</param>
        /// <param name="instancingClassType">The class type if this instance is a derivation (such as renaming).</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="classType">The class type, if successful.</param>
        public static bool Create(IClass baseClass, IHashtableEx<string, ICompiledType> typeArgumentTable, IClassType instancingClassType, IList<IError> errorList, out IClassType classType)
        {
            classType = null;

            IHashtableEx<ITypeName, ICompiledType> ConformanceTable = new HashtableEx<ITypeName, ICompiledType>();
            bool Success = true;

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
                            Success &= ParentType.InstanciateType(instancingClassType, ref ParentTypeName, ref ParentType, errorList);
                            ConformanceTable.Add(ParentTypeName, ParentType);
                        }

                    ConformanceTable.Seal();
                }
            }

            if (Success)
                classType = new ClassType(baseClass, typeArgumentTable, instancingClassType, ConformanceTable);

            return Success;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassType"/> class.
        /// </summary>
        /// <param name="baseClass">The class used to instanciate this type.</param>
        /// <param name="typeArgumentTable">Arguments if the class is generic.</param>
        /// <param name="instancingClassType">The class type if this instance is a derivation (such as renaming).</param>
        /// <param name="conformanceTable">The initialized conformance table.</param>
        private ClassType(IClass baseClass, IHashtableEx<string, ICompiledType> typeArgumentTable, IClassType instancingClassType, IHashtableEx<ITypeName, ICompiledType> conformanceTable)
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

        #region Properties
        /// <summary>
        /// The class used to instanciate this type.
        /// </summary>
        public IClass BaseClass { get; }

        /// <summary>
        /// Arguments if the class is generic.
        /// </summary>
        public IHashtableEx<string, ICompiledType> TypeArgumentTable { get; }

        /// <summary>
        /// Typedefs available in this type.
        /// </summary>
        public IHashtableEx<IFeatureName, ITypedefType> TypedefTable { get; private set; } = new HashtableEx<IFeatureName, ITypedefType>();

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
        #endregion

        #region Client Interface
        /// <summary>
        /// Merge inheritance clauses.
        /// </summary>
        /// <param name="baseClass">The class with inheritance to merge.</param>
        /// <param name="resolvedClassType">The type from the class.</param>
        /// <param name="errorList">The list of errors found.</param>
        public static bool MergeConformingParentTypes(IClass baseClass, IClassType resolvedClassType, IList<IError> errorList)
        {
            bool Success = true;

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
                IClassType InstancingClassType = Record.InstancingClassType;
                ITypeName ResolvedTypeName = Record.ResolvedTypeName;
                ICompiledType ResolvedType = Record.ResolvedType;

                Success &= ResolvedType.InstanciateType(InstancingClassType, ref ResolvedTypeName, ref ResolvedType, errorList);
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
                            Success &= ParentType.InstanciateType(ResolvedInstancingClassType, ref ParentTypeName, ref ParentType, errorList);

                            Record.ResolvedType.ConformanceTable.Add(ParentTypeName, ParentType);
                        }

                    Record.ResolvedType.ConformanceTable.Seal();
                }
            }

            return Success;
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

            IHashtableEx<string, ICompiledType> InstancedTypeArgumentTable = new HashtableEx<string, ICompiledType>();
            foreach (KeyValuePair<string, ICompiledType> TypeArgument in TypeArgumentTable)
            {
                ITypeName InstancedTypeArgumentName = null;
                ICompiledType InstancedTypeArgument = TypeArgument.Value;
                Success &= InstancedTypeArgument.InstanciateType(instancingClassType, ref InstancedTypeArgumentName, ref InstancedTypeArgument, errorList);

                InstancedTypeArgumentTable.Add(TypeArgument.Key, InstancedTypeArgument);

                if (InstancedTypeArgument != TypeArgument.Value)
                    IsNewInstance = true;
            }

            if (IsNewInstance)
                Success &= ResolveType(instancingClassType.BaseClass.TypeTable, BaseClass, InstancedTypeArgumentTable, instancingClassType, errorList, out resolvedTypeName, out resolvedType);

            return Success;
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
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static bool ResolveType(IHashtableEx<ITypeName, ICompiledType> typeTable, IClass baseClass, IHashtableEx<string, ICompiledType> typeArgumentTable, IClassType instancingClassType, IList<IError> errorList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            resolvedTypeName = null;
            resolvedType = null;
            bool Success = true;

            if (!TypeTableContaining(typeTable, baseClass, typeArgumentTable, out resolvedTypeName, out resolvedType))
            {
                Success &= BuildType(baseClass, typeArgumentTable, instancingClassType, errorList, out resolvedTypeName, out resolvedType);
                if (Success)
                    typeTable.Add(resolvedTypeName, resolvedType);
            }

            return Success;
        }

        /// <summary>
        /// Checks if a matching class type exists in a type table.
        /// </summary>
        /// <param name="typeTable">The table of existing types.</param>
        /// <param name="baseClass">The class this is from.</param>
        /// <param name="typeArgumentTable">The generic arguments used when creating the class type.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static bool TypeTableContaining(IHashtableEx<ITypeName, ICompiledType> typeTable, IClass baseClass, IHashtableEx<string, ICompiledType> typeArgumentTable, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            resolvedTypeName = null;
            resolvedType = null;
            bool Result = false;

            foreach (KeyValuePair<ITypeName, ICompiledType> Entry in typeTable)
                if (Entry.Value is IClassType AsClassType)
                    if (AsClassType.BaseClass == baseClass)
                    {
                        IHashtableEx<string, ICompiledType> ResolvedTypeArgumentTable = AsClassType.TypeArgumentTable;
                        bool AllArgumentsEqual = true;

                        foreach (KeyValuePair<string, ICompiledType> TypeArgumentEntry in typeArgumentTable)
                        {
                            string GenericName = TypeArgumentEntry.Key;
                            ICompiledType TypeArgument = TypeArgumentEntry.Value;
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
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedTypeName">The type name upon return.</param>
        /// <param name="resolvedType">The type upon return.</param>
        public static bool BuildType(IClass baseClass, IHashtableEx<string, ICompiledType> typeArgumentTable, IClassType instancingClassType, IList<IError> errorList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            resolvedTypeName = null;
            resolvedType = null;

            if (!Create(baseClass, typeArgumentTable, instancingClassType, errorList, out IClassType ResolvedClassType))
                return false;

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
            return true;
        }
        #endregion

        #region Compiler
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
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="clonedType">The cloned type, if successful.</param>
        public bool CloneWithRenames(IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> renamedExportTable, IHashtableEx<IFeatureName, ITypedefType> renamedTypedefTable, IHashtableEx<IFeatureName, IDiscrete> renamedDiscreteTable, IHashtableEx<IFeatureName, IFeatureInstance> renamedFeatureTable, IClassType instancingClassType, IList<IError> errorList, out IClassType clonedType)
        {
            bool Success = false;

            if (Create(BaseClass, TypeArgumentTable, instancingClassType, errorList, out clonedType))
            {
                clonedType.ExportTable.Merge(renamedExportTable);
                clonedType.ExportTable.Seal();
                clonedType.TypedefTable.Merge(renamedTypedefTable);
                clonedType.TypedefTable.Seal();
                clonedType.DiscreteTable.Merge(renamedDiscreteTable);
                clonedType.DiscreteTable.Seal();
                clonedType.FeatureTable.Merge(renamedFeatureTable);
                clonedType.FeatureTable.Seal();
                Success = true;
            }

            return Success;
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
