namespace EaslyCompiler
{
    /// <summary>
    /// Some list must not be empty.
    /// </summary>
    public class ErrorEmptyList : Error
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorEmptyList"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="listName">The list name.</param>
        public ErrorEmptyList(ISource source, string listName)
            : base(source)
        {
            ListName = listName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list name.
        /// </summary>
        public string ListName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"List '{ListName}' must contain at least one item."; } }
        #endregion
    }
}
