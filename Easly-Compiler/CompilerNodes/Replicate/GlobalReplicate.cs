namespace CompilerNode
{
    using System.Collections.Generic;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IGlobalReplicate.
    /// </summary>
    public interface IGlobalReplicate : BaseNode.IGlobalReplicate, INode, ICompiledReplicate
    {
    }

    /// <summary>
    /// Compiler IGlobalReplicate.
    /// </summary>
    public class GlobalReplicate : BaseNode.GlobalReplicate, IGlobalReplicate
    {
        #region Implementation of ICompiledReplicate
        /// <summary>
        /// Processed list of patterns.
        /// </summary>
        public IList<IPattern> PatternList { get; } = new List<IPattern>();
        #endregion
    }
}
