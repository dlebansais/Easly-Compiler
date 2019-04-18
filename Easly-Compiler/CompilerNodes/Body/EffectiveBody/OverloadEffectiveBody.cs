namespace CompilerNode
{
    /// <summary>
    /// Specialization of <see cref="IEffectiveBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public interface IOverloadEffectiveBody : IEffectiveBody
    {
    }

    /// <summary>
    /// Specialization of <see cref="IEffectiveBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public class OverloadEffectiveBody : EffectiveBody, IOverloadEffectiveBody
    {
    }
}
