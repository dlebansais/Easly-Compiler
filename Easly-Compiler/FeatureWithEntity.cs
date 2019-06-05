namespace EaslyCompiler
{
    using System;

    /// <summary>
    /// Features for which one can obtain an entity.
    /// </summary>
    public interface IFeatureWithEntity
    {
        /// <summary>
        /// Guid of the language type corresponding to the entity object for an instance of this class.
        /// </summary>
        Guid EntityGuid { get; }

        /// <summary>
        /// The source node associated to this instance.
        /// </summary>
        ISource Location { get; }
    }
}
