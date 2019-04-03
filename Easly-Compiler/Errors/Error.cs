namespace EaslyCompiler
{
    /// <summary>
    /// Base interface for errors.
    /// </summary>
    public interface IError
    {
        /// <summary>
        /// The error location.
        /// </summary>
        ErrorLocation Location { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        string Message { get; }
    }

    /// <summary>
    /// Base class for errors.
    /// </summary>
    public abstract class Error : IError
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="location">The error location.</param>
        public Error(ErrorLocation location)
        {
            Location = location;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public Error(ISource source)
        {
            Location = new ErrorLocation(source);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error location.
        /// </summary>
        public ErrorLocation Location { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public abstract string Message { get; }
        #endregion
    }
}
