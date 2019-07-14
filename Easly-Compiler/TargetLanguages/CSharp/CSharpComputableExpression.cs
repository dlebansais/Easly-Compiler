namespace EaslyCompiler
{
    /// <summary>
    /// Interface for expressions that can be compiled separately to obtain a constant value.
    /// </summary>
    public interface ICSharpComputableExpression
    {
        /// <summary>
        /// The expression computed constant value.
        /// </summary>
        string ComputedValue { get; }

        /// <summary>
        /// Runs the compiler to compute the value as a string.
        /// </summary>
        void Compute();
    }
}
