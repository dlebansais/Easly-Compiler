namespace EaslyCompiler
{
    using System;

    /// <summary>
    /// Invalid Input File or stream.
    /// </summary>
    public class ErrorInputFileInvalid : Error
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInputFileInvalid"/> class.
        /// </summary>
        /// <param name="exception">The exception that occured reading the file or stream.</param>
        public ErrorInputFileInvalid(Exception exception)
            : base(ErrorLocation.NoLocation)
        {
            Exception = exception;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The exception that occured reading the file or stream.
        /// </summary>
        public Exception Exception { get; }
        #endregion
    }
}
