namespace Test
{
    public interface ICoverageTupleType2<IT, T>
    {
        <Not Supported> Test { get; set; }
    }

    [System.Serializable]
    public class CoverageTupleType2<IT, T> : ICoverageTupleType2<IT, T>
    {
        #region Properties
        public virtual <Not Supported> Test { get; set; }
        #endregion
    }
}
