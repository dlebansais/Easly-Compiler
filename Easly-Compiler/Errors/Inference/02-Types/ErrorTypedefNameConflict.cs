namespace EaslyCompiler
{
    /// <summary>
    /// Typedef already inherited.
    /// </summary>
    public interface IErrorTypedefNameConflict : IError
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
    /// Typedef already inherited.
    /// </summary>
    internal class ErrorTypedefNameConflict : Error, IErrorTypedefNameConflict
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorTypedefNameConflict"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="newName">The new name</param>
        /// <param name="previousName">The previous name.</param>
        public ErrorTypedefNameConflict(ISource source, string newName, string previousName)
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
        public override string Message { get { return $"Typedef '{NewName}' is already inherited with the name '{PreviousName}'."; } }
        #endregion
    }
}
