namespace EaslyCompiler
{
    /// <summary>
    /// A body, from a <see cref="BaseNode.Body"/> or specific to the compiler.
    /// </summary>
    public interface ICompiledBody
    {
        /// <summary>
        /// Indicates if the body is deferred in another class.
        /// </summary>
        bool IsDeferredBody { get; }
    }
}
