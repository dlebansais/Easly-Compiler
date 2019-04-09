namespace EaslyCompiler
{
    using System;
    using BaseNode;

    /// <summary>
    /// The identity starting point.
    /// </summary>
    public interface ITemplateNodeStart : ITemplatePathStart
    {
    }

    /// <summary>
    /// The identity starting point.
    /// </summary>
    public class TemplateNodeStart : ITemplateNodeStart
    {
        #region Init
        /// <summary>
        /// An instance that can be used in any source template.
        /// </summary>
        public static TemplateNodeStart Default { get; } = new TemplateNodeStart();

        private TemplateNodeStart()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The type of the starting point.
        /// </summary>
        public Type PropertyType { get { return typeof(INode); } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the starting point.
        /// </summary>
        /// <param name="node">The node for which a value is requested.</param>
        public virtual object GetStart(INode node)
        {
            return node;
        }
        #endregion
    }
}
