namespace CompilerNode
{
    /// <summary>
    /// Specialization of <see cref="IDeferredBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public interface IOverloadDeferredBody : IDeferredBody
    {
    }

    /// <summary>
    /// Specialization of <see cref="IDeferredBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public class OverloadDeferredBody : DeferredBody, IOverloadDeferredBody
    {
    }
}
