namespace EaslyCompiler
{
    using System.Collections;
    using System.Collections.Generic;
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a list of objects with an assigned <see cref="OnceReference{Tref}"/>.
    /// </summary>
    public interface IOnceReferenceSourceListTemplate : ISourceTemplate
    {
        /// <summary>
        /// The reference value if ready.
        /// </summary>
        IList ReadyReferenceList { get; }
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a list of objects with an assigned <see cref="OnceReference{Tref}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public interface IOnceReferenceSourceListTemplate<TSource, TRef> : ISourceTemplate<TSource, IList<OnceReference<TRef>>>
        where TSource : ISource
        where TRef : class
    {
        /// <summary>
        /// The reference value if ready.
        /// </summary>
        IList<OnceReference<TRef>> ReadyReferenceList { get; }
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a list of objects with an assigned <see cref="OnceReference{Tref}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public class OnceReferenceSourceListTemplate<TSource, TRef> : SourceTemplate<TSource, IList<OnceReference<TRef>>>, IOnceReferenceSourceListTemplate<TSource, TRef>, IOnceReferenceSourceListTemplate
        where TSource : ISource
        where TRef : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="OnceReferenceSourceListTemplate{TSource, TRef}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public OnceReferenceSourceListTemplate(string path, string propertyName, ITemplatePathStart<TSource> startingPoint = null)
            : base(path, startingPoint)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The reference value if ready.
        /// </summary>
        public IList<OnceReference<TRef>> ReadyReferenceList { get; private set; }
        IList IOnceReferenceSourceListTemplate.ReadyReferenceList { get { return (IList)ReadyReferenceList; } }
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
            bool Result = true;

            IList<OnceReference<TRef>> Value = GetSourceObject(node);

            foreach (OnceReference<TRef> Reference in Value)
                if (!Reference.IsAssigned)
                {
                    Result = false;
                    break;
                }

            if (Result)
                ReadyReferenceList = Value;

            return Result;
        }
        #endregion
    }
}
