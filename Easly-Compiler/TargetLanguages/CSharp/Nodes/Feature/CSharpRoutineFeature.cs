namespace EaslyCompiler
{
    using System.Collections.Generic;

    /// <summary>
    /// A C# routine.
    /// </summary>
    public interface ICSharpRoutineFeature : ICSharpFeatureWithName
    {
        /// <summary>
        /// The list of overloads.
        /// </summary>
        IList<ICSharpOverload> OverloadList { get; }
    }
}
