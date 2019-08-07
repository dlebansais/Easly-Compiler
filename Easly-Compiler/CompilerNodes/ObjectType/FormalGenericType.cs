namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler-only IFormalGenericType.
    /// </summary>
    public interface IFormalGenericType : ICompiledTypeWithFeature, ICompiledNumberType
    {
        /// <summary>
        /// The generic from which this instance is issued.
        /// </summary>
        IGeneric FormalGeneric { get; }

        /// <summary>
        /// The associated unique type name.
        /// </summary>
        ITypeName ResolvedTypeName { get; }

        /// <summary>
        /// True if the type is used to create at least one object.
        /// </summary>
        bool IsUsedToCreate { get; }

        /// <summary>
        /// Sets the <see cref="IsUsedToCreate"/> property.
        /// </summary>
        void SetIsUsedToCreate();
    }

    /// <summary>
    /// Compiler-only IFormalGenericType.
    /// </summary>
    public class FormalGenericType : IFormalGenericType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="FormalGenericType"/> class.
        /// </summary>
        /// <param name="formalGeneric">The generic from which this instance is issued.</param>
        /// <param name="resolvedTypeName">The associated unique type name.</param>
        public FormalGenericType(IGeneric formalGeneric, ITypeName resolvedTypeName)
        {
            FormalGeneric = formalGeneric;
            ResolvedTypeName = resolvedTypeName;
            DiscreteTable.Seal();
            NumberKind = NumberKinds.NotChecked;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The generic from which this instance is issued.
        /// </summary>
        public IGeneric FormalGeneric { get; }

        /// <summary>
        /// The associated unique type name.
        /// </summary>
        public ITypeName ResolvedTypeName { get; }
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
                IName EntityName = (IName)FormalGeneric.EntityName;
                return EntityName.ValidText.Item;
            }
        }

        /// <summary>
        /// True if the type is a reference type.
        /// </summary>
        public bool IsReference
        {
            get
            {
                bool Result = false;

                Debug.Assert(FormalGeneric.ResolvedConformanceTable.IsSealed);

                foreach (KeyValuePair<ITypeName, ICompiledType> Entry in FormalGeneric.ResolvedConformanceTable)
                {
                    ICompiledType ConformanceType = Entry.Value;
                    Result |= ConformanceType.IsReference;
                }

                return Result;
            }
        }

        /// <summary>
        /// True if the type is a value type.
        /// </summary>
        public bool IsValue
        {
            get
            {
                bool Result = false;

                Debug.Assert(FormalGeneric.ResolvedConformanceTable.IsSealed);

                foreach (KeyValuePair<ITypeName, ICompiledType> Entry in FormalGeneric.ResolvedConformanceTable)
                {
                    ICompiledType ConformanceType = Entry.Value;
                    Result |= ConformanceType.IsValue;
                }

                return Result;
            }
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
            return FormalGeneric.TypeTable;
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

            if (instancingClassType is IClassType AsClassType)
            {
                ISealableDictionary<string, ICompiledType> TypeArgumentTable = AsClassType.TypeArgumentTable;

                foreach (KeyValuePair<string, ICompiledType> TypeArgument in TypeArgumentTable)
                    if (TypeArgument.Key == TypeFriendlyName)
                    {
                        resolvedType = TypeArgument.Value;
                        break;
                    }
            }
        }
        #endregion

        #region Compiler
        /// <summary>
        /// True if the type is used to create at least one object.
        /// </summary>
        public bool IsUsedToCreate { get; private set; }

        /// <summary>
        /// Sets the <see cref="IsUsedToCreate"/> property.
        /// </summary>
        public void SetIsUsedToCreate()
        {
            IsUsedToCreate = true;
        }

        /// <summary>
        /// Compares two types.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        public static bool TypesHaveIdenticalSignature(IFormalGenericType type1, IFormalGenericType type2)
        {
            return type1.FormalGeneric == type2.FormalGeneric;
        }
        #endregion

        #region Numbers
        /// <summary>
        /// The number kind if the type is a number.
        /// </summary>
        public NumberKinds NumberKind { get; private set; }

        /// <summary>
        /// Gets the default number kind for this type.
        /// </summary>
        public NumberKinds GetDefaultNumberKind()
        {
            NumberKinds Result = NumberKinds.NotApplicable;

            foreach (IConstraint Constraint in FormalGeneric.ConstraintList)
            {
                Debug.Assert(Constraint.ResolvedParentType.IsAssigned);

                if (Constraint.ResolvedParentType.Item is IClassType AsClassType)
                {
                    NumberKinds ClassKind = AsClassType.GetDefaultNumberKind();
                    Result = DowngradedKind(NumberKind, ClassKind);
                }
            }

            if (Result == NumberKinds.NotApplicable)
            {
                foreach (IConstraint Constraint in FormalGeneric.ConstraintList)
                {
                    Debug.Assert(Constraint.ResolvedParentType.IsAssigned);

                    if (Constraint.ResolvedParentType.Item is ICompiledNumberType AsNumberType)
                        Result = DowngradedKind(NumberKind, AsNumberType.NumberKind);
                }
            }

            return Result;
        }

        private NumberKinds DowngradedKind(NumberKinds oldKind, NumberKinds newKind)
        {
            NumberKinds Result;

            if (oldKind == NumberKinds.Integer && (newKind == NumberKinds.Real || newKind == NumberKinds.Unknown))
                Result = newKind;
            else if (oldKind == NumberKinds.Real && newKind == NumberKinds.Unknown)
                Result = newKind;
            else if (oldKind == NumberKinds.NotApplicable && newKind == NumberKinds.Unknown)
                Result = newKind;
            else
                Result = oldKind;

            return Result;
        }

        /// <summary>
        /// Tentatively updates the number kind if <paramref name="numberKind"/> is more accurate.
        /// </summary>
        /// <param name="numberKind">The new kind.</param>
        /// <param name="isChanged">True if the number kind was changed.</param>
        public void UpdateNumberKind(NumberKinds numberKind, ref bool isChanged)
        {
            if (NumberKind == NumberKinds.NotApplicable && numberKind != NumberKinds.NotApplicable)
            {
                NumberKind = numberKind;
                isChanged = true;
            }
            else if (NumberKind == NumberKinds.Unknown && numberKind == NumberKinds.Integer)
            {
                NumberKind = numberKind;
                isChanged = true;
            }
        }

        /// <summary>
        /// Tentatively updates the number kind from another type if it is more accurate.
        /// </summary>
        /// <param name="type">The other type.</param>
        /// <param name="isChanged">True if the number kind was changed.</param>
        public void UpdateNumberKind(ICompiledNumberType type, ref bool isChanged)
        {
            UpdateNumberKind(type.NumberKind, ref isChanged);
        }

        /// <summary>
        /// Tentatively updates the number kind from a list of other types, if they are all more accurate.
        /// </summary>
        /// <param name="typeList">The list of types.</param>
        /// <param name="isChanged">True if the number kind was changed.</param>
        public void UpdateNumberKind(IList<ICompiledNumberType> typeList, ref bool isChanged)
        {
            if (NumberKind != NumberKinds.NotApplicable || typeList.Count == 0)
                return;

            NumberKinds ComposedNumberKind = typeList[0].NumberKind;

            for (int i = 1; i < typeList.Count; i++)
            {
                NumberKinds ItemNumberKind = typeList[i].NumberKind;

                if (ItemNumberKind == NumberKinds.NotApplicable)
                    ComposedNumberKind = NumberKinds.NotApplicable;
                else if (ComposedNumberKind == NumberKinds.Integer && (ItemNumberKind == NumberKinds.Unknown || ItemNumberKind == NumberKinds.Real))
                    ComposedNumberKind = ItemNumberKind;
                else if (ComposedNumberKind == NumberKinds.Real && ItemNumberKind == NumberKinds.Unknown)
                    ComposedNumberKind = ItemNumberKind;
            }

            if (ComposedNumberKind != NumberKinds.NotApplicable)
            {
                NumberKind = ComposedNumberKind;
                isChanged = true;
            }
        }
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return $"Formal Generic Type '{FormalGeneric.EntityName.Text}'";
        }
        #endregion
    }
}
