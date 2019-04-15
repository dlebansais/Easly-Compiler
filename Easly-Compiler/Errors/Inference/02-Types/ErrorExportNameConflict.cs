namespace EaslyCompiler
{
    /// <summary>
    /// Export already inherited.
    /// </summary>
    public interface IErrorExportNameConflict : IError
    {
        /// <summary>
        /// The new name.
        /// </summary>
        string NewName { get; }

        /// <summary>
        /// The previous name.
        /// </summary>
        string PreviousName { get; }
    }

    /// <summary>
    /// Export already inherited.
    /// </summary>
    internal class ErrorExportNameConflict : Error, IErrorExportNameConflict
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorExportNameConflict"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="newName">The new name</param>
        /// <param name="previousName">The previous name.</param>
        public ErrorExportNameConflict(ISource source, string newName, string previousName)
            : base(source)
        {
            NewName = newName;
            PreviousName = previousName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The new name.
        /// </summary>
        public string NewName { get; }

        /// <summary>
        /// The previous name.
        /// </summary>
        public string PreviousName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Export '{NewName}' is already inherited with the name '{PreviousName}'."; } }
        #endregion
    }
}
