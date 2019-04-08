namespace EaslyCompiler
{
    /// <summary>
    /// Class importing itself.
    /// </summary>
    public interface IErrorNameChanged : IError
    {
        /// <summary>
        /// The class name.
        /// </summary>
        string ClassName { get; }

        /// <summary>
        /// The new name.
        /// </summary>
        string NewName { get; }
    }

    /// <summary>
    /// Class importing itself.
    /// </summary>
    internal class ErrorNameChanged : Error, IErrorNameChanged
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorNameChanged"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="className">The class name.</param>
        /// <param name="newName">The new name</param>
        public ErrorNameChanged(ISource source, string className, string newName)
            : base(source)
        {
            ClassName = className;
            NewName = newName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The class name.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// The new name.
        /// </summary>
        public string NewName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Class '{ClassName}' cannot import itself under name '{NewName}'"; } }
        #endregion
    }
}
