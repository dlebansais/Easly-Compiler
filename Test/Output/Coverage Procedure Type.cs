namespace Test
{
    public interface ICoverageProcedureType : ICoverageGeneric<IT, T>
    {
    }

    [System.Serializable]
    public class CoverageProcedureType : CoverageGeneric<IT, T>, ICoverageProcedureType
    {
        #region Client Interface
        #endregion
    }
}
