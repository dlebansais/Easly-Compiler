namespace EaslyCompiler
{
    /// <summary>
    /// A C# feature with a name.
    /// </summary>
    public interface ICSharpFeatureWithName : ICSharpFeature
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        string Name { get; }
    }
}
