namespace EaslyCompiler
{
    using System.Collections.Generic;

    /// <summary>
    /// Context use when walking the node tree to replicate blocks.
    /// </summary>
    public class ReplicationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReplicationContext"/> class.
        /// </summary>
        public ReplicationContext()
        {
            ReplicateTable = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// A table of subsitutions.
        /// </summary>
        public Dictionary<string, List<string>> ReplicateTable { get; }

        /// <summary>
        /// A table of pattern association. The key is the text to replace, the value the replacement.
        /// </summary>
        public IDictionary<string, string> PatternTable { get; } = new Dictionary<string, string>();
    }
}
