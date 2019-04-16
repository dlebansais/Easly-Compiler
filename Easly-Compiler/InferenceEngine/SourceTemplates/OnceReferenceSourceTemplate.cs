namespace EaslyCompiler
{
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="IOnceReference"/>.
    /// </summary>
    public interface IOnceReferenceSourceTemplate : ISourceTemplate
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="OnceReference{Tref}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public interface IOnceReferenceSourceTemplate<TSource, TRef> : ISourceTemplate<TSource, OnceReference<TRef>>
        where TSource : ISource
        where TRef : class
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="OnceReference{Tref}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public class OnceReferenceSourceTemplate<TSource, TRef> : SourceTemplate<TSource, OnceReference<TRef>>, IOnceReferenceSourceTemplate<TSource, TRef>, IOnceReferenceSourceTemplate
        where TSource : ISource
        where TRef : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="OnceReferenceSourceTemplate{TSource, TRef}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public OnceReferenceSourceTemplate(string path, ITemplatePathStart<TSource> startingPoint = null)
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

            OnceReference<TRef> Value = GetSourceObject(node);
            if (Value.IsAssigned)
            {
                data = Value.Item;
                Result = true;
            }

            return Result;
        }
        #endregion
    }
}
