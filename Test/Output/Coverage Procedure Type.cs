namespace Test
{
    public interface ICoverageProcedureType : ICoverageGeneric<IT, T>
    {
    }

    [System.Serializable]
    public class CoverageProcedureType : CoverageGeneric<IT, T>, ICoverageProcedureType
    {
        #region Client Interface
        public ICoverageGeneric<<Not supported>, <Not supported>> Test2 { get; private set; }
        #endregion
    }
}
