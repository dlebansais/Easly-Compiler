namespace EaslyCompiler
{
    /// <summary>
    /// Discrete already inherited.
    /// </summary>
    public interface IErrorDiscreteNameConflict : IError
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
    /// Discrete already inherited.
    /// </summary>
    internal class ErrorDiscreteNameConflict : Error, IErrorDiscreteNameConflict
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDiscreteNameConflict"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="newName">The new name</param>
        /// <param name="previousName">The previous name.</param>
        public ErrorDiscreteNameConflict(ISource source, string newName, string previousName)
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
        public override string Message { get { return $"Discrete '{NewName}' is already inherited with the name '{PreviousName}'."; } }
        #endregion
    }
}
