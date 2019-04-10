namespace EaslyCompiler
{
    /// <summary>
    /// Invalid number.
    /// </summary>
    public interface IErrorInvalidManifestNumber : IError
    {
        /// <summary>
        /// The Invalid number.
        /// </summary>
        string Number { get; }
    }

    /// <summary>
    /// Invalid number.
    /// </summary>
    internal class ErrorInvalidManifestNumber : Error, IErrorInvalidManifestNumber
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidManifestNumber"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="number">The Invalid number.</param>
        public ErrorInvalidManifestNumber(ISource source, string number)
            : base(source)
        {
            Number = number;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Invalid number.
        /// </summary>
        public string Number { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Invalid Number '{Number}'."; } }
        #endregion
    }
}
