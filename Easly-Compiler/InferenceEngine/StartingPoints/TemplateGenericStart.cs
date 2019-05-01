namespace EaslyCompiler
{
    using System;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// The embedding generic starting point.
    /// </summary>
    public interface ITemplateGenericStart : ITemplatePathStart
    {
    }

    /// <summary>
    /// The embedding generic starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public interface ITemplateGenericStart<TSource> : ITemplatePathStart<TSource>
        where TSource : ISource
    {
    }

    /// <summary>
    /// The embedding generic starting point.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public class TemplateGenericStart<TSource> : ITemplateGenericStart<TSource>, ITemplateGenericStart
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// An instance that can be used in any source template.
        /// </summary>
        public static TemplateGenericStart<TSource> Default { get; } = new TemplateGenericStart<TSource>();

        private TemplateGenericStart()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The type of the starting point.
        /// </summary>
        public Type PropertyType { get { return typeof(IGeneric); } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the starting point.
        /// </summary>
        /// <param name="source">The node for which a value is requested.</param>
        public virtual ISource GetStart(TSource source)
        {
            IConstraint ConstraintSource = source as IConstraint;
            Debug.Assert(ConstraintSource != null);

            IGeneric ParentGeneric = ConstraintSource.ParentSource as IGeneric;
            Debug.Assert(ParentGeneric != null);
            Debug.Assert(ParentGeneric.ConstraintList.Contains(ConstraintSource));

            return ParentGeneric;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Returns a string representing this instance.
        /// </summary>
        public override string ToString()
        {
            return "/generic";
        }
        #endregion
    }
}
