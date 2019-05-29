namespace EaslyCompiler
{
    /// <summary>
    /// The mode to use to name a C# namespace.
    /// </summary>
    public enum CSharpNamespaceFormats
    {
        /// <summary>
        /// Ignore the namespace.
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Only one word.
        /// </summary>
        OneWord = 0x0001,

        /// <summary>
        /// The full namespace path.
        /// </summary>
        FullNamespace = 0x0002,
    }
}
