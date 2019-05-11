namespace EaslyCompiler
{
    /// <summary>
    /// An invalid instruction.
    /// </summary>
    public interface IErrorInvalidInstruction : IError
    {
    }

    /// <summary>
    /// An invalid instruction.
    /// </summary>
    internal class ErrorInvalidInstruction : Error, IErrorInvalidInstruction
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidInstruction"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorInvalidInstruction(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Invalid Instruction."; } }
        #endregion
    }
}
