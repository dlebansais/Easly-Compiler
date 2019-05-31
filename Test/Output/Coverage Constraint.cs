namespace Test
{
    public interface ICoverageConstraint<IT, T>
        where IT : ICoverageProcedureFeature
        where T : CoverageProcedureFeature, new(), IT
    {
    }

    [System.Serializable]
    public class CoverageConstraint<IT, T> : ICoverageConstraint<IT, T>
        where IT : ICoverageProcedureFeature
        where T : CoverageProcedureFeature, new(), IT
    {
        #region Init
        #endregion
    }
}
