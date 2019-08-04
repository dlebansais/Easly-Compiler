namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpEqualityExpression : ICSharpExpression, ICSharpExpressionAsConstant, ICSharpComputableExpression
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
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public override void CheckNumberType(ref bool isChanged)
        {
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expressionContext">The context.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex)
        {
            Debug.Assert(WriteDown);

            string LeftText;

            ICSharpExpressionContext LeftSourceExpressionContext = new CSharpExpressionContext();
            LeftExpression.WriteCSharp(writer, LeftSourceExpressionContext, -1);

            if (LeftSourceExpressionContext.ReturnValue != null)
            {
                string Result = LeftSourceExpressionContext.ReturnValue;

                if (LeftExpression.IsComplex)
                    Result = $"({Result})";

                LeftText = Result;
            }
            else
            {
                //TODO
                LeftText = "TODO";
            }

            string RightText;

            ICSharpExpressionContext RightSourceExpressionContext = new CSharpExpressionContext();
            RightExpression.WriteCSharp(writer, RightSourceExpressionContext, -1);

            if (RightSourceExpressionContext.ReturnValue != null)
            {
                string Result = RightSourceExpressionContext.ReturnValue;

                if (RightExpression.IsComplex)
                    Result = $"({Result})";

                RightText = Result;
            }
            else
            {
                //TODO
                RightText = "TODO";
            }

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

        #region Implementation of ICSharpComputableExpression
        /// <summary>
        /// The expression computed constant value.
        /// </summary>
        public string ComputedValue { get; private set; }

        /// <summary>
        /// Runs the compiler to compute the value as a string.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public void Compute(ICSharpWriter writer)
        {
            string LeftValue = ComputeNestedExpression(writer, LeftExpression);
            string RightValue = ComputeNestedExpression(writer, RightExpression);

            bool IsHandled = false;

            switch (Source.Comparison)
            {
                case BaseNode.ComparisonType.Equal:
                    ComputedValue = ToComputedValue(LeftValue == RightValue);
                    IsHandled = true;
                    break;
                case BaseNode.ComparisonType.Different:
                    ComputedValue = ToComputedValue(LeftValue != RightValue);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }

        private string ToComputedValue(bool value)
        {
            return value ? "true" : "false";
        }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// Sets the <see cref="ICSharpOutputNode.WriteDown"/> flag.
        /// </summary>
        public override void SetWriteDown()
        {
            if (WriteDown)
                return;

            WriteDown = true;

            LeftExpression.SetWriteDown();
            RightExpression.SetWriteDown();
        }
        #endregion
    }
}
