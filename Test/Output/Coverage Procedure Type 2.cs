namespace Test
{
    public interface ICoverageProcedureType2<IT, T>
    {
    }

    [System.Serializable]
    public class CoverageProcedureType2<IT, T> : ICoverageProcedureType2<IT, T>
    {
        #region Client Interface
        public ICoverageGeneric<<Not supported>, <Not supported>> Test1 { get; private set; }
        #endregion
    }
}
