namespace EaslyCompiler
{
    using System;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// The embedding generic type starting point.
    /// </summary>
    public interface ITemplateGenericTypeStart : ITemplatePathStart
    {
    }

    /// <summary>
    /// The embedding generic type starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public interface ITemplateGenericTypeStart<TSource> : ITemplatePathStart<TSource>
        where TSource : ISource
    {
    }

    /// <summary>
    /// The embedding generic type starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public class TemplateGenericTypeStart<TSource> : ITemplateGenericTypeStart<TSource>, ITemplateGenericTypeStart
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// An instance that can be used in any source template.
        /// </summary>
        public static TemplateGenericTypeStart<TSource> Default { get; } = new TemplateGenericTypeStart<TSource>();

        private TemplateGenericTypeStart()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The type of the starting point.
        /// </summary>
        public Type PropertyType { get { return typeof(IGenericType); } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the starting point.
        /// </summary>
        /// <param name="source">The node for which a value is requested.</param>
        public virtual ISource GetStart(TSource source)
        {
            ITypeArgument TypeArgumentSource = source as ITypeArgument;
            Debug.Assert(TypeArgumentSource != null);

            IGenericType ParentGeneric = TypeArgumentSource.ParentSource as IGenericType;
            Debug.Assert(ParentGeneric != null);
            Debug.Assert(ParentGeneric.TypeArgumentList.Contains(TypeArgumentSource));

            return ParentGeneric;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Returns a string representing this instance.
        /// </summary>
        public override string ToString()
        {
            return "/generic type";
        }
        #endregion
    }
}
