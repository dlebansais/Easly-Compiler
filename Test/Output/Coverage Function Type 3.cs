namespace Test
{
    public interface ICoverageFunctionType3<IN, N> : ICoverageFunctionType2<IN, N, IT, T>
    {
    }

    [System.Serializable]
    public class CoverageFunctionType3<IN, N> : CoverageFunctionType2<IN, N, IT, T>, ICoverageFunctionType3<IN, N>
    {
    }
}
