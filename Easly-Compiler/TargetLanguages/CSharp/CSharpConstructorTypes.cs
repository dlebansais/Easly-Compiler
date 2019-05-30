namespace EaslyCompiler
{
    /// <summary>
    /// How many constructors in the C# class.
    /// </summary>
    public enum CSharpConstructorTypes
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1602 // Enumeration items must be documented
        NoConstructor,
        OneConstructor,
        ManyConstructors
#pragma warning restore SA1602 // Enumeration items must be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
