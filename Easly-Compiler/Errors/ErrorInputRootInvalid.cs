namespace EaslyCompiler
{
    using BaseNode;

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
            : base(new ErrorLocation(root))
        {
        }
        #endregion
    }
}
