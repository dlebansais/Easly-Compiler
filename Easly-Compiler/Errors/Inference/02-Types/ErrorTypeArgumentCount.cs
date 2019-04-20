namespace EaslyCompiler
{
    /// <summary>
    /// Use of a generic class with not enough type arguments.
    /// </summary>
    public interface IErrorTypeArgumentCount : IError
    {
        /// <summary>
        /// The class name.
        /// </summary>
        string ClassName { get; }

        /// <summary>
        /// The required count of arguments.
        /// </summary>
        int RequiredArgumentCount { get; }
    }

    /// <summary>
    /// Use of a generic class with not enough type arguments.
    /// </summary>
    internal class ErrorTypeArgumentCount : Error, IErrorTypeArgumentCount
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorTypeArgumentCount"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="className">The class name.</param>
        /// <param name="requiredArgumentCount">The required count of arguments.</param>
        public ErrorTypeArgumentCount(ISource source, string className, int requiredArgumentCount)
            : base(source)
        {
            ClassName = className;
            RequiredArgumentCount = requiredArgumentCount;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The class name.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// The required count of arguments.
        /// </summary>
        public int RequiredArgumentCount { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Using the generic class '{ClassName}' requires at least '{RequiredArgumentCount}' type arguments."; } }
        #endregion
    }
}
