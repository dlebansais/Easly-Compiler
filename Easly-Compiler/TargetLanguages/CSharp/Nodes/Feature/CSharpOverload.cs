namespace EaslyCompiler
{
    using System.Collections.Generic;

    /// <summary>
    /// A C# overload (query or command).
    /// </summary>
    public interface ICSharpOverload
    {
        /// <summary>
        /// The list of parameters.
        /// </summary>
        IList<ICSharpParameter> ParameterList { get; }
    }
}
