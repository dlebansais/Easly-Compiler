namespace EaslyCompiler
{
    /// <summary>
    /// Ancestor not found.
    /// </summary>
    public interface IErrorMissingAncestor : IError
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        string FeatureName { get; }
    }

    /// <summary>
    /// Ancestor not found.
    /// </summary>
    internal class ErrorMissingAncestor : Error, IErrorMissingAncestor
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMissingAncestor"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="featureName">The feature name</param>
        public ErrorMissingAncestor(ISource source, string featureName)
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
        public override string Message { get { return $"Feature '{FeatureName}' has one or more precursor bodies but no ancestor."; } }
        #endregion
    }
}
