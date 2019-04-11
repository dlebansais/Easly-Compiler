namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// Invalid Input Root.
    /// </summary>
    public interface IErrorInputRootInvalid : IError
    {
    }

    /// <summary>
    /// Invalid Input Root.
    /// </summary>
    internal class ErrorInputRootInvalid : Error, IErrorInputRootInvalid
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInputRootInvalid"/> class.
        /// </summary>
        public ErrorInputRootInvalid()
            : base(ErrorLocation.NoLocation)
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
