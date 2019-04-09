namespace EaslyCompiler
{
    using System;
    using BaseNode;

    /// <summary>
    /// An interface to find the starting point of a source template path.
    /// </summary>
    public interface ITemplatePathStart
    {
        /// <summary>
        /// The type of the starting point.
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// Gets the starting point.
        /// </summary>
        /// <param name="node">The node for which a value is requested.</param>
        object GetStart(INode node);
    }
}
