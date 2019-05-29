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
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpEqualityExpression Create(IEqualityExpression source, ICSharpContext context)
        {
            return new CSharpEqualityExpression(source, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpEqualityExpression"/> class.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpEqualityExpression(IEqualityExpression source, ICSharpContext context)
            : base(source, context)
        {
            LeftExpression = Create((IExpression)source.LeftExpression, context);
            RightExpression = Create((IExpression)source.RightExpression, context);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IEqualityExpression Source { get { return (IEqualityExpression)base.Source; } }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return true; } }

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
        /// <param name="cSharpNamespace">The current namespace.</param>
        public override string CSharpText(string cSharpNamespace)
        {
            return CSharpText(cSharpNamespace, new List<ICSharpQualifiedName>());
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="destinationList">The list of destinations.</param>
        public virtual string CSharpText(string cSharpNamespace, IList<ICSharpQualifiedName> destinationList)
        {
            string LeftText = NestedExpressionText(LeftExpression, cSharpNamespace);
            string RightText = NestedExpressionText(RightExpression, cSharpNamespace);

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

            return $"{LeftText} {EqualitySign} {RightText}";
        }

        private string NestedExpressionText(ICSharpExpression expression, string cSharpNamespace)
        {
            string Result = expression.CSharpText(cSharpNamespace);

            if (expression.IsComplex)
                Result = $"({Result})";

            return Result;
        }
        #endregion
    }
}
