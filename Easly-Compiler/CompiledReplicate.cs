namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A replicate, from a <see cref="BaseNode.ClassReplicate"/> or <see cref="BaseNode.GlobalReplicate"/>.
    /// </summary>
    public interface ICompiledReplicate
    {
        /// <summary>
        /// Replicate name (from BaseNode).
        /// </summary>
        BaseNode.IName ReplicateName { get; }

        /// <summary>
        /// Processed list of patterns.
        /// </summary>
        IList<IPattern> PatternList { get; }
    }
}
