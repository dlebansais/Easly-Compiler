namespace EaslyCompiler
{
    using BaseNode;

    /// <summary>
    /// Macro used outside a valid context.
    /// </summary>
    public class ErrorMacroOutOfContext : Error
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMacroOutOfContext"/> class.
        /// </summary>
        /// <param name="location">The error location.</param>
        public ErrorMacroOutOfContext(IPreprocessorExpression location)
            : base(new ErrorLocation(location))
        {
        }
        #endregion
    }
}
