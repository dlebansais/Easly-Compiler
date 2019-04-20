namespace EaslyCompiler
{
    /// <summary>
    /// Use of a generic class without an expected type arguments.
    /// </summary>
    public interface IErrorMissingTypeArgument : IError
    {
        /// <summary>
        /// The generic name.
        /// </summary>
        string GenericName { get; }
    }

    /// <summary>
    /// Use of a generic class without an expected type arguments.
    /// </summary>
    internal class ErrorMissingTypeArgument : Error, IErrorMissingTypeArgument
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMissingTypeArgument"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="genericName">The generic name.</param>
        public ErrorMissingTypeArgument(ISource source, string genericName)
            : base(source)
        {
            GenericName = genericName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The generic name.
        /// </summary>
        public string GenericName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Type argument '{GenericName}' missing."; } }
        #endregion
    }
}
