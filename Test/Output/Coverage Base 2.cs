namespace Test
{
    public interface ICoverageBase2 : ICoverageBase1
    {
    }

    [System.Serializable]
    public class CoverageBase2 : CoverageBase1, ICoverageBase2
    {
    }
}
