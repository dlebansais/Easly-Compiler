namespace Test
{
    public interface ICoverageInspectInstruction
    {
    }

    [System.Serializable]
    public class CoverageInspectInstruction : ICoverageInspectInstruction
    {
        #region Client Interface
        public CoverageEnumeration Test3 { get; private set; }
        public Number Test2 { get; private set; }
        #endregion
    }
}
