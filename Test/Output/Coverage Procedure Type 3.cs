namespace Test
{
    public interface ICoverageProcedureType3 : ICoverageProcedureType2<IT, T>
    {
    }

    [System.Serializable]
    public class CoverageProcedureType3 : CoverageProcedureType2<IT, T>, ICoverageProcedureType3
    {
    }
}
