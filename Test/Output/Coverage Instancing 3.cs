namespace Test
{
    public interface ICoverageInstancing3
    {
    }

    [System.Serializable]
    public class CoverageInstancing3 : ICoverageInstancing3
    {
        #region Client Interface
        public ICoverageInstancing2<Number, Number, String, String> Test { get; private set; }
        #endregion
    }
}
