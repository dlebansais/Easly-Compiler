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
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    public interface IStringSourceTemplate<TSource> : ISourceTemplate<TSource, string>
        where TSource : ISource
    {
        /// <summary>
        /// The string value if ready.
        /// </summary>
        string ReadyString { get; }
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    public class StringSourceTemplate<TSource> : SourceTemplate<TSource, string>, IStringSourceTemplate<TSource>, IStringSourceTemplate
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="StringSourceTemplate{TSource}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public StringSourceTemplate(string path, ITemplatePathStart<TSource> startingPoint = null)
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
        /// <param name="data">Optional data returned to the caller.</param>
        public override bool IsReady(TSource node, out object data)
        {
            data = null;
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
