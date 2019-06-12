namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler-only IFormalGenericType.
    /// </summary>
    public interface IFormalGenericType : ICompiledTypeWithFeature, IPathParticipatingType
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
        /// The type to use instead of this type for a source or destination type, for the purpose of path searching.
        /// </summary>
        public ICompiledType TypeAsDestinationOrSource { get { return this; } }

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

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return $"Formal Generic Type '{FormalGeneric.EntityName.Text}'";
        }
        #endregion
    }
}
