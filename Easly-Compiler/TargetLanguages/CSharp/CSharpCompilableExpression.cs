namespace EaslyCompiler
{
    /// <summary>
    /// Interface for expressions that can be compiled separately to obtain a constant value.
    /// </summary>
    public interface ICSharpCompilableExpression
    {
        /// <summary>
        /// The expression compiled constant value.
        /// </summary>
        string CompiledValue { get; }
    }
}
