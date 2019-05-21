namespace EaslyCompiler
{
    /// <summary>
    /// A type, from a <see cref="BaseNode.ObjectType"/> or specific to the compiler, that can be using in a path.
    /// </summary>
    public interface IPathParticipatingType
    {
        /// <summary>
        /// The type to use instead of this type for a source or destination type, for the purpose of path searching.
        /// </summary>
        ICompiledType TypeAsDestinationOrSource { get; }
    }
}
