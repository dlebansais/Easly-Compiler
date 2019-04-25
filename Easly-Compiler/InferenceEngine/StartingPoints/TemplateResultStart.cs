namespace EaslyCompiler
{
    using System;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// The embedding property, indexer or query overload starting point.
    /// </summary>
    public interface ITemplateResultStart : ITemplatePathStart
    {
    }

    /// <summary>
    /// The embedding property, indexer or query overload starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public interface ITemplateResultStart<TSource> : ITemplatePathStart<TSource>
        where TSource : ISource
    {
    }

    /// <summary>
    /// The embedding property, indexer or query overload starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public class TemplateResultStart<TSource> : ITemplateResultStart<TSource>, ITemplateResultStart
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// An instance that can be used in any source template.
        /// </summary>
        public static TemplateResultStart<TSource> Default { get; } = new TemplateResultStart<TSource>();

        private TemplateResultStart()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The type of the starting point.
        /// </summary>
        public Type PropertyType { get { return typeof(INodeWithResult); } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the starting point.
        /// </summary>
        /// <param name="source">The node for which a value is requested.</param>
        public virtual ISource GetStart(TSource source)
        {
            bool IsHandled = false;
            INodeWithResult Result = null;

            if (source.EmbeddingFeature is IPropertyFeature AsPropertyFeature)
            {
                Result = AsPropertyFeature;
                IsHandled = true;
            }
            else if (source.EmbeddingFeature is IIndexerFeature AsIndexerFeature)
            {
                Result = AsIndexerFeature;
                IsHandled = true;
            }
            else if (source.EmbeddingOverload is IQueryOverload AsQueryOverload)
            {
                Result = AsQueryOverload;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);

            return Result;
        }
        #endregion
    }
}
