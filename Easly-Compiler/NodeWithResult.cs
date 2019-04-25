namespace EaslyCompiler
{
    using Easly;

    /// <summary>
    /// Information about nodes that can have a result.
    /// </summary>
    public interface INodeWithResult : ISource
    {
        /// <summary>
        /// The name of the resolved result type.
        /// </summary>
        OnceReference<ITypeName> ResolvedResultTypeName { get; }

        /// <summary>
        /// The resolved result type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedResultType { get; }
    }
}
