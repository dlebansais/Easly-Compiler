namespace Test
{
    public interface ICoverageEntityExpression
    {
        Number Test4 { get; set; }
        Number this[Number N] { get; }
    }

    [System.Serializable]
    public class CoverageEntityExpression : ICoverageEntityExpression
    {
        #region Init
        #endregion

        #region Properties
        public virtual Number Test4 { get; set; }
        #endregion

        #region Client Interface
        public Number Test3 { get; private set; }
        public const Number Test5 = 0;
        #endregion
    }
}
