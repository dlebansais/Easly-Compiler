namespace Test
{
    public interface ICoverageInstancing2<IT, T, IG, G> : ICoverageInstancing1<IT, T>
    {
    }

    [System.Serializable]
    public class CoverageInstancing2<IT, T, IG, G> : CoverageInstancing1<IT, T>, ICoverageInstancing2<IT, T, IG, G>
    {
    }
}
