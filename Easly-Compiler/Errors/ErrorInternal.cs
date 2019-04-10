namespace EaslyCompiler
{
    using System;

    /// <summary>
    /// Internal Error.
    /// </summary>
    public interface IErrorInternal : IError
    {
        /// <summary>
        /// The exception leading to the error.
        /// </summary>
        Exception Exception { get; }
    }

    /// <summary>
    /// Internal Error.
    /// </summary>
    internal class ErrorInternal : Error, IErrorInternal
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInternal"/> class.
        /// </summary>
        /// <param name="exception">The exception leading to the error.</param>
        public ErrorInternal(Exception exception)
            : base(ErrorLocation.NoLocation)
        {
            Exception = exception;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The exception leading to the error.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return Exception.Message; } }
        #endregion
    }
}
