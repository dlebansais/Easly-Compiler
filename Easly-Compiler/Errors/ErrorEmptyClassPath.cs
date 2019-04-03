namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// Empty class path.
    /// </summary>
    public class ErrorEmptyClassPath : Error
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEmptyClassPath"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorEmptyClassPath(IClass source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Class has an empty class path."; } }
        #endregion
    }
}
