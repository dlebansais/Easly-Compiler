namespace Test
{
    public interface ICoverageInheritance : ICoverageProcedureFeature
    {
    }

    [System.Serializable]
    public class CoverageInheritance : CoverageProcedureFeature, ICoverageInheritance
    {
    }
}
