namespace CompilerNode
{
    using System.Collections.Generic;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IGlobalReplicate.
    /// </summary>
    public interface IGlobalReplicate : INode, ICompiledReplicate
    {
        /// <summary>
        /// Gets or sets the patterns to use in the replication.
        /// </summary>
        IList<BaseNode.Pattern> Patterns { get; }
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
