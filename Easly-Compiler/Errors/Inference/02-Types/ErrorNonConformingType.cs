namespace EaslyCompiler
{
    /// <summary>
    /// A type doesn't conform to a base type.
    /// </summary>
    public interface IErrorNonConformingType : IError
    {
    }

    /// <summary>
    /// A type doesn't conform to a base type.
    /// </summary>
    internal class ErrorNonConformingType : Error, IErrorNonConformingType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorNonConformingType"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorNonConformingType(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type must conform to base."; } }
        #endregion
    }
}
