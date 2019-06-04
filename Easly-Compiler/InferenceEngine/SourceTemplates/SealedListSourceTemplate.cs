namespace EaslyCompiler
{
    using System.Collections;
    using System.Diagnostics;
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a sealed <see cref="ISealableList"/> list.
    /// </summary>
    public interface ISealedListSourceTemplate : ISourceTemplate
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a sealed <see cref="SealableList{TValue}"/> list.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public interface ISealedListSourceTemplate<TSource, TValue> : ISourceTemplate<TSource, IList>
        where TSource : ISource
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a sealed <see cref="SealableList{TValue}"/> list.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class SealedListSourceTemplate<TSource, TValue> : SourceTemplate<TSource, IList>, ISealedListSourceTemplate<TSource, TValue>, ISealedListSourceTemplate
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SealedListSourceTemplate{TSource, TValue}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public SealedListSourceTemplate(string path, ITemplatePathStart<TSource> startingPoint = null)
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

            SealableList<TValue> Value = GetSourceObject(node, out bool IsInterrupted) as SealableList<TValue>;
            Debug.Assert(Value != null || IsInterrupted);

            if (!IsInterrupted && Value != null && Value.IsSealed)
            {
                data = Value;
                Result = true;
            }

            return Result;
        }
        #endregion
    }
}
