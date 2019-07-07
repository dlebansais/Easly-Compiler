namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpUnaryNotExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IUnaryNotExpression Source { get; }

        /// <summary>
        /// The right expression.
        /// </summary>
        ICSharpExpression RightExpression { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpUnaryNotExpression : CSharpExpression, ICSharpUnaryNotExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpUnaryNotExpression Create(ICSharpContext context, IUnaryNotExpression source)
        {
            return new CSharpUnaryNotExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpUnaryNotExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpUnaryNotExpression(ICSharpContext context, IUnaryNotExpression source)
            : base(context, source)
        {
            RightExpression = Create(context, (IExpression)source.RightExpression);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IUnaryNotExpression Source { get { return (IUnaryNotExpression)base.Source; } }

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
            //TODO: event expression
            CSharpTextBoolean(writer, expressionContext);
        }

        private void CSharpTextBoolean(ICSharpWriter writer, ICSharpExpressionContext expressionContext)
        {
            string RightText = NestedExpressionText(writer, RightExpression);
            string OperatorName = "!";

            expressionContext.SetSingleReturnValue($"{OperatorName}{RightText}");
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
