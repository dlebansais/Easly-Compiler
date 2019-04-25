namespace CompilerNode
{
    /// <summary>
    /// Compiler IFeature for all but indexers.
    /// </summary>
    public interface IFeatureWithName
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        BaseNode.IName EntityName { get; }
    }
}
