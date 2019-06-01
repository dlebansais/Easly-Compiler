namespace Test
{
    public interface ICoveragePropertyFeature
    {
        String Test { get; set; }
    }

    [System.Serializable]
    public class CoveragePropertyFeature : ICoveragePropertyFeature
    {
        #region Properties
        public virtual String Test
        {
            get
            set
        }
        protected String _Test;
        #endregion
    }
}
