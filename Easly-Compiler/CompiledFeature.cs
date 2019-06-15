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
        /// Name of the agent type associated to the feature.
        /// </summary>
        OnceReference<ITypeName> ResolvedAgentTypeName { get; }

        /// <summary>
        /// The agent type associated to the feature.
        /// </summary>
        OnceReference<ICompiledType> ResolvedAgentType { get; }

        /// <summary>
        /// The name of the type to use, as source or destination, for the purpose of path searching, assignment and query.
        /// </summary>
        OnceReference<ITypeName> ResolvedEffectiveTypeName { get; }

        /// <summary>
        /// The type to use, as source or destination, for the purpose of path searching, assignment and query.
        /// </summary>
        OnceReference<ICompiledType> ResolvedEffectiveType { get; }
    }
}
