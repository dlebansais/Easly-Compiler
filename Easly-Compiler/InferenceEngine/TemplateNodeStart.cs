namespace EaslyCompiler
{
    using System;

    /// <summary>
    /// The identity starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public interface ITemplateNodeStart<TSource> : ITemplatePathStart<TSource>
        where TSource : ISource
    {
    }

    /// <summary>
    /// The identity starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public class TemplateNodeStart<TSource> : ITemplateNodeStart<TSource>
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// An instance that can be used in any source template.
        /// </summary>
        public static TemplateNodeStart<TSource> Default { get; } = new TemplateNodeStart<TSource>();

        private TemplateNodeStart()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The type of the starting point.
        /// </summary>
        public Type PropertyType { get { return typeof(TSource); } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the starting point.
        /// </summary>
        /// <param name="node">The node for which a value is requested.</param>
        public virtual ISource GetStart(TSource node)
        {
            return node;
        }
        #endregion
    }
}
