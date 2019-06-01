namespace Test
{
    public interface ICoverageAnchoredType
    {
        ICoverageBase Test9 { get; set; }
        ICoverageBase this[Number N] { get; }
    }

    [System.Serializable]
    public class CoverageAnchoredType : ICoverageAnchoredType
    {
        #region Properties
        public virtual ICoverageBase Test9 { get; set; }
        #endregion

        #region Client Interface
        public Number Test15 { get; private set; }
        public Number Test14 { get; private set; }
        public Number Test13 { get; private set; }
        public Number Test12 { get; private set; }
        public Number Test1 { get; private set; }
        public Number Test2 { get; private set; }
        public ICoverageBase Test3 { get; private set; }
        public Number Test4 { get; private set; }
        public Number Test6 { get; private set; }
        public Number Test8 { get; private set; }
        public Number Test10 { get; private set; }
        public Number Test11 { get; private set; }
        public const ICoverageBase Test5 = 0;
        #endregion
    }
}
