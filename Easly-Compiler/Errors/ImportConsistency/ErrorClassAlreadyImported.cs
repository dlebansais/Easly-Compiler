namespace EaslyCompiler
{
    /// <summary>
    /// Class already imported with a different name.
    /// </summary>
    public interface IErrorClassAlreadyImported : IError
    {
        /// <summary>
        /// The old name.
        /// </summary>
        string OldName { get; }

        /// <summary>
        /// The new name.
        /// </summary>
        string NewName { get; }
    }

    /// <summary>
    /// Class already imported with a different name.
    /// </summary>
    internal class ErrorClassAlreadyImported : Error, IErrorClassAlreadyImported
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorClassAlreadyImported"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name</param>
        public ErrorClassAlreadyImported(ISource source, string oldName, string newName)
            : base(source)
        {
            OldName = oldName;
            NewName = newName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The old name.
        /// </summary>
        public string OldName { get; }

        /// <summary>
        /// The new name.
        /// </summary>
        public string NewName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Class '{NewName}' already imported under name '{OldName}'."; } }
        #endregion
    }
}
