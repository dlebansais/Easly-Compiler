namespace Test
{
    public interface ICoveragePropertyType
    {
        <Not Supported> Test1 { get; set; }
        ICoveragePropertyType Test2 { get; }
        ICoveragePropertyType Test3 { set; }
    }

    [System.Serializable]
    public class CoveragePropertyType : ICoveragePropertyType
    {
        #region Properties
        public virtual <Not Supported> Test1 { get; set; }
        public virtual ICoveragePropertyType Test2
        {
            get
            protected set { _Test2 = value; }
        }
        protected ICoveragePropertyType _Test2;

        public virtual ICoveragePropertyType Test3
        {
            protected get { return _Test3; }
            set
        }
        protected ICoveragePropertyType _Test3;
        #endregion

        #region Client Interface
        public ICoveragePropertyType Test4 { get; private set; }
        #endregion
    }
}
