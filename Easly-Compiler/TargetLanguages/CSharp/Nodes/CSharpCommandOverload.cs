namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# command overload node.
    /// </summary>
    public interface ICSharpCommandOverload : ICSharpSource<ICommandOverload>, ICSharpOverload
    {
    }

    /// <summary>
    /// A C# command overload node.
    /// </summary>
    public class CSharpCommandOverload : CSharpSource<ICommandOverload>, ICSharpCommandOverload
    {
        #region Init
        /// <summary>
        /// Create a new C# overload.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="owner">The class where the overload is declared.</param>
        public static ICSharpCommandOverload Create(ICSharpContext context, ICommandOverload source, ICSharpClass owner)
        {
            return new CSharpCommandOverload(context, source, owner);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCommandOverload"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="owner">The class where the overload is declared.</param>
        protected CSharpCommandOverload(ICSharpContext context, ICommandOverload source, ICSharpClass owner)
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
    }
}
