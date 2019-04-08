namespace EaslyCompiler
{
    /// <summary>
    /// Duplicate import.
    /// </summary>
    public interface IErrorDuplicateImport : IError
    {
        /// <summary>
        /// The library name.
        /// </summary>
        string LibraryName { get; }

        /// <summary>
        /// The source name.
        /// </summary>
        string SourceName { get; }
    }

    /// <summary>
    /// Duplicate import.
    /// </summary>
    internal class ErrorDuplicateImport : Error, IErrorDuplicateImport
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDuplicateImport"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="libraryName">The library name</param>
        /// <param name="sourceName">The source name.</param>
        public ErrorDuplicateImport(ISource source, string libraryName, string sourceName)
            : base(source)
        {
            LibraryName = libraryName;
            SourceName = sourceName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The library name.
        /// </summary>
        public string LibraryName { get; }

        /// <summary>
        /// The source name.
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message
        {
            get
            {
                if (SourceName.Length > 0)
                    return $"Library '{LibraryName}' from '{SourceName}' already imported.";
                else
                    return $"Library '{LibraryName}' already imported.";
            }
        }
        #endregion
    }
}
