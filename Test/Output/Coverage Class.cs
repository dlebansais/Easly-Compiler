namespace Test
{
    public interface ICoverageClass<IT, T> : ICoverageBase1, ICoverageBase2
    {
    }

    [System.Serializable]
    public class CoverageClass<IT, T> : CoverageBase1, ICoverageClass<IT, T>
    {
        #region Client Interface
        public Boolean TestAssert { get; private set; }
        #endregion
    }
}
