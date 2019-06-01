namespace Test
{
    public interface ICoverageCreateInstruction
    {
        ICoverageCreateInstruction Test3 { get; set; }
    }

    [System.Serializable]
    public class CoverageCreateInstruction : ICoverageCreateInstruction
    {
        #region Init
        #endregion

        #region Properties
        public virtual ICoverageCreateInstruction Test3 { get; set; }
        #endregion

        #region Client Interface
        public ICoverageCreateInstruction Test2 { get; private set; }
        public ICoverageConstraint<ICoverageProcedureFeature, CoverageProcedureFeature> Test4 { get; private set; }
        #endregion
    }
}
