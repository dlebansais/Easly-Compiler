namespace Test
{
    public interface ICoverageCheckInstruction
    {
    }

    [System.Serializable]
    public class CoverageCheckInstruction : ICoverageCheckInstruction
    {
        #region Client Interface
        public Boolean Test2 { get; private set; }
        #endregion
    }
}
