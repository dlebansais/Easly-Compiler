namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler-only IFormalGenericType.
    /// </summary>
    public interface IFormalGenericType : ICompiledType
    {
        /// <summary>
        /// The generic from which this instance is issued.
        /// </summary>
        IGeneric FormalGeneric { get; }

        /// <summary>
        /// The associated unique type name.
        /// </summary>
        ITypeName ResolvedTypeName { get; }
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
                if (FormalGeneric.ResolvedConformanceTable.IsSealed)
                    foreach (KeyValuePair<ITypeName, ICompiledType> Entry in FormalGeneric.ResolvedConformanceTable)
                    {
                        ICompiledType ConformanceType = Entry.Value;
                        if (ConformanceType.IsReference)
                            return true;
                    }

                return false;
            }
        }

        /// <summary>
        /// True if the type is a value type.
        /// </summary>
        public bool IsValue
        {
            get
            {
                if (FormalGeneric.ResolvedConformanceTable.IsSealed)
                    foreach (KeyValuePair<ITypeName, ICompiledType> Entry in FormalGeneric.ResolvedConformanceTable)
                    {
                        ICompiledType ConformanceType = Entry.Value;
                        if (ConformanceType.IsValue)
                            return true;
                    }

                return false;
            }
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
            foreach (KeyValuePair<string, ICompiledType> TypeArgument in instancingClassType.TypeArgumentTable)
                if (TypeArgument.Key == TypeFriendlyName)
                {
                    resolvedType = TypeArgument.Value;
                    break;
                }

            return true;
        }
        #endregion

        #region Compiler
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
