namespace EaslyCompiler
{
    /// <summary>
    /// One of the export keywords in C#
    /// </summary>
    public enum CSharpExports
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1602 // Enumeration items must be documented
        None, // For interfaces
        Private,
        Protected,
        Public
#pragma warning restore SA1602 // Enumeration items must be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
