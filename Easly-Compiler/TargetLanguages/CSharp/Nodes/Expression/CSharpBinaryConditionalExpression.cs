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
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpBinaryConditionalExpression Create(IBinaryConditionalExpression source, ICSharpContext context)
        {
            return new CSharpBinaryConditionalExpression(source, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpBinaryConditionalExpression"/> class.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpBinaryConditionalExpression(IBinaryConditionalExpression source, ICSharpContext context)
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
            if (IsEventExpression)
                return CSharpTextEvent(cSharpNamespace, destinationList);
            else
                return CSharpTextBoolean(cSharpNamespace, destinationList);
        }

        private string CSharpTextEvent(string cSharpNamespace, IList<ICSharpQualifiedName> destinationList)
        {
            string LeftText = LeftExpression.CSharpText(cSharpNamespace);
            string RightText = RightExpression.CSharpText(cSharpNamespace);

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

        private string CSharpTextBoolean(string cSharpNamespace, IList<ICSharpQualifiedName> destinationList)
        {
            string LeftText = NestedExpressionText(LeftExpression, cSharpNamespace);
            string RightText = NestedExpressionText(RightExpression, cSharpNamespace);

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
