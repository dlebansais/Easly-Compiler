namespace Test
{
    public interface ICoverageBase
    {
        Number Test13 { get; set; }
        Number this[Number N] { get; }
    }

    [System.Serializable]
    public class CoverageBase : ICoverageBase
    {
        #region Properties
        public virtual Number Test13 { get; set; }
        #endregion

        #region Client Interface
        public Number Test10 { get; private set; }
        public const Number Test11 = 0;
        #endregion
    }
}
