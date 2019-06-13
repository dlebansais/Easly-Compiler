namespace EaslyCompiler
{
    using Easly;

    /// <summary>
    /// Interface for types that have accessible features (class type and generic type with constraints).
    /// </summary>
    public interface ICompiledTypeWithFeature : ICompiledType
    {
        /// <summary>
        /// Gets the type table for this type.
        /// </summary>
        ISealableDictionary<ITypeName, ICompiledType> GetTypeTable();
    }
}
