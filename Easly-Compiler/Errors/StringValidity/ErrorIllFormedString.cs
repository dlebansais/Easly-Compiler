namespace EaslyCompiler
{
    /// <summary>
    /// Invalid unicode string.
    /// </summary>
    public class ErrorIllFormedString : ErrorStringValidity
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorIllFormedString"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorIllFormedString(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Ill-formed string."; } }
        #endregion
    }
}
