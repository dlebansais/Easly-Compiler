namespace EaslyCompiler
{
    /// <summary>
    /// Represents an object feature as some constant.
    /// </summary>
    public interface IObjectLanguageConstant : ILanguageConstant
    {
    }

    /// <summary>
    /// Represents an object feature as some constant.
    /// </summary>
    public class ObjectLanguageConstant : LanguageConstant, IObjectLanguageConstant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectLanguageConstant"/> class.
        /// </summary>
        public ObjectLanguageConstant()
        {
        }

        /// <summary>
        /// True if the constant value is known.
        /// </summary>
        public override bool IsValueKnown { get { return false; } }

        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsCompatibleWith(ILanguageConstant other)
        {
            return false;
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsConstantEqual(ILanguageConstant other)
        {
            return false;
        }
    }
}
