namespace EaslyCompiler
{
    /// <summary>
    /// Can only inherit from a class type.
    /// </summary>
    public interface IErrorClassTypeRequired : IError
    {
    }

    /// <summary>
    /// Can only inherit from a class type.
    /// </summary>
    internal class ErrorClassTypeRequired : Error, IErrorClassTypeRequired
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorClassTypeRequired"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorClassTypeRequired(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Only a class or generic type is allowed in this context."; } }
        #endregion
    }
}
