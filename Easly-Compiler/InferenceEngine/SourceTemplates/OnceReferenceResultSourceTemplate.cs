namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="IOnceReference"/> in a node that can have a result.
    /// </summary>
    public interface IOnceReferenceResultSourceTemplate : ISourceTemplate
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="OnceReference{Tref}"/> in a node that can have a result.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public interface IOnceReferenceResultSourceTemplate<TSource, TRef> : ISourceTemplate<TSource, OnceReference<TRef>>
        where TSource : ISource
        where TRef : class
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="OnceReference{Tref}"/> in a node that can have a result.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public class OnceReferenceResultSourceTemplate<TSource, TRef> : SourceTemplate<TSource, OnceReference<TRef>>, IOnceReferenceResultSourceTemplate<TSource, TRef>, IOnceReferenceResultSourceTemplate
        where TSource : ISource
        where TRef : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="OnceReferenceResultSourceTemplate{TSource, TRef}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        public OnceReferenceResultSourceTemplate(string path)
            : base(path, TemplateResultStart<TSource>.Default)
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

            INodeWithResult EmbeddingNode = StartingPoint.GetStart(node) as INodeWithResult;
            if (EmbeddingNode != null)
            {
                OnceReference<TRef> Value = TemplateHelper.GetPropertyPathValue<INodeWithResult, OnceReference<TRef>>(EmbeddingNode, TemplateNodeStart<INodeWithResult>.Default, PropertyPath, out bool IsInterrupted);
                if (!IsInterrupted && Value.IsAssigned)
                {
                    data = Value.Item;
                    Result = true;
                }
            }
            else
                Result = true;

            return Result;
        }
        #endregion
    }
}
