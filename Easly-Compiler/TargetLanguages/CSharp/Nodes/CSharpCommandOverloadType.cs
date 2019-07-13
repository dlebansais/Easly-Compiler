namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A C# command overload type node.
    /// </summary>
    public interface ICSharpCommandOverloadType : ICSharpSource<ICommandOverloadType>
    {
        /// <summary>
        /// The list of parameters.
        /// </summary>
        IList<ICSharpParameter> ParameterList { get; }
    }

    /// <summary>
    /// A C# command overload type node.
    /// </summary>
    public class CSharpCommandOverloadType : CSharpSource<ICommandOverloadType>, ICSharpCommandOverloadType
    {
        #region Init
        /// <summary>
        /// Create a new C# overload.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="owner">The class where the overload is declared.</param>
        public static ICSharpCommandOverloadType Create(ICSharpContext context, ICommandOverloadType source, ICSharpClass owner)
        {
            return new CSharpCommandOverloadType(context, source, owner);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCommandOverloadType"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="owner">The class where the overload is declared.</param>
        protected CSharpCommandOverloadType(ICSharpContext context, ICommandOverloadType source, ICSharpClass owner)
            : base(source)
        {
            foreach (IParameter Parameter in source.ParameterTable)
            {
                ICSharpParameter NewParameter = CSharpParameter.Create(context, Parameter, owner);
                ParameterList.Add(NewParameter);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list of parameters.
        /// </summary>
        public IList<ICSharpParameter> ParameterList { get; } = new List<ICSharpParameter>();
        #endregion

        #region Client Interface
        /*/// <summary>
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
        public void WriteCSharp(ICSharpWriter writer, CSharpFeatureTextTypes featureTextType, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            //TODO
        }*/
        #endregion
    }
}
