namespace EaslyCompiler
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for types that have accessible features (class type and generic type with constraints).
    /// </summary>
    public interface ICSharpTypeWithFeature : ICSharpType
    {
        /// <summary>
        /// The list of class types this type conforms to.
        /// </summary>
        IList<ICSharpClassType> ConformingClassTypeList { get; }
    }
}
