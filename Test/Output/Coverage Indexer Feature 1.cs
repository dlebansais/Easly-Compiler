namespace Test
{
    public interface ICoverageIndexerFeature1
    {
        String this[Number N] { get; }
    }

    [System.Serializable]
    public class CoverageIndexerFeature1 : ICoverageIndexerFeature1
    {
        #region Client Interface
        public Number Test { get; private set; }
        public String Indexer { get; private set; }
        #endregion
    }
}
