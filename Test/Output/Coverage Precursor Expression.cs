namespace Test
{
    public interface ICoveragePrecursorExpression : ICoverageClassConstantExpression
    {
    }

    [System.Serializable]
    public class CoveragePrecursorExpression : CoverageClassConstantExpression, ICoveragePrecursorExpression
    {
        #region Properties
        public override Number Test5
        {
            get
            set
        }
        #endregion

        #region Client Interface
        public const Number Test3 = base.Test3();
        #endregion
    }
}
