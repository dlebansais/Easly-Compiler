namespace CompilerNode
{
    /// <summary>
    /// Specialization of <see cref="IExternBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public interface ISetterExternBody : IExternBody
    {
    }

    /// <summary>
    /// Specialization of <see cref="IExternBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public class SetterExternBody : ExternBody, ISetterExternBody
    {
    }
}
