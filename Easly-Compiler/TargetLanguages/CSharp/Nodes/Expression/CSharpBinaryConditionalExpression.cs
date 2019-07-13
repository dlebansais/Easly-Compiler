namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpBinaryConditionalExpression : ICSharpExpression, ICSharpExpressionAsConstant, ICSharpCompilableExpression
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
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex)
        {
            if (IsEventExpression)
                expressionContext.SetSingleReturnValue(CSharpTextEvent(writer, expressionContext));
            else
                WriteCSharpBoolean(writer, expressionContext);
        }

        private string CSharpTextEvent(ICSharpWriter writer, ICSharpExpressionContext expressionContext)
        {
            string LeftText = SingleResultExpressionText(writer, LeftExpression);
            string RightText = SingleResultExpressionText(writer, RightExpression);

            string OperatorName = null;

            switch (Source.Conditional)
            {
                case BaseNode.ConditionalTypes.And:
                    OperatorName = "&&";
                    break;

                case BaseNode.ConditionalTypes.Or:
                    OperatorName = "||";
                    break;

                case BaseNode.ConditionalTypes.Xor:
                    OperatorName = "^";
                    break;

                case BaseNode.ConditionalTypes.Implies:
                    OperatorName = "/";
                    break;
            }

            Debug.Assert(OperatorName != null);

            return $"{LeftText} {OperatorName} {RightText}";
        }

        private void WriteCSharpBoolean(ICSharpWriter writer, ICSharpExpressionContext expressionContext)
        {
            if (LeftExpression.IsSingleResult && RightExpression.IsSingleResult)
            {
                string LeftText = SingleResultExpressionText(writer, LeftExpression);
                string RightText = SingleResultExpressionText(writer, RightExpression);

                if (Source.Conditional == BaseNode.ConditionalTypes.Implies)
                    expressionContext.SetSingleReturnValue($"!{LeftText} || {RightText}");
                else
                {
                    string OperatorName = null;

                    switch (Source.Conditional)
                    {
                        case BaseNode.ConditionalTypes.And:
                            OperatorName = "&&";
                            break;

                        case BaseNode.ConditionalTypes.Or:
                            OperatorName = "||";
                            break;

                        case BaseNode.ConditionalTypes.Xor:
                            OperatorName = "^";
                            break;
                    }

                    Debug.Assert(OperatorName != null);

                    expressionContext.SetSingleReturnValue($"{LeftText} {OperatorName} {RightText}");
                }
            }
            else
            {
                //TODO
                expressionContext.SetSingleReturnValue("TODO");
            }
        }
        #endregion

        #region Implementation of ICSharpExpressionAsConstant
        /// <summary>
        /// True if the expression can provide its constant value directly.
        /// </summary>
        public bool IsDirectConstant
        {
            get
            {
                return LeftExpression is ICSharpExpressionAsConstant LeftExpressionAsConstant &&
                       RightExpression is ICSharpExpressionAsConstant RightExpressionAsConstant &&
                       LeftExpressionAsConstant.IsDirectConstant && RightExpressionAsConstant.IsDirectConstant;
            }
        }
        #endregion

        #region Implementation of ICSharpCompilableExpression
        /// <summary>
        /// The expression compiled constant value.
        /// </summary>
        public string CompiledValue
        {
            get
            {
                return "TODO";
            }
        }
        #endregion
    }
}
