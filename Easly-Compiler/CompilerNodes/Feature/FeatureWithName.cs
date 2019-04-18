namespace CompilerNode
{
    using EaslyCompiler;

    /// <summary>
    /// Compiler IFeature for all but indexers.
    /// </summary>
    public interface IFeatureWithName : IFeature
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        BaseNode.IName EntityName { get; }
    }
}
