namespace EaslyCompiler
{
    /// <summary>
    /// Bad feature.
    /// </summary>
    public interface IErrorCreatedFeatureNotAttributeOrProperty : IError
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        string FeatureName { get; }
    }

    /// <summary>
    /// Bad feature.
    /// </summary>
    internal class ErrorCreatedFeatureNotAttributeOrProperty : Error, IErrorCreatedFeatureNotAttributeOrProperty
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCreatedFeatureNotAttributeOrProperty"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="featureName">The feature name.</param>
        public ErrorCreatedFeatureNotAttributeOrProperty(ISource source, string featureName)
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
        public override string Message { get { return $"Feature '{FeatureName}' must be an attribute or a property."; } }
        #endregion
    }
}
