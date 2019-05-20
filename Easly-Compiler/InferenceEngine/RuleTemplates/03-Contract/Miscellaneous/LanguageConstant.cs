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
        /// True if the constant value is known.
        /// </summary>
        bool IsValueKnown { get; }

        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        bool IsCompatibleWith(ILanguageConstant other);

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        bool IsConstantEqual(ILanguageConstant other);
    }

    /// <summary>
    /// Represents a specific type of constant number.
    /// </summary>
    public abstract class LanguageConstant : ILanguageConstant
    {
        #region Properties
        /// <summary>
        /// True if the constant value is known.
        /// </summary>
        public abstract bool IsValueKnown { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks if another constant can be compared with this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public abstract bool IsCompatibleWith(ILanguageConstant other);

        /// <summary>
        /// Checks if another constant is equal to this instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public abstract bool IsConstantEqual(ILanguageConstant other);

        /// <summary>
        /// Tries to parse the specified constant as an integer.
        /// </summary>
        /// <param name="constant">The constant to parse.</param>
        /// <param name="value">The integer value upon return if successful.</param>
        /// <returns>True if the constant could be parsed as an integer; Otherwise, false.</returns>
        /*public static bool TryParseInt(ILanguageConstant constant, out int value)
        {
            value = 0;
            bool Result = false;

            if (constant is INumberLanguageConstant AsNumberLanguageConstant)
            {
                ICanonicalNumber Value = AsNumberLanguageConstant.Value;
                Result = Value.TryParseInt(out value);
            }

            return Result;
        }*/

        /// <summary>
        /// Tries to parse an expression as a constant number.
        /// </summary>
        /// <param name="expression">The expression to parse.</param>
        /// <param name="expressionConstant">The parsed constant upon return if successful.</param>
        /// <param name="error">Error found on failure.</param>
        /// <returns>True if the expression could be parsed as a constant; Otherwise, false.</returns>
        public static bool TryParseExpression(IExpression expression, out IOrderedLanguageConstant expressionConstant, out IError error)
        {
            expressionConstant = null;
            error = null;
            bool Result = false;

            if (expression.ExpressionConstant.IsAssigned && expression.ExpressionConstant.Item is IOrderedLanguageConstant AsOrderedLanguageConstant)
            {
                expressionConstant = AsOrderedLanguageConstant;
                Result = true;
            }
            else
                error = new ErrorNumberConstantExpected(expression);

            return Result;
        }
        #endregion
    }
}
