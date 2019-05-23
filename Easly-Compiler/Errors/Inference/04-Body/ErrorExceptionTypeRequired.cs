namespace EaslyCompiler
{
    /// <summary>
    /// Invalid type.
    /// </summary>
    public interface IErrorExceptionTypeRequired : IError
    {
    }

    /// <summary>
    /// Invalid type.
    /// </summary>
    internal class ErrorExceptionTypeRequired : Error, IErrorExceptionTypeRequired
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorExceptionTypeRequired"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorExceptionTypeRequired(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type not inheriting from 'Exception'."; } }
        #endregion
    }
}
