namespace EaslyCompiler
{
    /// <summary>
    /// Two overloads can be confused.
    /// </summary>
    public interface IErrorMoreBasicParameter : IError
    {
    }

    /// <summary>
    /// Two overloads can be confused.
    /// </summary>
    internal class ErrorMoreBasicParameter : Error, IErrorMoreBasicParameter
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMoreBasicParameter"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorMoreBasicParameter(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Derived type used in a different overload."; } }
        #endregion
    }
}
