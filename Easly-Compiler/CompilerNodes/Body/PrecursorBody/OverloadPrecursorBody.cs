namespace CompilerNode
{
    /// <summary>
    /// Specialization of <see cref="IPrecursorBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public interface IOverloadPrecursorBody : IPrecursorBody
    {
    }

    /// <summary>
    /// Specialization of <see cref="IPrecursorBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public class OverloadPrecursorBody : PrecursorBody, IOverloadPrecursorBody
    {
    }
}
