namespace CompilerNode
{
    /// <summary>
    /// Compiler IFeature that can have a precursor.
    /// </summary>
    public interface IFeatureWithPrecursor
    {
        /// <summary>
        /// True if the feature is calling a precursor.
        /// </summary>
        bool IsCallingPrecursor { get; }

        /// <summary>
        /// Sets the <see cref="IsCallingPrecursor"/> property.
        /// </summary>
        void MarkAsCallingPrecursor();
    }
}
