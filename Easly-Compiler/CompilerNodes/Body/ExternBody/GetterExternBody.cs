namespace CompilerNode
{
    /// <summary>
    /// Specialization of <see cref="IExternBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public interface IGetterExternBody : IExternBody
    {
    }

    /// <summary>
    /// Specialization of <see cref="IExternBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public class GetterExternBody : ExternBody, IGetterExternBody
    {
    }
}
