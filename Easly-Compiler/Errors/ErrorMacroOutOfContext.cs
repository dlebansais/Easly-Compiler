namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// Macro used outside a valid context.
    /// </summary>
    public class ErrorMacroOutOfContext : Error
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMacroOutOfContext"/> class.
        /// </summary>
        /// <param name="expression">The error location.</param>
        public ErrorMacroOutOfContext(IPreprocessorExpression expression)
            : base(expression)
        {
            InvalidMacro = expression.Value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The invalid character.
        /// </summary>
        public BaseNode.PreprocessorMacro InvalidMacro { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Macro '{InvalidMacro}' not available in this context."; } }
        #endregion
    }
}
