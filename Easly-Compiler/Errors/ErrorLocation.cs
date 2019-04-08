namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

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
        /// <param name="source">The node location.</param>
        public ErrorLocation(ISource source)
        {
            Debug.Assert(source != null);

            Node = source as INode;
            Debug.Assert(Node != null);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The node location.
        /// </summary>
        public INode Node { get; }
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            string Result = string.Empty;

            ISource Source = Node as ISource;
            while (Source != null)
            {
                if (Result.Length > 0)
                    Result = $"{Source} / {Result}";
                else
                    Result = Source.ToString();

                Source = Source.ParentSource;
            }

            return Result;
        }
        #endregion
    }
}
