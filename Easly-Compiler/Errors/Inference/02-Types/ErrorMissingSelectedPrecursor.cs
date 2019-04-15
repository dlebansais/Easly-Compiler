namespace EaslyCompiler
{
    /// <summary>
    /// Ancestor not selected.
    /// </summary>
    public interface IErrorMissingSelectedPrecursor : IError
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        string FeatureName { get; }
    }

    /// <summary>
    /// Ancestor not selected.
    /// </summary>
    internal class ErrorMissingSelectedPrecursor : Error, IErrorMissingSelectedPrecursor
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMissingSelectedPrecursor"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="featureName">The feature name</param>
        public ErrorMissingSelectedPrecursor(ISource source, string featureName)
            : base(source)
        {
            FeatureName = featureName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The feature name.
        /// </summary>
        public string FeatureName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Feature '{FeatureName}' is inherited from multiple ancestors and needs to be selected."; } }
        #endregion
    }
}
