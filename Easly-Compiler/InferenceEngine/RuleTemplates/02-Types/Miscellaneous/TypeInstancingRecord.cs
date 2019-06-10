namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// Link between class types and their instancing.
    /// </summary>
    public class TypeInstancingRecord
    {
        /// <summary>
        /// The class type.
        /// </summary>
        public ICompiledTypeWithFeature InstancingClassType { get; set; }

        /// <summary>
        /// The name of the instance.
        /// </summary>
        public ITypeName ResolvedTypeName { get; set; }

        /// <summary>
        /// The type instance.
        /// </summary>
        public ICompiledType ResolvedType { get; set; }
    }
}
