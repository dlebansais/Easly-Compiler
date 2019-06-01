namespace Test
{
    public interface ICoverageKeywordAnchoredType
    {
        Number Test2 { get; set; }
    }

    [System.Serializable]
    public class CoverageKeywordAnchoredType : ICoverageKeywordAnchoredType
    {
        #region Properties
        public virtual Number Test2
        {
            get
            set
        }
        protected Number _Test2;
        #endregion

        #region Client Interface
        public ICoverageKeywordAnchoredType Test1 { get; private set; }
        public Boolean Test4 { get; private set; }
        public Exception Test5 { get; private set; }
        public Boolean Test6 { get; private set; }
        public Boolean Test7 { get; private set; }
        #endregion
    }
}
