namespace EaslyCompiler
{
    using System;
    using CompilerNode;

    /// <summary>
    /// The embedding class starting point.
    /// </summary>
    public interface ITemplatePropertyStart : ITemplatePathStart
    {
    }

    /// <summary>
    /// The embedding class starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public interface ITemplatePropertyStart<TSource> : ITemplatePathStart<TSource>
        where TSource : ISource
    {
    }

    /// <summary>
    /// The embedding class starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public class TemplatePropertyStart<TSource> : ITemplatePropertyStart<TSource>, ITemplatePropertyStart
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// An instance that can be used in any source template.
        /// </summary>
        public static TemplatePropertyStart<TSource> Default { get; } = new TemplatePropertyStart<TSource>();

        private TemplatePropertyStart()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The type of the starting point.
        /// </summary>
        public Type PropertyType { get { return typeof(IPropertyFeature); } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the starting point.
        /// </summary>
        /// <param name="source">The node for which a value is requested.</param>
        public virtual ISource GetStart(TSource source)
        {
            return source.EmbeddingFeature as IPropertyFeature;
        }
        #endregion
    }
}
