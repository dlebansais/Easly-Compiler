namespace EaslyCompiler
{
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a sealed <see cref="IHashtableEx"/> hash table.
    /// </summary>
    public interface ISealedTableSourceTemplate : ISourceTemplate
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a sealed <see cref="IHashtableEx{TKey, TValue}"/> hash table.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public interface ISealedTableSourceTemplate<TSource, TKey, TValue> : ISourceTemplate<TSource, HashtableEx<TKey, TValue>>
        where TSource : ISource
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a sealed <see cref="IHashtableEx{TKey, TValue}"/> hash table.
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
                data = Value;
                Result = true;
            }

            return Result;
        }
        #endregion
    }
}
