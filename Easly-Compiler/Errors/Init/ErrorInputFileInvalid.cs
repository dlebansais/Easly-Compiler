namespace EaslyCompiler
{
    using System;

    /// <summary>
    /// Invalid Input File or stream.
    /// </summary>
    public interface IErrorInputFileInvalid : IError
    {
        /// <summary>
        /// The exception that occured reading the file or stream.
        /// </summary>
        Exception Exception { get; }
    }

    /// <summary>
    /// Invalid Input File or stream.
    /// </summary>
    internal class ErrorInputFileInvalid : Error, IErrorInputFileInvalid
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

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return Exception.Message; } }
        #endregion
    }
}
