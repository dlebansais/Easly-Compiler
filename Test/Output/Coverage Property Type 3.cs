namespace Test
{
    public interface ICoveragePropertyType3 : ICoveragePropertyType2<IT, T>
    {
    }

    [System.Serializable]
    public class CoveragePropertyType3 : CoveragePropertyType2<IT, T>, ICoveragePropertyType3
    {
        #region Properties
        public override Number Test { get; set; }
        #endregion
    }
}
