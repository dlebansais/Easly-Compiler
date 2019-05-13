namespace EaslyCompiler
{
    /// <summary>
    /// Call with too many argument.
    /// </summary>
    public interface IErrorTooManyArguments : IError
    {
        /// <summary>
        /// Actual argument count.
        /// </summary>
        int ActualCount { get; }

        /// <summary>
        /// Expected count.
        /// </summary>
        int ExpectedCount { get; }
    }

    /// <summary>
    /// Call with too many argument.
    /// </summary>
    internal class ErrorTooManyArguments : Error, IErrorTooManyArguments
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorTooManyArguments"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="actualCount">Actual argument count.</param>
        /// <param name="expectedCount">Expected count.</param>
        public ErrorTooManyArguments(ISource source, int actualCount, int expectedCount)
            : base(source)
        {
            ActualCount = actualCount;
            ExpectedCount = expectedCount;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Actual argument count.
        /// </summary>
        public int ActualCount { get; }

        /// <summary>
        /// Expected count.
        /// </summary>
        public int ExpectedCount { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Feature call uses too any arguments, found: {ActualCount}, expected: {ExpectedCount}."; } }
        #endregion
    }
}
