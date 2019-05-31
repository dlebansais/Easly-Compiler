namespace Test
{
    public interface ICoverageAny<IT, T> : IAny
        where IT : Any
        where T : Any, IT
    {
    }

    [System.Serializable]
    public class CoverageAny<IT, T> : ICoverageAny<IT, T>
        where IT : Any
        where T : Any, IT
    {
        #region Client Interface
        #endregion
    }
}
