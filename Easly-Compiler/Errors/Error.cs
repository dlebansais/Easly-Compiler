namespace EaslyCompiler
{
    /// <summary>
    /// Base class for errors.
    /// </summary>
    public class Error
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
        #endregion

        #region Properties
        /// <summary>
        /// The error location.
        /// </summary>
        public ErrorLocation Location { get; }
        #endregion
    }
}
