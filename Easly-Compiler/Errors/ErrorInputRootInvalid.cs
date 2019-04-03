namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// Invalid Input Root.
    /// </summary>
    public class ErrorInputRootInvalid : Error
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInputRootInvalid"/> class.
        /// </summary>
        /// <param name="root">The error location.</param>
        public ErrorInputRootInvalid(IRoot root)
            : base(root)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Invalid root object."; } }
        #endregion
    }
}
