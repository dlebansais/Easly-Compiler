namespace Test
{
    public interface ICoverageTupleType3 : ICoverageTupleType2<IT, T>
    {
    }

    [System.Serializable]
    public class CoverageTupleType3 : CoverageTupleType2<IT, T>, ICoverageTupleType3
    {
        #region Properties
        #endregion

        #region Client Interface
        #endregion
    }
}
