namespace CompilerNode
{
    /// <summary>
    /// Specialization of <see cref="IDeferredBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public interface IGetterDeferredBody : IDeferredBody
    {
    }

    /// <summary>
    /// Specialization of <see cref="IDeferredBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public class GetterDeferredBody : DeferredBody, IGetterDeferredBody
    {
    }
}
