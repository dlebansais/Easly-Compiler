namespace Test
{
    public interface ICoverageUnaryOperatorExpression
    {
    }

    [System.Serializable]
    public class CoverageUnaryOperatorExpression : ICoverageUnaryOperatorExpression
    {
        #region Client Interface
        public const ICoverageUnaryOperatorExpression Source = new CoverageUnaryOperatorExpression() {  };
        public const Number Test1 = - 1;
        public const Boolean Test2 = not true;
        #endregion
    }
}
