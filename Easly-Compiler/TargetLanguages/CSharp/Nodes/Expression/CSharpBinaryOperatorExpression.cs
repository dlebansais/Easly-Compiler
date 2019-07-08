namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpBinaryOperatorExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IBinaryOperatorExpression Source { get; }

        /// <summary>
        /// The left expression.
        /// </summary>
        ICSharpExpression LeftExpression { get; }

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

        /// <summary>
        /// The selected overload type.
        /// </summary>
        ICSharpQueryOverloadType SelectedOverloadType { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpBinaryOperatorExpression : CSharpExpression, ICSharpBinaryOperatorExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpBinaryOperatorExpression Create(ICSharpContext context, IBinaryOperatorExpression source)
        {
            return new CSharpBinaryOperatorExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpBinaryOperatorExpression"/> class.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpBinaryOperatorExpression(ICSharpContext context, IBinaryOperatorExpression source)
            : base(context, source)
        {
            LeftExpression = Create(context, (IExpression)source.LeftExpression);
            RightExpression = Create(context, (IExpression)source.RightExpression);

            Operator = context.GetFeature(source.SelectedFeature.Item) as ICSharpFunctionFeature;
            Debug.Assert(Operator != null);

            IResultType ResolvedLeftResult = LeftExpression.Source.ResolvedResult.Item;
            IExpressionType PreferredLeftResult = ResolvedLeftResult.Preferred;
            Debug.Assert(PreferredLeftResult != null);

            if (PreferredLeftResult.ValueType is IClassType AsClassType)
                if (AsClassType.BaseClass.ClassGuid == LanguageClasses.Number.Guid)
                    IsCallingNumberFeature = true;

            if (IsCallingNumberFeature)
            {
                IResultType ResolvedRightResult = RightExpression.Source.ResolvedResult.Item;
                Debug.Assert(ResolvedRightResult.Count == 1);
            }
            else
            {
                if (!LeftExpression.IsSingleResult)
                {
                    IResultType LeftResultType = LeftExpression.Source.ResolvedResult.Item;
                    LeftNameList = new List<string>();
                    LeftResultNameIndex = LeftResultType.ResultNameIndex;

                    for (int i = 0; i < LeftResultType.Count; i++)
                    {
                        IExpressionType DestinationType = LeftResultType.At(i);
                        string Text = DestinationType.Name;
                        LeftNameList.Add(Text);
                    }
                }

                if (!RightExpression.IsSingleResult)
                {
                    IResultType RightResultType = RightExpression.Source.ResolvedResult.Item;
                    RightNameList = new List<string>();
                    RightResultNameIndex = RightResultType.ResultNameIndex;

                    for (int i = 0; i < RightResultType.Count; i++)
                    {
                        IExpressionType DestinationType = RightResultType.At(i);
                        string Text = DestinationType.Name;
                        RightNameList.Add(Text);
                    }
                }
            }

            FeatureCall = new CSharpFeatureCall(context, Source.FeatureCall.Item);

            Debug.Assert(Source.SelectedOverload.IsAssigned);
            IQueryOverload Overload = Source.SelectedOverload.Item;
            IQueryOverloadType ResolvedAssociatedType = Overload.ResolvedAssociatedType.Item;
            SelectedOverloadType = CSharpQueryOverloadType.Create(context, ResolvedAssociatedType, Operator.Owner);
        }

        private IList<string> LeftNameList;
        private int LeftResultNameIndex = -1;

        private IList<string> RightNameList;
        private int RightResultNameIndex = -1;
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IBinaryOperatorExpression Source { get { return (IBinaryOperatorExpression)base.Source; } }

        /// <summary>
        /// The left expression.
        /// </summary>
        public ICSharpExpression LeftExpression { get; }

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
        /// The selected overload type.
        /// </summary>
        public ICSharpQueryOverloadType SelectedOverloadType { get; }

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
            string OperatorText = null;

            switch (Operator.Name)
            {
                case "≥":
                    OperatorText = ">=";
                    break;
                case "≤":
                    OperatorText = "<=";
                    break;
                case "shift right":
                    OperatorText = ">>";
                    break;
                case "shift left":
                    OperatorText = "<<";
                    break;
                case "modulo":
                    OperatorText = "%";
                    break;
                case "bitwise and":
                    OperatorText = "&";
                    break;
                case "bitwise or":
                    OperatorText = "|";
                    break;
                case "bitwise xor":
                    OperatorText = "^";
                    break;
                default:
                    OperatorText = Operator.Name;
                    break;
            }

            string LeftText = NestedExpressionText(writer, LeftExpression);
            string RightText = NestedExpressionText(writer, RightExpression);

            expressionContext.SetSingleReturnValue($"{LeftText} {OperatorText} {RightText}");
        }

        private void WriteCSharpCustomOperator(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex)
        {
            string OperatorText = CSharpNames.ToCSharpIdentifier(Operator.Name);

            if (LeftExpression.IsSingleResult && RightExpression.IsSingleResult)
            {
                string LeftText = NestedExpressionText(writer, LeftExpression);

                ICSharpExpressionContext SourceExpressionContext = new CSharpExpressionContext();
                RightExpression.WriteCSharp(writer, SourceExpressionContext, -1);
                string RightText = SourceExpressionContext.ReturnValue;

                expressionContext.SetSingleReturnValue($"{LeftText}.{OperatorText}({RightText})");
            }
            else
            {
                Operator.GetOutputFormat(SelectedOverloadType, out int OutgoingParameterCount, out int ReturnValueIndex);

                string LeftText = NestedExpressionText(writer, LeftExpression);

                CSharpArgument.CSharpArgumentList(writer, expressionContext, FeatureCall, ReturnValueIndex, false, out string ArgumentListText, out IList<string> OutgoingResultList);

                Debug.Assert(OutgoingParameterCount > 1);

                if (ReturnValueIndex >= 0)
                {
                    string TemporaryResultName = writer.GetTemporaryName();
                    writer.WriteIndentedLine($"var {TemporaryResultName} = {LeftText}.{OperatorText}({ArgumentListText});");

                    OutgoingResultList.Insert(ReturnValueIndex, TemporaryResultName);
                }
                else
                    writer.WriteIndentedLine($"{LeftText}.{OperatorText}({ArgumentListText});");

                expressionContext.SetMultipleResult(OutgoingResultList, ReturnValueIndex);

                /*
                if (LeftExpression.IsSingleResult)
                {
                    Debug.Assert(RightNameList != null);

                    string LeftText = NestedExpressionText(writer, LeftExpression);

                    ICSharpExpressionContext ExpressionContext = new CSharpExpressionContext();
                    RightExpression.WriteCSharp(writer, ExpressionContext, true, true, -1);
                    string RightText = ExpressionContext.ResultListAsArgument;

                    expressionContext.SetSingleReturnValue($"{LeftText}.{OperatorText}({RightText})");
                }
                else if (RightExpression.IsSingleResult)
                {
                    Debug.Assert(LeftNameList != null);

                    ICSharpExpressionContext ExpressionContext = new CSharpExpressionContext();
                    LeftExpression.WriteCSharp(writer, ExpressionContext, true, true, LeftResultNameIndex);

                    string LeftExpressionText = ExpressionContext.ResultListAsArgument;

                    ICSharpExpressionContext SourceExpressionContext = new CSharpExpressionContext();
                    RightExpression.WriteCSharp(writer, SourceExpressionContext, false, false, -1);

                    string RightText = SourceExpressionContext.ReturnValue;

                    expressionContext.SetSingleReturnValue($"{LeftExpressionText}.{OperatorText}({RightText})");
                }
                else
                {
                    Debug.Assert(LeftNameList != null);
                    Debug.Assert(RightNameList != null);

                    ICSharpExpressionContext LeftExpressionContext = new CSharpExpressionContext();
                    LeftExpression.WriteCSharp(writer, LeftExpressionContext, true, true, LeftResultNameIndex);
                    string LeftExpressionText = LeftExpressionContext.ResultListAsArgument;

                    ICSharpExpressionContext RightExpressionContext = new CSharpExpressionContext();
                    RightExpression.WriteCSharp(writer, RightExpressionContext, true, true, RightResultNameIndex);
                    string RightExpressionText = RightExpressionContext.ResultListAsArgument;

                    expressionContext.SetSingleReturnValue($"{LeftExpressionText}.{OperatorText}({RightExpressionText})");
                }
                */
            }
        }

        private string NestedExpressionText(ICSharpWriter writer, ICSharpExpression expression)
        {
            ICSharpExpressionContext SourceExpressionContext = new CSharpExpressionContext();
            expression.WriteCSharp(writer, SourceExpressionContext, -1);

            string Result = SourceExpressionContext.ReturnValue;

            if (expression.IsComplex)
                Result = $"({Result})";

            return Result;
        }
        #endregion
    }
}
