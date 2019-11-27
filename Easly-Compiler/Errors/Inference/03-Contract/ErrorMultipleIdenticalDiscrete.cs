namespace EaslyCompiler
{
    using EaslyNumber;

    /// <summary>
    /// Discretes with the same value.
    /// </summary>
    public interface IErrorMultipleIdenticalDiscrete : IError
    {
        /// <summary>
        /// The shared number.
        /// </summary>
        Number Number { get; }
    }

    /// <summary>
    /// Discretes with the same value.
    /// </summary>
    internal class ErrorMultipleIdenticalDiscrete : Error, IErrorMultipleIdenticalDiscrete
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMultipleIdenticalDiscrete"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="number">The shared number.</param>
        public ErrorMultipleIdenticalDiscrete(ISource source, Number number)
            : base(source)
        {
            Number = number;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The shared number.
        /// </summary>
        public Number Number { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"More than one discrete with value '{Number}'."; } }
        #endregion
    }
}
