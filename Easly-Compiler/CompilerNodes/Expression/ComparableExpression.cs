namespace EaslyCompiler
{
    /// <summary>
    /// Expression that can be compared with others.
    /// </summary>
    public interface IComparableExpression
    {
        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        bool IsExpressionEqual(IComparableExpression other);
    }
}
