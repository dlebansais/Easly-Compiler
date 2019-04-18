namespace CompilerNode
{
    /// <summary>
    /// Specialization of <see cref="IPrecursorBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public interface ISetterPrecursorBody : IPrecursorBody
    {
    }

    /// <summary>
    /// Specialization of <see cref="IPrecursorBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public class SetterPrecursorBody : PrecursorBody, ISetterPrecursorBody
    {
    }
}
