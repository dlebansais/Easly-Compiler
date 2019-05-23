namespace EaslyCompiler
{
    /// <summary>
    /// Invalid type.
    /// </summary>
    public interface IErrorReferenceTypeRequired : IError
    {
    }

    /// <summary>
    /// Invalid type.
    /// </summary>
    internal class ErrorReferenceTypeRequired : Error, IErrorReferenceTypeRequired
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReferenceTypeRequired"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorReferenceTypeRequired(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type must be a reference."; } }
        #endregion
    }
}
