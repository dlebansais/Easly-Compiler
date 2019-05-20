namespace EaslyCompiler
{
    using System.Diagnostics;

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
            GlobalInstance++;
            ConstantInstance = GlobalInstance;
        }

        private int ConstantInstance;
        private static int GlobalInstance;

        /// <summary>
        /// True if the constant value is known.
        /// </summary>
        public override bool IsValueKnown { get { return true; } }

        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsCompatibleWith(ILanguageConstant other)
        {
            return other is IObjectLanguageConstant AsObjectLanguageConstant && IsValueKnown && AsObjectLanguageConstant.IsValueKnown;
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public override bool IsConstantEqual(ILanguageConstant other)
        {
            return IsConstantEqual(other as ObjectLanguageConstant);
        }

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        protected virtual bool IsConstantEqual(ObjectLanguageConstant other)
        {
            Debug.Assert(other != null);

            return ConstantInstance == other.ConstantInstance;
        }
    }
}
