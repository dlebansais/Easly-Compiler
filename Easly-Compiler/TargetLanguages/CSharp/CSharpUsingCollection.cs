namespace EaslyCompiler
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface to add using directives to a collection.
    /// </summary>
    public interface ICSharpUsingCollection
    {
        /// <summary>
        /// Gets the default namespace.
        /// </summary>
        string DefaultNamespace { get; }

        /// <summary>
        /// Map of attached variable names.
        /// </summary>
        IDictionary<string, string> AttachmentMap { get; }

        /// <summary>
        /// Adds a using directive to write separately.
        /// </summary>
        void AddUsing(string usingDirective);

        /// <summary>
        /// Adds a name and its corresponding attached name to the attachment map.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="nameAttached">The corresponding attached name.</param>
        void AddAttachment(string name, string nameAttached);

        /// <summary>
        /// Removes a name and its corresponding attached name from the attachment map.
        /// </summary>
        /// <param name="name">The name.</param>
        void RemoveAttachment(string name);
    }
}
