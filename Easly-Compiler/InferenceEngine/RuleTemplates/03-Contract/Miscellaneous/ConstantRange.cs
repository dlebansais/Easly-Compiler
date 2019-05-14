namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// The range between two constants.
    /// </summary>
    public interface IConstantRange
    {
        /// <summary>
        /// The minimum value in the range.
        /// </summary>
        IOrderedLanguageConstant Minimum { get; }

        /// <summary>
        /// The maximum value in the range.
        /// </summary>
        IOrderedLanguageConstant Maximum { get; }

        /// <summary>
        /// Checks if two ranges intersect.
        /// </summary>
        /// <param name="other">The other range.</param>
        bool IsIntersecting(IConstantRange other);
    }

    /// <summary>
    /// The range between two constants.
    /// </summary>
    public class ConstantRange : IConstantRange
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantRange"/> class.
        /// </summary>
        /// <param name="minimum">The minimum value in the range.</param>
        /// <param name="maximum">The maximum value in the range.</param>
        public ConstantRange(IOrderedLanguageConstant minimum, IOrderedLanguageConstant maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The minimum value in the range.
        /// </summary>
        public IOrderedLanguageConstant Minimum { get; }

        /// <summary>
        /// The maximum value in the range.
        /// </summary>
        public IOrderedLanguageConstant Maximum { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Tries to parse a range node to obtain the corresponding range of constants.
        /// </summary>
        /// <param name="range">The range to parse.</param>
        /// <param name="result">The constant range upon return if successful.</param>
        /// <param name="error">The error in case of failure.</param>
        public static bool TryParseRange(IRange range, out IConstantRange result, out IError error)
        {
            result = null;

            IOrderedLanguageConstant LeftExpressionConstant;
            IOrderedLanguageConstant RightExpressionConstant;

            if (!LanguageConstant.TryParseExpression((IExpression)range.LeftExpression, out LeftExpressionConstant, out error))
                return false;

            if (!range.RightExpression.IsAssigned)
                RightExpressionConstant = LeftExpressionConstant;
            else if (!LanguageConstant.TryParseExpression((IExpression)range.RightExpression.Item, out RightExpressionConstant, out error))
                return false;

            if (!LeftExpressionConstant.IsCompatibleWith(RightExpressionConstant))
            {
                error = new ErrorIncompatibleRangeBounds(range);
                return false;
            }

            result = new ConstantRange(LeftExpressionConstant, RightExpressionConstant);
            error = null;
            return true;
        }

        /// <summary>
        /// Checks if two ranges intersect.
        /// </summary>
        /// <param name="other">The other range.</param>
        public bool IsIntersecting(IConstantRange other)
        {
            if (!Minimum.IsCompatibleWith(other.Maximum) || !other.Minimum.IsCompatibleWith(Maximum))
                return false;

            if (Minimum.IsConstantGreater(other.Maximum) || other.Minimum.IsConstantGreater(Maximum))
                return false;

            return true;
        }
        #endregion
    }
}
