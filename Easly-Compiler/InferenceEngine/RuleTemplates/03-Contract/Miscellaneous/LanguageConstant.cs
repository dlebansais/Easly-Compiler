namespace EaslyCompiler
{
    using BaseNodeHelper;
    using CompilerNode;

    /// <summary>
    /// Represents a specific type of constant number.
    /// </summary>
    public interface ILanguageConstant
    {
        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        bool IsCompatibleWith(ILanguageConstant other);

        /// <summary>
        /// Checks if another constant is greater than this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        bool IsGreater(ILanguageConstant other);
    }

    /// <summary>
    /// Represents a specific type of constant number.
    /// </summary>
    public abstract class LanguageConstant : ILanguageConstant
    {
        #region Implementation
        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public abstract bool IsCompatibleWith(ILanguageConstant other);

        /// <summary>
        /// Checks if another constant is greater than this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public abstract bool IsGreater(ILanguageConstant other);

        /// <summary>
        /// Tries to parse the specified constant as an integer.
        /// </summary>
        /// <param name="constant">The constant to parse.</param>
        /// <param name="value">The integer value upon return if successful.</param>
        /// <returns>True if the constant could be parsed as an integer; Otherwise, false.</returns>
        public static bool TryParseInt(ILanguageConstant constant, out int value)
        {
            value = 0;
            bool Result = false;

            if (constant is IManifestLanguageConstant AsManifestConstant)
            {
                ICanonicalNumber Number = AsManifestConstant.Number;
                Result = Number.TryParseInt(out value);
            }

            return Result;
        }

        /// <summary>
        /// Tries to parse an expression as a constant number.
        /// </summary>
        /// <param name="expression">The expression to parse.</param>
        /// <param name="constant">The parsed constant upon return if successful.</param>
        /// <param name="error">Error found on failure.</param>
        /// <returns>True if the expression could be parsed as a constant; Otherwise, false.</returns>
        public static bool TryParseExpression(IExpression expression, out ILanguageConstant constant, out IError error)
        {
            constant = null;
            error = null;
            bool Result = false;

            if (expression.NumberConstant.IsAssigned)
            {
                constant = expression.NumberConstant.Item;
                Result = true;
            }
            else
                error = new ErrorNumberConstantExpected(expression);

            return Result;
        }
        #endregion
    }
}
