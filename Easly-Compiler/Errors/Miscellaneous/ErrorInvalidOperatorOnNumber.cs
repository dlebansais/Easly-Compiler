namespace EaslyCompiler
{
    /// <summary>
    /// Invalid operator.
    /// </summary>
    public interface IErrorInvalidOperatorOnNumber : IError
    {
        /// <summary>
        /// The operator name.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Duplicate name.
    /// </summary>
    internal class ErrorInvalidOperatorOnNumber : Error, IErrorInvalidOperatorOnNumber
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidOperatorOnNumber"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="name">The operator name.</param>
        public ErrorInvalidOperatorOnNumber(ISource source, string name)
            : base(source)
        {
            Name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The operator name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Invalid operator '{Name}' in this context."; } }
        #endregion
    }
}
