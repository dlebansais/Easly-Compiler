namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using FormattedNumber;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpUnaryOperatorExpression : ICSharpExpression, ICSharpExpressionAsConstant, ICSharpComputableExpression
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

        /// <summary>
        /// The feature call.
        /// </summary>
        ICSharpFeatureCall FeatureCall { get; }
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

            FeatureCall = new CSharpFeatureCall(context, new FeatureCall());

            IResultType ResolvedRightResult = RightExpression.Source.ResolvedResult.Item;
            IExpressionType PreferredRightResult = ResolvedRightResult.Preferred;
            Debug.Assert(PreferredRightResult != null);

            if (PreferredRightResult.ValueType is IClassType AsClassType)
                if (AsClassType.BaseClass.ClassGuid == LanguageClasses.Number.Guid)
                    IsCallingNumberFeature = true;

            if (IsCallingNumberFeature)
            {
                Debug.Assert(ResolvedRightResult.Count == 1);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IUnaryOperatorExpression Source { get { return (IUnaryOperatorExpression)base.Source; } }

        /// <summary>
        /// The right expression.
        /// </summary>
        public ICSharpExpression RightExpression { get; }

        /// <summary>
        /// The operator.
        /// </summary>
        public ICSharpFunctionFeature Operator { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        public ICSharpFeatureCall FeatureCall { get; }

        /// <summary>
        /// True if calling a feature of the Number class.
        /// </summary>
        public bool IsCallingNumberFeature { get; }
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
            if (IsCallingNumberFeature)
                WriteCSharpNumberOperator(writer, expressionContext, skippedIndex);
            else
                WriteCSharpCustomOperator(writer, expressionContext, skippedIndex);
        }

        private void WriteCSharpNumberOperator(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex)
        {
            string RightText = SingleResultExpressionText(writer, RightExpression);
            string OperatorText = Operator.Name;

            expressionContext.SetSingleReturnValue($"{OperatorText}{RightText}");
        }

        private void WriteCSharpCustomOperator(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex)
        {
            string RightText = SingleResultExpressionText(writer, RightExpression);
            string OperatorText = Operator.Name;

            expressionContext.SetSingleReturnValue($"{OperatorText} {RightText}");
        }
        #endregion

        #region Implementation of ICSharpExpressionAsConstant
        /// <summary>
        /// True if the expression can provide its constant value directly.
        /// </summary>
        public bool IsDirectConstant { get { return false; } }
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
            if (IsCallingNumberFeature)
                ComputeNumberOperator(writer);
            else
                ComputeCustomOperator(writer);
        }

        private void ComputeNumberOperator(ICSharpWriter writer)
        {
            CanonicalNumber RightNumber = ComputeSide(writer, RightExpression);

            bool IsHandled = false;

            switch (Operator.Name)
            {
                case "-":
                    ComputedValue = ToComputedValue(RightNumber.Negate());
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }

        private CanonicalNumber ComputeSide(ICSharpWriter writer, ICSharpExpression expression)
        {
            string ValueString;

            ICSharpExpressionAsConstant ExpressionAsConstant = expression as ICSharpExpressionAsConstant;
            Debug.Assert(ExpressionAsConstant != null);

            if (ExpressionAsConstant.IsDirectConstant)
            {
                ICSharpExpressionContext SourceExpressionContext = new CSharpExpressionContext();
                expression.WriteCSharp(writer, SourceExpressionContext, -1);

                ValueString = SourceExpressionContext.ReturnValue;
            }
            else
            {
                ICSharpComputableExpression ComputableExpression = ExpressionAsConstant as ICSharpComputableExpression;
                Debug.Assert(ComputableExpression != null);

                ComputableExpression.Compute(writer);
                ValueString = ComputableExpression.ComputedValue;
            }

            FormattedNumber Result = Parser.Parse(ValueString);

            return Result.Canonical;
        }

        private string ToComputedValue(CanonicalNumber value)
        {
            return value.ToString();
        }

        private void ComputeCustomOperator(ICSharpWriter writer)
        {
            ComputedValue = CSharpQueryExpression.ComputeQueryResult(writer, Operator, FeatureCall);
        }
        #endregion
    }
}
