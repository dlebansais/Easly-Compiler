namespace Test
{
    public interface ICoverageContinuation
    {
    }

    [System.Serializable]
    public class CoverageContinuation : ICoverageContinuation
    {
        #region Client Interface
        public Boolean Test2 { get; private set; }
        #endregion
    }
}
