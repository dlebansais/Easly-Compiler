namespace EaslyCompiler
{
    using System;
    using CompilerNode;

    /// <summary>
    /// The embedding class starting point.
    /// </summary>
    public interface ITemplateClassStart : ITemplatePathStart
    {
    }

    /// <summary>
    /// The embedding class starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public interface ITemplateClassStart<TSource> : ITemplatePathStart<TSource>
        where TSource : ISource
    {
    }

    /// <summary>
    /// The embedding class starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public class TemplateClassStart<TSource> : ITemplateClassStart<TSource>, ITemplateClassStart
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// An instance that can be used in any source template.
        /// </summary>
        public static TemplateClassStart<TSource> Default { get; } = new TemplateClassStart<TSource>();

        private TemplateClassStart()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The type of the starting point.
        /// </summary>
        public Type PropertyType { get { return typeof(IClass); } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the starting point.
        /// </summary>
        /// <param name="source">The node for which a value is requested.</param>
        public virtual ISource GetStart(TSource source)
        {
            return source.EmbeddingClass;
        }
        #endregion
    }
}
