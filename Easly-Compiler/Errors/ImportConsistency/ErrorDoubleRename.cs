namespace EaslyCompiler
{
    /// <summary>
    /// Double rename.
    /// </summary>
    public interface IErrorDoubleRename : IError
    {
        /// <summary>
        /// The source name.
        /// </summary>
        string SourceName { get; }

        /// <summary>
        /// The destination name.
        /// </summary>
        string DestinationName { get; }
    }

    /// <summary>
    /// Double rename.
    /// </summary>
    internal class ErrorDoubleRename : Error, IErrorDoubleRename
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDoubleRename"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="sourceName">The source name.</param>
        /// <param name="destinationName">The destination name</param>
        public ErrorDoubleRename(ISource source, string sourceName, string destinationName)
            : base(source)
        {
            SourceName = sourceName;
            DestinationName = destinationName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The source name.
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// The destination name.
        /// </summary>
        public string DestinationName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"'{SourceName}' has already been renamed as '{DestinationName}'."; } }
        #endregion
    }
}
