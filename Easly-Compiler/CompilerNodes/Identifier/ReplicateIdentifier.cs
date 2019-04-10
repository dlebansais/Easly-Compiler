namespace CompilerNode
{
    /// <summary>
    /// Specialization of <see cref="IIdentifier"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public interface IReplicateIdentifier : IIdentifier
    {
    }

    /// <summary>
    /// Specialization of <see cref="IIdentifier"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public class ReplicateIdentifier : Identifier, IReplicateIdentifier
    {
    }
}
