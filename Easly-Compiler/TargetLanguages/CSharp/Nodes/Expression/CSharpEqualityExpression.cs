namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpEqualityExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IEqualityExpression Source { get; }

        /// <summary>
        /// The left expression.
        /// </summary>
        ICSharpExpression LeftExpression { get; }

        /// <summary>
        /// The right expression.
        /// </summary>
        ICSharpExpression RightExpression { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpEqualityExpression : CSharpExpression, ICSharpEqualityExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpEqualityExpression Create(ICSharpContext context, IEqualityExpression source)
        {
            return new CSharpEqualityExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpEqualityExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpEqualityExpression(ICSharpContext context, IEqualityExpression source)
            : base(context, source)
        {
            LeftExpression = Create(context, (IExpression)source.LeftExpression);
            RightExpression = Create(context, (IExpression)source.RightExpression);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IEqualityExpression Source { get { return (IEqualityExpression)base.Source; } }

        /// <summary>
        /// The left expression.
        /// </summary>
        public ICSharpExpression LeftExpression { get; }

        /// <summary>
        /// The right expression.
        /// </summary>
        public ICSharpExpression RightExpression { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expressionContext">The context.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, bool isNeverSimple, bool isDeclaredInPlace, int skippedIndex)
        {
            string LeftText = NestedExpressionText(writer, LeftExpression);
            string RightText = NestedExpressionText(writer, RightExpression);

            string EqualitySign = null;

            switch (Source.Comparison)
            {
                case BaseNode.ComparisonType.Equal:
                    EqualitySign = "==";
                    break;

                case BaseNode.ComparisonType.Different:
                    EqualitySign = "!=";
                    break;
            }

            Debug.Assert(EqualitySign != null);

            expressionContext.SetSingleReturnValue($"{LeftText} {EqualitySign} {RightText}");
        }

        private string NestedExpressionText(ICSharpWriter writer, ICSharpExpression expression)
        {
            ICSharpExpressionContext SourceExpressionContext = new CSharpExpressionContext();
            expression.WriteCSharp(writer, SourceExpressionContext, false, false, -1);

            string Result = SourceExpressionContext.ReturnValue;

            if (expression.IsComplex)
                Result = $"({Result})";

            return Result;
        }
        #endregion
    }
}
