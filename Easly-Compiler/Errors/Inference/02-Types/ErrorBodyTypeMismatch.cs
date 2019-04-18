namespace EaslyCompiler
{
    /// <summary>
    /// Two bodies of different type in the same feature.
    /// </summary>
    public interface IErrorBodyTypeMismatch : IError
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        string FeatureName { get; }
    }

    /// <summary>
    /// Two bodies of different type in the same feature.
    /// </summary>
    internal class ErrorBodyTypeMismatch : Error, IErrorBodyTypeMismatch
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorBodyTypeMismatch"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="featureName">The feature name.</param>
        public ErrorBodyTypeMismatch(ISource source, string featureName)
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
        public override string Message { get { return $"Feature '{FeatureName}' has mismatching bodies."; } }
        #endregion
    }
}
