namespace Test
{
    public interface ICoverageAssertionTagExpression : ICoverageClassConstantExpression
    {
    }

    [System.Serializable]
    public class CoverageAssertionTagExpression : CoverageClassConstantExpression, ICoverageAssertionTagExpression
    {
        #region Client Interface
        #endregion
    }
}
