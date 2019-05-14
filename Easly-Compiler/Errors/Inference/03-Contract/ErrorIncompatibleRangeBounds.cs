namespace EaslyCompiler
{
    /// <summary>
    /// Invalid arguments.
    /// </summary>
    public interface IErrorIncompatibleRangeBounds : IError
    {
    }

    /// <summary>
    /// Invalid arguments.
    /// </summary>
    internal class ErrorIncompatibleRangeBounds : Error, IErrorIncompatibleRangeBounds
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorIncompatibleRangeBounds"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorIncompatibleRangeBounds(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Incompatible range bounds."; } }
        #endregion
    }
}
