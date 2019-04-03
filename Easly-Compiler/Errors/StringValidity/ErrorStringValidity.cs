namespace EaslyCompiler
{
    /// <summary>
    /// Invalid string.
    /// </summary>
    public abstract class ErrorStringValidity : Error
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorStringValidity"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorStringValidity(ISource source)
            : base(source)
        {
        }
        #endregion
    }
}
