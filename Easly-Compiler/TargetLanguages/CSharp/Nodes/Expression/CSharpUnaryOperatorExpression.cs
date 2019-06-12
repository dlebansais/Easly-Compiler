namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpUnaryOperatorExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IUnaryOperatorExpression Source { get; }

        /// <summary>
        /// The right expression.
        /// </summary>
        ICSharpExpression RightExpression { get; }

        /// <summary>
        /// The operator.
        /// </summary>
        ICSharpFunctionFeature Operator { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpUnaryOperatorExpression : CSharpExpression, ICSharpUnaryOperatorExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpUnaryOperatorExpression Create(ICSharpContext context, IUnaryOperatorExpression source)
        {
            return new CSharpUnaryOperatorExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpUnaryOperatorExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpUnaryOperatorExpression(ICSharpContext context, IUnaryOperatorExpression source)
            : base(context, source)
        {
            RightExpression = Create(context, (IExpression)source.RightExpression);

            Operator = context.GetFeature(source.SelectedFeature.Item) as ICSharpFunctionFeature;
            Debug.Assert(Operator != null);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IUnaryOperatorExpression Source { get { return (IUnaryOperatorExpression)base.Source; } }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return true; } }

        /// <summary>
        /// The right expression.
        /// </summary>
        public ICSharpExpression RightExpression { get; }

        /// <summary>
        /// The operator.
        /// </summary>
        public ICSharpFunctionFeature Operator { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection)
        {
            return CSharpText(usingCollection, new List<ICSharpQualifiedName>());
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="destinationList">The list of destinations.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection, IList<ICSharpQualifiedName> destinationList)
        {
            string RightText = NestedExpressionText(usingCollection, RightExpression);
            string OperatorText = Operator.Name;

            return $"{OperatorText} {RightText}";
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
