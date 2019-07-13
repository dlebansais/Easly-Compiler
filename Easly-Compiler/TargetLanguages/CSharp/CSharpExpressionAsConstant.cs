namespace EaslyCompiler
{
    /// <summary>
    /// Interface for expressions that can be used as the value of a constant feature.
    /// </summary>
    public interface ICSharpExpressionAsConstant
    {
        /// <summary>
        /// True if the expression can provide its constant value directly.
        /// </summary>
        bool IsDirectConstant { get; }
    }
}
