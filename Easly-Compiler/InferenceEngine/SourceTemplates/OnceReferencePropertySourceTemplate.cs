namespace EaslyCompiler
{
    using CompilerNode;
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="IOnceReference"/> in a property.
    /// </summary>
    public interface IOnceReferencePropertySourceTemplate : ISourceTemplate
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="OnceReference{Tref}"/> in a property.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public interface IOnceReferencePropertySourceTemplate<TSource, TRef> : ISourceTemplate<TSource, OnceReference<TRef>>
        where TSource : ISource
        where TRef : class
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="OnceReference{Tref}"/> in a property.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public class OnceReferencePropertySourceTemplate<TSource, TRef> : SourceTemplate<TSource, OnceReference<TRef>>, IOnceReferencePropertySourceTemplate<TSource, TRef>, IOnceReferencePropertySourceTemplate
        where TSource : ISource
        where TRef : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="OnceReferencePropertySourceTemplate{TSource, TRef}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        public OnceReferencePropertySourceTemplate(string path)
            : base(path, TemplatePropertyStart<TSource>.Default)
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

            if (node.EmbeddingFeature is IPropertyFeature AsPropertyFeature)
            {
                OnceReference<TRef> Value = TemplateHelper.GetPropertyPathValue<IPropertyFeature, OnceReference<TRef>>(AsPropertyFeature, TemplateNodeStart<IPropertyFeature>.Default, PropertyPath, out bool IsInterrupted);
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
