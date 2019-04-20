namespace EaslyCompiler
{
    /// <summary>
    /// Can only inherit from a class once.
    /// </summary>
    public interface IErrorTypeAlreadyInherited : IError
    {
    }

    /// <summary>
    /// Can only inherit from a class once.
    /// </summary>
    internal class ErrorTypeAlreadyInherited : Error, IErrorTypeAlreadyInherited
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorTypeAlreadyInherited"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorTypeAlreadyInherited(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type already inherited."; } }
        #endregion
    }
}
