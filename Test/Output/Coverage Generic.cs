namespace Test
{
    public interface ICoverageGeneric<IT, T>
    {
    }

    [System.Serializable]
    public class CoverageGeneric<IT, T> : ICoverageGeneric<IT, T>
    {
        #region Client Interface
        public <Not supported> Test { get; private set; }
        #endregion
    }
}
