namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A context for source initialization.
    /// </summary>
    public class InitializeSourceContext
    {
        /// <summary>
        /// The parent source.
        /// </summary>
        public ISource ParentSource { get; set; }

        /// <summary>
        /// The current class.
        /// </summary>
        public IClass CurrentClass { get; set; }
    }
}
