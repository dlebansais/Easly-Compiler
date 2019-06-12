namespace EaslyCompiler
{
    using System.Collections.Generic;

    /// <summary>
    /// A C# overload (query or command).
    /// </summary>
    public interface ICSharpOverload
    {
        /// <summary>
        /// The parent feature.
        /// </summary>
        ICSharpFeature ParentFeature { get; }

        /// <summary>
        /// The list of parameters.
        /// </summary>
        IList<ICSharpParameter> ParameterList { get; }

        /// <summary>
        /// The precursor overload. Can be null.
        /// </summary>
        ICSharpOverload Precursor { get; }

        /// <summary>
        /// Writes down the C# overload of a feature.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="featureTextType">The write mode.</param>
        /// <param name="isOverride">True if the feature is an override.</param>
        /// <param name="nameString">The composed feature name.</param>
        /// <param name="exportStatus">The feature export status.</param>
        /// <param name="isConstructor">True if the feature is a constructor.</param>
        /// <param name="isFirstFeature">True if the feature is the first in a list.</param>
        /// <param name="isMultiline">True if there is a separating line above.</param>
        void WriteCSharp(ICSharpWriter writer, CSharpFeatureTextTypes featureTextType, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline);
    }
}
