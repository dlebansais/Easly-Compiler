namespace EaslyCompiler
{
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    public interface ISealedTableSourceTemplate : ISourceTemplate
    {
        /// <summary>
        /// The sealed table value if ready.
        /// </summary>
        IHashtableEx ReadyTable { get; }
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public interface ISealedTableSourceTemplate<TSource, TKey, TValue> : ISourceTemplate<TSource, HashtableEx<TKey, TValue>>
        where TSource : ISource
    {
        /// <summary>
        /// The reference value if ready.
        /// </summary>
        IHashtableEx<TKey, TValue> ReadyTable { get; }
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class SealedTableSourceTemplate<TSource, TKey, TValue> : SourceTemplate<TSource, HashtableEx<TKey, TValue>>, ISealedTableSourceTemplate<TSource, TKey, TValue>, ISealedTableSourceTemplate
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SealedTableSourceTemplate{TSource, TKey, TValue}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public SealedTableSourceTemplate(string path, ITemplatePathStart<TSource> startingPoint = null)
            : base(path, startingPoint)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The reference value if ready.
        /// </summary>
        public IHashtableEx<TKey, TValue> ReadyTable { get; private set; }
        IHashtableEx ISealedTableSourceTemplate.ReadyTable { get { return ReadyTable; } }
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

            IHashtableEx<TKey, TValue> Value = GetSourceObject(node);
            if (Value.IsSealed)
            {
                ReadyTable = Value;
                Result = true;
            }

            return Result;
        }
        #endregion
    }
}
