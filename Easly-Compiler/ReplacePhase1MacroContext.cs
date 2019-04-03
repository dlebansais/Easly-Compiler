namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A context for macro replacement.
    /// </summary>
    public class ReplacePhase1MacroContext
    {
        /// <summary>
        /// The current class.
        /// </summary>
        public IClass CurrentClass { get; set; }
    }
}
