namespace EaslyCompiler
{
    using BaseNode;

    /// <summary>
    /// Location of an error.
    /// </summary>
    public class ErrorLocation
    {
        #region Init
        /// <summary>
        /// The default if an error has no location.
        /// </summary>
        public static ErrorLocation NoLocation { get; } = new ErrorLocation();

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLocation"/> class.
        /// </summary>
        protected ErrorLocation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLocation"/> class.
        /// </summary>
        /// <param name="node">The node location.</param>
        public ErrorLocation(INode node)
        {
            Node = node;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The node location.
        /// </summary>
        public INode Node { get; }
        #endregion
    }
}
