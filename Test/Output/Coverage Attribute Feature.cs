namespace Test
{
    public interface ICoverageAttributeFeature
    {
    }

    [System.Serializable]
    public class CoverageAttributeFeature : ICoverageAttributeFeature
    {
        #region Client Interface
        public String Test { get; private set; }
        public const Boolean Test2 = true;
        #endregion
    }
}
