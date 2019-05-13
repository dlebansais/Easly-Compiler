namespace EaslyCompiler
{
    /// <summary>
    /// Feature not a constant.
    /// </summary>
    public interface IErrorConstantRequired : IError
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        string FeatureName { get; }
    }

    /// <summary>
    /// Feature not a constant.
    /// </summary>
    internal class ErrorConstantRequired : Error, IErrorConstantRequired
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorConstantRequired"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="featureName">The feature name.</param>
        public ErrorConstantRequired(ISource source, string featureName)
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
        public override string Message { get { return $"A constant is required and '{FeatureName}' does not evaluate to a constant."; } }
        #endregion
    }
}
