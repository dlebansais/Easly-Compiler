namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# query overload node.
    /// </summary>
    public interface ICSharpQueryOverload : ICSharpSource<IQueryOverload>, ICSharpOverload
    {
        /// <summary>
        /// The list of results.
        /// </summary>
        IList<ICSharpParameter> ResultList { get; }
    }

    /// <summary>
    /// A C# query overload node.
    /// </summary>
    public class CSharpQueryOverload : CSharpSource<IQueryOverload>, ICSharpQueryOverload
    {
        #region Init
        /// <summary>
        /// Create a new C# overload.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="owner">The class where the overload is declared.</param>
        public static ICSharpQueryOverload Create(IQueryOverload source, ICSharpClass owner)
        {
            return new CSharpQueryOverload(source, owner);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpQueryOverload"/> class.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="owner">The class where the overload is declared.</param>
        protected CSharpQueryOverload(IQueryOverload source, ICSharpClass owner)
            : base(source)
        {
            foreach (IParameter Parameter in source.ParameterTable)
            {
                ICSharpParameter NewParameter = CSharpParameter.Create(Parameter, owner);
                ParameterList.Add(NewParameter);
            }

            foreach (IParameter Result in source.ResultTable)
            {
                ICSharpParameter NewResult = CSharpParameter.Create(Result, owner);
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
    }
}
