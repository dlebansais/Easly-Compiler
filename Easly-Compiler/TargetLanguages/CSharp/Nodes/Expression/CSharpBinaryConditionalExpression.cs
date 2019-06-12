namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpBinaryConditionalExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IBinaryConditionalExpression Source { get; }

        /// <summary>
        /// The left expression.
        /// </summary>
        ICSharpExpression LeftExpression { get; }

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
    public class CSharpBinaryConditionalExpression : CSharpExpression, ICSharpBinaryConditionalExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpBinaryConditionalExpression Create(ICSharpContext context, IBinaryConditionalExpression source)
        {
            return new CSharpBinaryConditionalExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpBinaryConditionalExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpBinaryConditionalExpression(ICSharpContext context, IBinaryConditionalExpression source)
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
        public new IBinaryConditionalExpression Source { get { return (IBinaryConditionalExpression)base.Source; } }

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
            return CSharpText(usingCollection, new List<ICSharpQualifiedName>());
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="destinationList">The list of destinations.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection, IList<ICSharpQualifiedName> destinationList)
        {
            if (IsEventExpression)
                return CSharpTextEvent(usingCollection, destinationList);
            else
                return CSharpTextBoolean(usingCollection, destinationList);
        }

        private string CSharpTextEvent(ICSharpUsingCollection usingCollection, IList<ICSharpQualifiedName> destinationList)
        {
            string LeftText = LeftExpression.CSharpText(usingCollection);
            string RightText = RightExpression.CSharpText(usingCollection);

            string WaitName = null;

            switch (Source.Conditional)
            {
                case BaseNode.ConditionalTypes.And:
                    WaitName = "WaitAll";
                    break;

                case BaseNode.ConditionalTypes.Or:
                    WaitName = "WaitAny";
                    break;
            }

            Debug.Assert(WaitName != null);

            return $"Event.{WaitName}(new Event[] {{ {LeftText}, {RightText} }})";
        }

        private string CSharpTextBoolean(ICSharpUsingCollection usingCollection, IList<ICSharpQualifiedName> destinationList)
        {
            string LeftText = NestedExpressionText(usingCollection, LeftExpression);
            string RightText = NestedExpressionText(usingCollection, RightExpression);

            string OperatorName = null;

            switch (Source.Conditional)
            {
                case BaseNode.ConditionalTypes.And:
                    OperatorName = "&&";
                    break;

                case BaseNode.ConditionalTypes.Or:
                    OperatorName = "||";
                    break;
            }

            Debug.Assert(OperatorName != null);

            return $"{LeftText} {OperatorName} {RightText}";
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
