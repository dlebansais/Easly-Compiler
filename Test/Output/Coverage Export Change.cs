namespace Test
{
    public interface ICoverageExportChange : ICoverageExport
    {
    }

    [System.Serializable]
    public class CoverageExportChange : CoverageExport, ICoverageExportChange
    {
    }
}
