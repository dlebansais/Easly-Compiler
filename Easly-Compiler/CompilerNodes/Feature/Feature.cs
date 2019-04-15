namespace CompilerNode
{
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IFeature.
    /// </summary>
    public interface IFeature : BaseNode.IFeature, INode, ISource
    {
        /// <summary>
        /// The resolved feature name.
        /// </summary>
        OnceReference<IFeatureName> ValidFeatureName { get; }

        /// <summary>
        /// The resolved feature.
        /// </summary>
        OnceReference<ICompiledFeature> ResolvedFeature { get; }
    }
}
