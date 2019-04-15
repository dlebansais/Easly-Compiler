namespace EaslyCompiler
{
    /// <summary>
    /// Inheritance with conflicting settings.
    /// </summary>
    public interface IErrorInheritanceConflict : IError
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        string FeatureName { get; }
    }

    /// <summary>
    /// Inheritance with conflicting settings.
    /// </summary>
    internal class ErrorInheritanceConflict : Error, IErrorInheritanceConflict
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInheritanceConflict"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="featureName">The feature name</param>
        public ErrorInheritanceConflict(ISource source, string featureName)
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
        public override string Message { get { return $"Feature '{FeatureName}' is inherited with conflicting settings."; } }
        #endregion
    }
}
