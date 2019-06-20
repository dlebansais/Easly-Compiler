namespace EaslyCompiler
{
    using System;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// The embedding scope starting point.
    /// </summary>
    public interface ITemplateScopeStart : ITemplatePathStart
    {
    }

    /// <summary>
    /// The embedding scope starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public interface ITemplateScopeStart<TSource> : ITemplatePathStart<TSource>
        where TSource : ISource
    {
    }

    /// <summary>
    /// The embedding scope starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public class TemplateScopeStart<TSource> : ITemplateScopeStart<TSource>, ITemplateScopeStart
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// An instance that can be used in any source template.
        /// </summary>
        public static TemplateScopeStart<TSource> Default { get; } = new TemplateScopeStart<TSource>();

        private TemplateScopeStart()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The type of the starting point.
        /// </summary>
        public Type PropertyType { get { return typeof(IScope); } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the starting point.
        /// </summary>
        /// <param name="source">The node for which a value is requested.</param>
        public virtual ISource GetStart(TSource source)
        {
            ISource ScopeSource = source;
            while (!Scope.IsScopeHolder(ScopeSource) && ScopeSource.ParentSource != null)
                ScopeSource = ScopeSource.ParentSource;

            Debug.Assert(ScopeSource is IScopeHolder);

            return ScopeSource;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Returns a string representing this instance.
        /// </summary>
        public override string ToString()
        {
            return "/scope";
        }
        #endregion
    }
}
