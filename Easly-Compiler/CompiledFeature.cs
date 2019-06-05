namespace EaslyCompiler
{
    using System;
    using Easly;

    /// <summary>
    /// A feature, from a <see cref="BaseNode.Feature"/> or specific to the compiler.
    /// </summary>
    public interface ICompiledFeature
    {
        /// <summary>
        /// Indicates if the feature is deferred in another class.
        /// </summary>
        bool IsDeferredFeature { get; }

        /// <summary>
        /// True if the feature contains extern bodies in its overloads.
        /// </summary>
        bool HasExternBody { get; }

        /// <summary>
        /// True if the feature contains precursor bodies in its overloads.
        /// </summary>
        bool HasPrecursorBody { get; }

        /// <summary>
        /// Name of the associated type.
        /// </summary>
        OnceReference<ITypeName> ResolvedFeatureTypeName { get; }

        /// <summary>
        /// Associated type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedFeatureType { get; }
    }
}
