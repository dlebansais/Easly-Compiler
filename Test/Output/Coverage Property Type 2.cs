namespace Test
{
    public interface ICoveragePropertyType2<IT, T>
    {
        Number Test { get; set; }
    }

    [System.Serializable]
    public class CoveragePropertyType2<IT, T> : ICoveragePropertyType2<IT, T>
    {
        #region Properties
        public virtual Number Test { get; set; }
        #endregion
    }
}
