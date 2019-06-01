namespace Test
{
    public interface ICoverageTupleType
    {
        <Not Supported> Test1 { get; set; }
    }

    [System.Serializable]
    public class CoverageTupleType : ICoverageTupleType
    {
        #region Properties
        public virtual <Not Supported> Test1 { get; set; }
        #endregion

        #region Client Interface
        public ICoverageGeneric<Number, Number> Test2 { get; private set; }
        public ICoverageGeneric<String, String> Test3 { get; private set; }
        #endregion
    }
}
