namespace EaslyCompiler
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for types that can be a number type.
    /// </summary>
    public interface ICompiledNumberType : ICompiledType
    {
        /// <summary>
        /// The number kind if the type is a number.
        /// </summary>
        NumberKinds NumberKind { get; }

        /// <summary>
        /// Gets the default number kind for this type.
        /// </summary>
        NumberKinds GetDefaultNumberKind();

        /// <summary>
        /// Tentatively updates the number kind if <paramref name="kind"/> is more accurate.
        /// </summary>
        /// <param name="kind">The new kind.</param>
        /// <param name="isChanged">True if the number kind was changed.</param>
        void UpdateNumberKind(NumberKinds kind, ref bool isChanged);

        /// <summary>
        /// Tentatively updates the number kind from another type if it is more accurate.
        /// </summary>
        /// <param name="type">The other type.</param>
        /// <param name="isChanged">True if the number kind was changed.</param>
        void UpdateNumberKind(ICompiledNumberType type, ref bool isChanged);

        /// <summary>
        /// Tentatively updates the number kind from a list of other types, if they are all more accurate.
        /// </summary>
        /// <param name="typeList">The list of types.</param>
        /// <param name="isChanged">True if the number kind was changed.</param>
        void UpdateNumberKind(IList<ICompiledNumberType> typeList, ref bool isChanged);
    }
}
