namespace Test
{
    public interface ICoverageBinaryOperatorExpression
    {
    }

    public abstract class CoverageBinaryOperatorExpression : ICoverageBinaryOperatorExpression
    {
        #region Client Interface
        public const Number Test3 = 0 + 1;
        public const Boolean Test4 = true xor true;
        #endregion
    }
}
