namespace EaslyCompiler
{
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
        /// Adds a using directive to write separately.
        /// </summary>
        void AddUsing(string usingDirective);
    }
}
