namespace Test
{
    public interface ICoverageIndexerType3 : ICoverageIndexerType2<IT, T>
    {
    }

    [System.Serializable]
    public class CoverageIndexerType3 : CoverageIndexerType2<IT, T>, ICoverageIndexerType3
    {
    }
}
