namespace Test
{
    public interface ICoverageFunctionType2<IN, N, IT, T>
    {
    }

    [System.Serializable]
    public class CoverageFunctionType2<IN, N, IT, T> : ICoverageFunctionType2<IN, N, IT, T>
    {
        #region Client Interface
        public Hashtable<IN, IT> Test1 { get; private set; }
        public Hashtable<Number, IT> Test2 { get; private set; }
        #endregion
    }
}
