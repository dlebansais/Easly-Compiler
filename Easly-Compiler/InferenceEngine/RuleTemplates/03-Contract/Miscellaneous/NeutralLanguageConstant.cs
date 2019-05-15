namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A singleton constant to use as a null expression constant.
    /// </summary>
    public class NeutralLanguageConstant : LanguageConstant
    {
        /// <summary>
        /// Static property that represents the null value for an expression constant.
        /// </summary>
        public static ILanguageConstant NotConstant { get; } = new NeutralLanguageConstant();

        /// <summary>
        /// Initializes a new instance of the <see cref="NeutralLanguageConstant"/> class.
        /// </summary>
        private NeutralLanguageConstant()
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
