namespace CompilerNode
{
    /// <summary>
    /// Compiler IPreprocessorExpression.
    /// </summary>
    public interface IPreprocessorExpression : BaseNode.IPreprocessorExpression
    {
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        string ExpressionToString { get; }
    }

    /// <summary>
    /// Compiler IPreprocessorExpression.
    /// </summary>
    public class PreprocessorExpression : BaseNode.PreprocessorExpression, IPreprocessorExpression
    {
        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString { get { return $"{Value}†"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Preprocessor Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
