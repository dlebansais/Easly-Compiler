namespace EaslyCompiler
{
    /// <summary>
    /// The mode to use to write a C# scope.
    /// </summary>
    public enum CSharpCurlyBracketsInsertions
    {
        /// <summary>
        /// The callee can handle it as they please.
        /// </summary>
        Indifferent,

        /// <summary>
        /// The callee must insert them.
        /// </summary>
        Mandatory,

        /// <summary>
        /// The callee should not insert them.
        /// </summary>
        AlreadyInserted,
    }
}
