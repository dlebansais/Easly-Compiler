namespace Test
{
    public interface ICoverageUnaryNotExpression
    {
    }

    [System.Serializable]
    public class CoverageUnaryNotExpression : ICoverageUnaryNotExpression
    {
        #region Client Interface
        public const Boolean Test5 = true;
        public const Boolean Test6 = !Test5;
        public const Boolean Test7 = !(true xor true);
        #endregion
    }
}
