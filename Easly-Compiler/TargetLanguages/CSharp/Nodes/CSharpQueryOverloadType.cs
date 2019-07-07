namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# query overload type node.
    /// </summary>
    public interface ICSharpQueryOverloadType : ICSharpSource<IQueryOverloadType>
    {
        /// <summary>
        /// The list of parameters.
        /// </summary>
        IList<ICSharpParameter> ParameterList { get; }

        /// <summary>
        /// The list of results.
        /// </summary>
        IList<ICSharpParameter> ResultList { get; }

        /// <summary>
        /// Gets the feature output format.
        /// </summary>
        /// <param name="outgoingParameterCount">The number of 'out' parameters upon return.</param>
        /// <param name="returnValueIndex">Index of the return value if the feature returns a value, -1 otherwise.</param>
        void GetOutputFormat(out int outgoingParameterCount, out int returnValueIndex);
    }

    /// <summary>
    /// A C# query overload type node.
    /// </summary>
    public class CSharpQueryOverloadType : CSharpSource<IQueryOverloadType>, ICSharpQueryOverloadType
    {
        #region Init
        /// <summary>
        /// Create a new C# overload.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="owner">The class where the overload is declared.</param>
        public static ICSharpQueryOverloadType Create(ICSharpContext context, IQueryOverloadType source, ICSharpClass owner)
        {
            return new CSharpQueryOverloadType(context, source, owner);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpQueryOverloadType"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="owner">The class where the overload is declared.</param>
        protected CSharpQueryOverloadType(ICSharpContext context, IQueryOverloadType source, ICSharpClass owner)
            : base(source)
        {
            foreach (IParameter Parameter in source.ParameterTable)
            {
                ICSharpParameter NewParameter = CSharpParameter.Create(context, Parameter, owner);
                ParameterList.Add(NewParameter);
            }

            foreach (IParameter Result in source.ResultTable)
            {
                ICSharpParameter NewResult = CSharpParameter.Create(context, Result, owner);
                ResultList.Add(NewResult);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list of parameters.
        /// </summary>
        public IList<ICSharpParameter> ParameterList { get; } = new List<ICSharpParameter>();

        /// <summary>
        /// The list of results.
        /// </summary>
        public IList<ICSharpParameter> ResultList { get; } = new List<ICSharpParameter>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the feature output format.
        /// </summary>
        /// <param name="outgoingParameterCount">The number of 'out' parameters upon return.</param>
        /// <param name="returnValueIndex">Index of the return value if the feature returns a value, -1 otherwise.</param>
        public virtual void GetOutputFormat(out int outgoingParameterCount, out int returnValueIndex)
        {
            outgoingParameterCount = ResultList.Count;
            returnValueIndex = -1;

            for (int i = 0; i < ResultList.Count; i++)
            {
                ICSharpParameter Result = ResultList[i];
                if (Result.Name == nameof(BaseNode.Keyword.Result))
                {
                    Debug.Assert(returnValueIndex == -1);
                    returnValueIndex = i;
                }
            }
        }

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
        public void WriteCSharp(ICSharpWriter writer, CSharpFeatureTextTypes featureTextType, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            //TODO
        }
        #endregion
    }
}
