namespace Test
{
    public interface ICoverageBinaryConditionalExpression
    {
    }

    [System.Serializable]
    public class CoverageBinaryConditionalExpression : ICoverageBinaryConditionalExpression
    {
        #region Client Interface
        public const Number Test3 = true && false;
        public const Number Test4 = true || false;
        public const Boolean Test6 = ( == ) && false;
        #endregion
    }
}
