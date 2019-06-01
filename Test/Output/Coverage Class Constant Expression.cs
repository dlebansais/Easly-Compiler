namespace Test
{
    public interface ICoverageClassConstantExpression
    {
        Number Test5 { get; set; }
        Number this[Number N] { get; }
    }

    [System.Serializable]
    public class CoverageClassConstantExpression : ICoverageClassConstantExpression
    {
        #region Properties
        public virtual Number Test5
        {
            get
            set { throw new InvalidOperationException(); }
        }
        protected Number _Test5;
        #endregion

        #region Client Interface
        public const Number Constant1 = 10;
        public const Number Test3 = CoverageClassConstantExpression.Constant1;
        public const Number Constant2 = ;
        public const Number Constant3 = Discrete3;
        public const Number Test41 = Discrete1;
        public const Number Test42 = Discrete2;
        #endregion
    }
}
