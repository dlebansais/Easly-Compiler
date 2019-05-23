namespace EaslyCompiler
{
    /// <summary>
    /// Bad feature.
    /// </summary>
    public interface IErrorCreationFeatureRequired : IError
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        string FeatureName { get; }
    }

    /// <summary>
    /// Bad feature.
    /// </summary>
    internal class ErrorCreationFeatureRequired : Error, IErrorCreationFeatureRequired
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCreationFeatureRequired"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="featureName">The feature name.</param>
        public ErrorCreationFeatureRequired(ISource source, string featureName)
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
        public override string Message { get { return $"Feature '{FeatureName}' must be a creation routine."; } }
        #endregion
    }
}
