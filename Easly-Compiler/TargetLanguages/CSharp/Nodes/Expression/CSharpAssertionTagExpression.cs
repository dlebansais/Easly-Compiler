namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpAssertionTagExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IAssertionTagExpression Source { get; }

        /// <summary>
        /// The target expression.
        /// </summary>
        ICSharpExpression BooleanExpression { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpAssertionTagExpression : CSharpExpression, ICSharpAssertionTagExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpAssertionTagExpression Create(ICSharpContext context, IAssertionTagExpression source)
        {
            return new CSharpAssertionTagExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAssertionTagExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpAssertionTagExpression(ICSharpContext context, IAssertionTagExpression source)
            : base(context, source)
        {
            BooleanExpression = Create(context, source.ResolvedBooleanExpression.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IAssertionTagExpression Source { get { return (IAssertionTagExpression)base.Source; } }

        /// <summary>
        /// The target expression.
        /// </summary>
        public ICSharpExpression BooleanExpression { get; }
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
            BooleanExpression.WriteCSharp(writer, expressionContext, isNeverSimple, isDeclaredInPlace, skippedIndex);
        }
        #endregion
    }
}
