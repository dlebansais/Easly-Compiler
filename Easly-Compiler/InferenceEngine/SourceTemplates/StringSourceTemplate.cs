namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    public interface IStringSourceTemplate : ISourceTemplate
    {
        /// <summary>
        /// The string value if ready.
        /// </summary>
        string ReadyString { get; }
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TNode">The node type on which the rule applies.</typeparam>
    public interface IStringSourceTemplate<TNode> : ISourceTemplate<TNode, string>
        where TNode : INode
    {
        /// <summary>
        /// The string value if ready.
        /// </summary>
        string ReadyString { get; }
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TNode">The node type on which the rule applies.</typeparam>
    public class StringSourceTemplate<TNode> : SourceTemplate<TNode, string>, IStringSourceTemplate<TNode>, IStringSourceTemplate
        where TNode : INode
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="StringSourceTemplate{TNode}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public StringSourceTemplate(string path, ITemplatePathStart startingPoint = null)
            : base(path, startingPoint)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The string value if ready.
        /// </summary>
        public string ReadyString { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks if a node source is ready.
        /// </summary>
        /// <param name="node">The node for which the value is checked.</param>
        public override bool IsReady(TNode node)
        {
            bool Result = false;

            string Value = GetSourceObject(node);
            if (Value != null)
            {
                ReadyString = Value;
                Result = true;
            }

            return Result;
        }
        #endregion
    }
}
