namespace EaslyCompiler
{
    /// <summary>
    /// Invalid string.
    /// </summary>
    public interface IErrorStringValidity : IError
    {
    }

    /// <summary>
    /// Invalid string.
    /// </summary>
    internal abstract class ErrorStringValidity : Error, IErrorStringValidity
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
