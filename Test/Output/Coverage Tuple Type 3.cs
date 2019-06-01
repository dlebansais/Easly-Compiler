namespace Test
{
    public interface ICoverageTupleType3 : ICoverageTupleType2<IT, T>
    {
    }

    [System.Serializable]
    public class CoverageTupleType3 : CoverageTupleType2<IT, T>, ICoverageTupleType3
    {
        #region Properties
        public override <Not Supported> Test { get; set; }
        #endregion

        #region Client Interface
        public <Not Supported> Test1 { get; private set; }
        #endregion
    }
}
