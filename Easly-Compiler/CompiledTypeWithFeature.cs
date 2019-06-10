namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// Interface for types that have accessible features (class type and generic type with constraints).
    /// </summary>
    public interface ICompiledTypeWithFeature : ICompiledType
    {
        /// <summary>
        /// The list of class types this type conforms to.
        /// </summary>
        IList<IClassType> ConformingClassTypeList { get; }
    }
}
