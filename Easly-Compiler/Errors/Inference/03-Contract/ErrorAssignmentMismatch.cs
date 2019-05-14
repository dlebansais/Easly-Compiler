namespace EaslyCompiler
{
    /// <summary>
    /// Invalid assignment.
    /// </summary>
    public interface IErrorAssignmentMismatch : IError
    {
    }

    /// <summary>
    /// Invalid assignment.
    /// </summary>
    internal class ErrorAssignmentMismatch : Error, IErrorAssignmentMismatch
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorAssignmentMismatch"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorAssignmentMismatch(ISource source)
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
