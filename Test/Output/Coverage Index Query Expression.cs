namespace Test
{
    public interface ICoverageIndexQueryExpression
    {
    }

    [System.Serializable]
    public class CoverageIndexQueryExpression : ICoverageIndexQueryExpression
    {
        #region Client Interface
        public ICoverageClassConstantExpression Test { get; private set; }
        #endregion
    }
}
