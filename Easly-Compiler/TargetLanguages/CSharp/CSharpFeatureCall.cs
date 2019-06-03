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
        /// <param name="context">The creation context.</param>
        /// <param name="parameterList">The list of parameters for the selected overload.</param>
        /// <param name="argumentList">The list of arguments.</param>
        /// <param name="argumentStyle">The argument passing style.</param>
        public CSharpFeatureCall(ICSharpContext context, IList<IParameter> parameterList, IList<IArgument> argumentList, TypeArgumentStyles argumentStyle)
        {
            foreach (IParameter Item in parameterList)
            {
                ICSharpClass Owner = context.GetClass(Item.ResolvedParameter.Location.EmbeddingClass);
                ICSharpParameter NewParameter = CSharpParameter.Create(context, Item, Owner);
                ParameterList.Add(NewParameter);
            }

            foreach (IArgument Item in argumentList)
            {
                ICSharpArgument NewArgument = CSharpArgument.Create(context, Item);
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
        public IList<ICSharpParameter> ParameterList { get; } = new List<ICSharpParameter>();

        /// <summary>
        /// The list of arguments.
        /// </summary>
        public IList<ICSharpArgument> ArgumentList { get; } = new List<ICSharpArgument>();

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
