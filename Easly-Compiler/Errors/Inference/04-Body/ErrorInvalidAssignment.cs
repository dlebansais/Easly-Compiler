namespace EaslyCompiler
{
    /// <summary>
    /// Invalid assignment.
    /// </summary>
    public interface IErrorInvalidAssignment : IError
    {
    }

    /// <summary>
    /// Invalid assignment.
    /// </summary>
    internal class ErrorInvalidAssignment : Error, IErrorInvalidAssignment
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidAssignment"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorInvalidAssignment(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Error in assignment."; } }
        #endregion
    }
}
