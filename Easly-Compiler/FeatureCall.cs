namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// Arguments and matching parameters of a feature call.
    /// </summary>
    public interface IFeatureCall
    {
        /// <summary>
        /// List of parameters from the selected overload.
        /// </summary>
        IList<IParameter> ParameterList { get; }

        /// <summary>
        /// List of results from the selected overload.
        /// </summary>
        IList<IParameter> ResultList { get; }

        /// <summary>
        /// Arguments of the call.
        /// </summary>
        IList<IArgument> ArgumentList { get; }

        /// <summary>
        /// Resolved arguments of the call.
        /// </summary>
        IList<IExpressionType> ResolvedArgumentList { get; }

        /// <summary>
        /// The argument passing style.
        /// </summary>
        TypeArgumentStyles TypeArgumentStyle { get; }

        /// <summary>
        /// True if the call has no parameters or arguments.
        /// </summary>
        bool IsEmpty { get; }
    }

    /// <summary>
    /// Arguments and matching parameters of a feature call.
    /// </summary>
    public class FeatureCall : IFeatureCall
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCall"/> class.
        /// </summary>
        public FeatureCall()
        {
            ParameterList = new List<IParameter>();
            ResultList = new List<IParameter>();
            ArgumentList = new List<IArgument>();
            ResolvedArgumentList = new List<IExpressionType>();
            TypeArgumentStyle = TypeArgumentStyles.None;

            Debug.Assert(IsEmpty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCall"/> class.
        /// </summary>
        /// <param name="parameterList">List of parameters from the selected overload.</param>
        /// <param name="resultList">List of results from the selected overload.</param>
        /// <param name="argumentList">Arguments of the call.</param>
        /// <param name="resolvedArgumentList">Resolved arguments of the call.</param>
        /// <param name="typeArgumentStyle">The argument passing style.</param>
        public FeatureCall(IList<IParameter> parameterList, IList<IParameter> resultList, IList<IArgument> argumentList, IList<IExpressionType> resolvedArgumentList, TypeArgumentStyles typeArgumentStyle)
        {
            ParameterList = parameterList;
            ResultList = resultList;
            ArgumentList = argumentList;
            ResolvedArgumentList = resolvedArgumentList;
            TypeArgumentStyle = typeArgumentStyle;
        }
        #endregion

        #region Properties
        /// <summary>
        /// List of parameters from the selected overload.
        /// </summary>
        public IList<IParameter> ParameterList { get; }

        /// <summary>
        /// List of results from the selected overload.
        /// </summary>
        public IList<IParameter> ResultList { get; }

        /// <summary>
        /// Resolved arguments of the call.
        /// </summary>
        public IList<IArgument> ArgumentList { get; }

        /// <summary>
        /// Resolved arguments of the call.
        /// </summary>
        public IList<IExpressionType> ResolvedArgumentList { get; }

        /// <summary>
        /// The argument passing style.
        /// </summary>
        public TypeArgumentStyles TypeArgumentStyle { get; }

        /// <summary>
        /// True if the call has no parameters or arguments.
        /// </summary>
        public bool IsEmpty { get { return ParameterList.Count == 0 && ResultList.Count <= 1 && ArgumentList.Count == 0 && ResolvedArgumentList.Count == 0 && TypeArgumentStyle == TypeArgumentStyles.None; } }
        #endregion

        #region Client Interface
        #endregion
    }
}
