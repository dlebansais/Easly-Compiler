namespace Test
{
    public interface ICoverageExport
    {
    }

    [System.Serializable]
    public class CoverageExport : ICoverageExport
    {
        #region Client Interface
        public String Test { get; private set; }
        #endregion
    }
}
