namespace EaslyCompiler
{
    /// <summary>
    /// Bad class.
    /// </summary>
    public interface IErrorCloningNotAllowed : IError
    {
        /// <summary>
        /// The class name.
        /// </summary>
        string ClassName { get; }
    }

    /// <summary>
    /// Bad class.
    /// </summary>
    internal class ErrorCloningNotAllowed : Error, IErrorCloningNotAllowed
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCloningNotAllowed"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="className">The class name.</param>
        public ErrorCloningNotAllowed(ISource source, string className)
            : base(source)
        {
            ClassName = className;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The class name.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Class '{ClassName}' cannot be cloned."; } }
        #endregion
    }
}
