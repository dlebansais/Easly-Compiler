namespace EaslyCompiler
{
    /// <summary>
    /// Class source missing.
    /// </summary>
    public interface IErrorSourceRequired : IError
    {
    }

    /// <summary>
    /// Class source missing.
    /// </summary>
    internal class ErrorSourceRequired : Error, IErrorSourceRequired
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorSourceRequired"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorSourceRequired(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "All items with the same name must have a source."; } }
        #endregion
    }
}
