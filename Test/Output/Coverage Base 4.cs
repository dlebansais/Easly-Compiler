namespace Test
{
    public interface ICoverageBase4 : ICoverageGeneric<IT, T>
    {
    }

    [System.Serializable]
    public class CoverageBase4 : CoverageGeneric<IT, T>, ICoverageBase4
    {
        #region Client Interface
        #endregion
    }
}
