namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// Information about entities, where they are declared or used.
    /// </summary>
    public interface IGetterSetterScopeHolder : IScopeHolder
    {
        /// <summary>
        /// Entities local to a scope, getter only.
        /// </summary>
        ISealableDictionary<string, IScopeAttributeFeature> LocalGetScope { get; }

        /// <summary>
        /// Entities local to a scope, setter only.
        /// </summary>
        ISealableDictionary<string, IScopeAttributeFeature> LocalSetScope { get; }

        /// <summary>
        /// List of scopes containing the current instance, getter only.
        /// </summary>
        IList<IScopeHolder> InnerGetScopes { get; }

        /// <summary>
        /// List of scopes containing the current instance, setter only.
        /// </summary>
        IList<IScopeHolder> InnerSetScopes { get; }

        /// <summary>
        /// All reachable entities, getter only.
        /// </summary>
        ISealableDictionary<string, IScopeAttributeFeature> FullGetScope { get; }

        /// <summary>
        /// All reachable entities, setter only.
        /// </summary>
        ISealableDictionary<string, IScopeAttributeFeature> FullSetScope { get; }
    }
}
