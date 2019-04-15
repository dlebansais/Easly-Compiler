﻿namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler-only IClassType.
    /// </summary>
    public interface IClassType : BaseNode.IShareableType, ISource, ICompiledType
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
        /// <param name="instancingClassType">The class type if this instance is a derivation (such as renaming).</param>
        /// <param name="errorList">The list of errors found.</param>
        public ClassType(IClass baseClass, IHashtableEx<string, ICompiledType> typeArgumentTable, IClassType instancingClassType, IList<IError> errorList)
        {
            BaseClass = baseClass;
            TypeArgumentTable = typeArgumentTable;

            if (BaseClass.ResolvedClassType.IsAssigned)
            {
                IClassType ResolvedClassType = BaseClass.ResolvedClassType.Item;

                if (ResolvedClassType.ConformanceTable.IsSealed)
                {
                    foreach (IInheritance InheritanceItem in BaseClass.InheritanceList)
                        if (InheritanceItem.Conformance == BaseNode.ConformanceType.Conformant)
                        {
                            ITypeName ParentTypeName = InheritanceItem.ResolvedParentTypeName.Item;
                            ICompiledType ParentType = InheritanceItem.ResolvedParentType.Item;
                            ParentType.InstanciateType(instancingClassType, ref ParentTypeName, ref ParentType, errorList);
                            ConformanceTable.Add(ParentTypeName, ParentType);
                        }

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
                TypedefTable = new HashtableEx<IFeatureName, ITypedefType>();
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

        #region Client Interface
        /// <summary>
        /// Merge inheritance clauses.
        /// </summary>
        /// <param name="baseClass">The class with inheritance to merge.</param>
        /// <param name="resolvedClassType">The type from the class.</param>
        /// <param name="errorList">The list of errors found.</param>
        public static void MergeConformingParentTypes(IClass baseClass, IClassType resolvedClassType, IList<IError> errorList)
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
                IClassType InstancingClassType = Record.InstancingClassType;
                ITypeName ResolvedTypeName = Record.ResolvedTypeName;
                ICompiledType ResolvedType = Record.ResolvedType;

                ResolvedType.InstanciateType(InstancingClassType, ref ResolvedTypeName, ref ResolvedType, errorList);
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
                            ParentType.InstanciateType(ResolvedInstancingClassType, ref ParentTypeName, ref ParentType, errorList);

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
        /// <param name="errorList">The list of errors found.</param>
        public void InstanciateType(IClassType instancingClassType, ref ITypeName resolvedTypeName, ref ICompiledType resolvedType, IList<IError> errorList)
        {
            bool IsNewInstance = false;

            IHashtableEx<string, ICompiledType> InstancedTypeArgumentTable = new HashtableEx<string, ICompiledType>();
            foreach (KeyValuePair<string, ICompiledType> TypeArgument in TypeArgumentTable)
            {
                ITypeName InstancedTypeArgumentName = null;
                ICompiledType InstancedTypeArgument = TypeArgument.Value;
                InstancedTypeArgument.InstanciateType(instancingClassType, ref InstancedTypeArgumentName, ref InstancedTypeArgument, errorList);

                InstancedTypeArgumentTable.Add(TypeArgument.Key, InstancedTypeArgument);

                if (InstancedTypeArgument != TypeArgument.Value)
                    IsNewInstance = true;
            }

            if (IsNewInstance)
                ResolveType(instancingClassType.BaseClass.TypeTable, BaseClass, InstancedTypeArgumentTable, instancingClassType, errorList, out resolvedTypeName, out resolvedType);
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
        public static void ResolveType(IHashtableEx<ITypeName, ICompiledType> typeTable, IClass baseClass, IHashtableEx<string, ICompiledType> typeArgumentTable, IClassType instancingClassType, IList<IError> errorList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            if (!TypeTableContaining(typeTable, baseClass, typeArgumentTable, out resolvedTypeName, out resolvedType))
            {
                BuildType(baseClass, typeArgumentTable, instancingClassType, errorList, out resolvedTypeName, out resolvedType);
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

                            if (TypeArgument != ResolvedTypeArgument)
                            {
                                AllArgumentsEqual = false;
                                break;
                            }
                        }

                        if (AllArgumentsEqual)
                        {
                            resolvedTypeName = Entry.Key;
                            resolvedType = AsClassType;
                            Result = true;
                            break;
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
        public static void BuildType(IClass baseClass, IHashtableEx<string, ICompiledType> typeArgumentTable, IClassType instancingClassType, IList<IError> errorList, out ITypeName resolvedTypeName, out ICompiledType resolvedType)
        {
            IClassType ResolvedClassType = new ClassType(baseClass, typeArgumentTable, instancingClassType, errorList);
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
        /// Compares two types.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        public static bool TypesHaveIdenticalSignature(IClassType type1, IClassType type2)
        {
            if (type1.BaseClass != type2.BaseClass)
                return false;

            if (type1.TypeArgumentTable.Count != type2.TypeArgumentTable.Count)
                return false;

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
                if (!ObjectType.TypesHaveIdenticalSignature(TypeArgumentList1[i], TypeArgumentList2[i]))
                    return false;

            return true;
        }
        #endregion
    }
}