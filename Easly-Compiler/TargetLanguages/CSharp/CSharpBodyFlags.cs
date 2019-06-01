namespace EaslyCompiler
{
    /// <summary>
    /// The mode to use to write a C# body.
    /// </summary>
    public enum CSharpBodyFlags
    {
        /// <summary>
        /// Normal.
        /// </summary>
        None = 0,

        /// <summary>
        /// Add a { and } around the body.
        /// </summary>
        MandatoryCurlyBrackets = 0x0001,

        /// <summary>
        /// Include the result.
        /// </summary>
        HasResult = 0x0002,

        /// <summary>
        /// Use the 'value' keyword.
        /// </summary>
        HasValue = 0x0004
    }
}
