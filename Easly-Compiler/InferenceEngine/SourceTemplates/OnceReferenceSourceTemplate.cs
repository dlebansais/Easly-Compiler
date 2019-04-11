namespace EaslyCompiler
{
#if LATER
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    public interface IOnceReferenceSourceTemplate : ISourceTemplate
    {
        /// <summary>
        /// The reference value if ready.
        /// </summary>
        IOnceReference ReadyReference { get; }
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public interface IOnceReferenceSourceTemplate<TSource, TRef> : ISourceTemplate<TSource, OnceReference<TRef>>
        where TSource : ISource
        where TRef : class
    {
        /// <summary>
        /// The reference value if ready.
        /// </summary>
        OnceReference<TRef> ReadyReference { get; }
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
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

        #region Properties
        /// <summary>
        /// The reference value if ready.
        /// </summary>
        public OnceReference<TRef> ReadyReference { get; private set; }
        IOnceReference IOnceReferenceSourceTemplate.ReadyReference { get { return ReadyReference; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks if a node source is ready.
        /// </summary>
        /// <param name="node">The node for which the value is checked.</param>
        public override bool IsReady(TSource node)
        {
            bool Result = false;

            OnceReference<TRef> Value = GetSourceObject(node);
            if (Value.IsAssigned)
            {
                ReadyReference = Value;
                Result = true;
            }

            return Result;
        }
        #endregion
    }
#endif
}
