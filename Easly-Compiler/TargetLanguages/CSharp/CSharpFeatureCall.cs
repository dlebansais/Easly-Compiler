namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// Feature call information.
    /// </summary>
    public interface ICSharpFeatureCall
    {
        /// <summary>
        /// The list of parameters for the selected overload.
        /// </summary>
        IList<ICSharpParameter> ParameterList { get; }

        /// <summary>
        /// The list of arguments.
        /// </summary>
        IList<ICSharpArgument> ArgumentList { get; }

        /// <summary>
        /// The number of parameters and arguments.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The argument passing style.
        /// </summary>
        TypeArgumentStyles ArgumentStyle { get; }
    }

    /// <summary>
    /// Feature call information.
    /// </summary>
    public class CSharpFeatureCall : ICSharpFeatureCall
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFeatureCall"/> class.
        /// </summary>
        /// <param name="parameterList">The list of parameters for the selected overload.</param>
        /// <param name="argumentList">The list of arguments.</param>
        /// <param name="argumentStyle">The argument passing style.</param>
        /// <param name="context">The creation context.</param>
        public CSharpFeatureCall(IList<IParameter> parameterList, IList<IArgument> argumentList, TypeArgumentStyles argumentStyle, ICSharpContext context)
        {
            foreach (IParameter Item in parameterList)
            {
                ICSharpParameter NewParameter = CSharpParameter.Create(Item, context);
                ParameterList.Add(NewParameter);
            }

            foreach (IArgument Item in argumentList)
            {
                ICSharpArgument NewArgument = CSharpArgument.Create(Item, context);
                ArgumentList.Add(NewArgument);
            }

            Debug.Assert(parameterList.Count >= argumentList.Count);
            Count = parameterList.Count;

            ArgumentStyle = argumentStyle;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list of parameters for the selected overload.
        /// </summary>
        public IList<ICSharpParameter> ParameterList { get; }

        /// <summary>
        /// The list of arguments.
        /// </summary>
        public IList<ICSharpArgument> ArgumentList { get; }

        /// <summary>
        /// The number of parameters and arguments.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// The argument passing style.
        /// </summary>
        public TypeArgumentStyles ArgumentStyle { get; }
        #endregion
    }
}
