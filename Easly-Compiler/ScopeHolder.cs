namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// Information about entities, where they are declared or used.
    /// </summary>
    public interface IScopeHolder
    {
        /*
        /// <summary>
        /// Entities local to a scope.
        /// </summary>
        IHashtableEx<string, IScopeAttributeFeature> LocalScope { get; }

        /// <summary>
        /// List of scopes containing the current instance.
        /// </summary>
        IList<IScopeHolder> InnerScopes { get; }

        /// <summary>
        /// All reachable entities.
        /// </summary>
        IHashtableEx<string, IScopeAttributeFeature> FullScope { get; }
        */
    }
}
