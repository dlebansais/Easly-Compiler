namespace Test
{
    public interface ICoverageQualifiedName
    {
    }

    [System.Serializable]
    public class CoverageQualifiedName : ICoverageQualifiedName
    {
        #region Client Interface
        public ICoverageQualifiedName Test2 { get; private set; }
        #endregion
    }
}
