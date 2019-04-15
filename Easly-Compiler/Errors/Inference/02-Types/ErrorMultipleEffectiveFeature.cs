namespace EaslyCompiler
{
    /// <summary>
    /// Inheritance of multiple features with the same name.
    /// </summary>
    public interface IErrorMultipleEffectiveFeature : IError
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        string FeatureName { get; }
    }

    /// <summary>
    /// Inheritance of multiple features with the same name.
    /// </summary>
    internal class ErrorMultipleEffectiveFeature : Error, IErrorMultipleEffectiveFeature
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMultipleEffectiveFeature"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="featureName">The feature name</param>
        public ErrorMultipleEffectiveFeature(ISource source, string featureName)
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
        public override string Message { get { return $"Multiple effective version of '{FeatureName}' encountered."; } }
        #endregion
    }
}
