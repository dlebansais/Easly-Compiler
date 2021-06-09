namespace CompilerNode
{
    using EaslyCompiler;
    /// <summary>
    /// Compiler IClass.
    /// </summary>
    public partial class Class : BaseNode.Class, IClass
    {
        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
            foreach (IFeature Feature in FeatureList)
                Feature.RestartNumberType(ref isChanged);
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            foreach (IFeature Feature in FeatureList)
                Feature.CheckNumberType(ref isChanged);
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
            foreach (IFeature Feature in FeatureList)
                Feature.ValidateNumberType(errorList);
        }
    }
}
