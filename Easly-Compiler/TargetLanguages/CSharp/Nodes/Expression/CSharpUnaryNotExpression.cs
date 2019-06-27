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

        /// <summary>
        /// True if the condition is on events.
        /// </summary>
        bool IsEventExpression { get; }
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

        /// <summary>
        /// True if the condition is on events.
        /// </summary>
        public bool IsEventExpression { get { return Source.IsEventExpression; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection)
        {
            return CSharpText(usingCollection, false, false, new List<ICSharpQualifiedName>(), -1);
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="destinationList">The list of destinations.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection, bool isNeverSimple, bool isDeclaredInPlace, IList<ICSharpQualifiedName> destinationList, int skippedIndex)
        {
            //TODO: event expression
            return CSharpTextBoolean(usingCollection, destinationList);
        }

        private string CSharpTextBoolean(ICSharpUsingCollection usingCollection, IList<ICSharpQualifiedName> destinationList)
        {
            string RightText = NestedExpressionText(usingCollection, RightExpression);
            string OperatorName = "!";

            return $"{OperatorName}{RightText}";
        }

        private string NestedExpressionText(ICSharpUsingCollection usingCollection, ICSharpExpression expression)
        {
            string Result = expression.CSharpText(usingCollection);

            if (expression.IsComplex)
                Result = $"({Result})";

            return Result;
        }
        #endregion
    }
}
