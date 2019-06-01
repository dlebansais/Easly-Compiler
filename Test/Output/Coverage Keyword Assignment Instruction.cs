namespace Test
{
    public interface ICoverageKeywordAssignmentInstruction
    {
        Number Test { get; set; }
    }

    [System.Serializable]
    public class CoverageKeywordAssignmentInstruction : ICoverageKeywordAssignmentInstruction
    {
        #region Properties
        public virtual Number Test
        {
            get
            set { _Test = value; }
        }
        protected Number _Test;
        #endregion
    }
}
