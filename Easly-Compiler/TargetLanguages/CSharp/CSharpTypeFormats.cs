namespace EaslyCompiler
{
    /// <summary>
    /// The mode to use to name a C# type.
    /// </summary>
    public enum CSharpTypeFormats
    {
        /// <summary>
        /// Use the type name.
        /// </summary>
        Normal,

        /// <summary>
        /// Add a 'I' to the type name.
        /// </summary>
        AsInterface,

        /// <summary>
        /// The type is a singleton.
        /// </summary>
        AsSingleton
    }
}
