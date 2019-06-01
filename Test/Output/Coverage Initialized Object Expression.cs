namespace Test
{
    public interface ICoverageInitializedObjectExpression
    {
    }

    [System.Serializable]
    public class CoverageInitializedObjectExpression : ICoverageInitializedObjectExpression
    {
        #region Client Interface
        public String Test { get; private set; }
        public String Foo { get; private set; }
        public const ICoverageInitializedObjectExpression Test3 = new CoverageInitializedObjectExpression() { Test = "1" };
        #endregion
    }
}
