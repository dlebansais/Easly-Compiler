namespace EaslyCompiler
{
    /// <summary>
    /// Input File Not Found.
    /// </summary>
    public class ErrorInputFileNotFound : Error
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInputFileNotFound"/> class.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public ErrorInputFileNotFound(string fileName)
            : base(ErrorLocation.NoLocation)
        {
            FileName = fileName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The file name.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"File not found: '{FileName}'."; } }
        #endregion
    }
}
