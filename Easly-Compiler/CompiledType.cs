namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A type, from a <see cref="BaseNode.ObjectType"/> or specific to the compiler.
    /// </summary>
    public interface ICompiledType
    {
        /// <summary>
        /// Discretes available in this type.
        /// </summary>
        IHashtableEx<IFeatureName, IDiscrete> DiscreteTable { get; }

        /// <summary>
        /// Features available in this type.
        /// </summary>
        IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable { get; }

        /// <summary>
        /// Exports available in this type.
        /// </summary>
        IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> ExportTable { get; }

        /// <summary>
        /// Table of conforming types.
        /// </summary>
        IHashtableEx<ITypeName, ICompiledType> ConformanceTable { get; }

        /// <summary>
        /// List of type instancing.
        /// </summary>
        IList<TypeInstancingRecord> InstancingRecordList { get; }

        /// <summary>
        /// Type friendly name, unique.
        /// </summary>
        string TypeFriendlyName { get; }

        /// <summary>
        /// True if the type is a reference type.
        /// </summary>
        bool IsReference { get; }

        /// <summary>
        /// True if the type is a value type.
        /// </summary>
        bool IsValue { get; }

        /// <summary>
        /// The typedef this type comes from, if assigned.
        /// </summary>
        OnceReference<ITypedef> OriginatingTypedef { get; }

        /// <summary>
        /// Creates an instance of a class type, or reuse an existing instance.
        /// </summary>
        /// <param name="instancingClassType">The class type to instanciate.</param>
        /// <param name="resolvedTypeName">The proposed type instance name.</param>
        /// <param name="resolvedType">The proposed type instance.</param>
        void InstanciateType(IClassType instancingClassType, ref ITypeName resolvedTypeName, ref ICompiledType resolvedType);
    }
}
