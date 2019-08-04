namespace EaslyCompiler
{
    /// <summary>
    /// Two overloads can be confused.
    /// </summary>
    public interface IErrorEqualParameters : IError
    {
    }

    /// <summary>
    /// Two overloads can be confused.
    /// </summary>
    internal class ErrorEqualParameters : Error, IErrorEqualParameters
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEqualParameters"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorEqualParameters(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Identical type used in a different overload."; } }
        #endregion
    }
}
