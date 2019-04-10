namespace EaslyCompiler
{
    using System;

    /// <summary>
    /// An interface to find the starting point of a source template path.
    /// </summary>
    /// <typeparam name="TSource">The node type for the starting point.</typeparam>
    public interface ITemplatePathStart<TSource>
        where TSource : ISource
    {
        /// <summary>
        /// The type of the starting point.
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// Gets the starting point.
        /// </summary>
        /// <param name="node">The node for which a value is requested.</param>
        ISource GetStart(TSource node);
    }
}
