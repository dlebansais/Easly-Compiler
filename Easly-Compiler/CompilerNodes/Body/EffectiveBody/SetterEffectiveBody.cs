namespace CompilerNode
{
    /// <summary>
    /// Specialization of <see cref="IEffectiveBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public interface ISetterEffectiveBody : IEffectiveBody
    {
    }

    /// <summary>
    /// Specialization of <see cref="IEffectiveBody"/> used to ensure a separate static constructor for <see cref="EaslyCompiler.IRuleTemplate"/>.
    /// </summary>
    public class SetterEffectiveBody : EffectiveBody, ISetterEffectiveBody
    {
    }
}
