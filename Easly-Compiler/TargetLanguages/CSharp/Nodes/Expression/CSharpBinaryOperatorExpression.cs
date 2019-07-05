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

            for (int i = 0; i < ResolvedLeftResult.Count; i++)
            {
                ICompiledType OperatorBaseType = ResolvedLeftResult.At(i).ValueType;
                if (OperatorBaseType is IClassType AsClassType)
                    if (AsClassType.BaseClass.ClassGuid == LanguageClasses.Number.Guid)
                        IsCallingNumberFeature = true;
            }

            if (!IsCallingNumberFeature)
            {
                if (!LeftExpression.IsSingleResult)
                {
                    IResultType LeftResultType = LeftExpression.Source.ResolvedResult.Item;
                    LeftNameList = new List<ICSharpQualifiedName>();
                    LeftResultNameIndex = LeftResultType.ResultNameIndex;

                    for (int i = 0; i < LeftResultType.Count; i++)
                    {
                        IExpressionType DestinationType = LeftResultType.At(i);
                        string Text = DestinationType.Name;

                        BaseNode.IQualifiedName BaseNodeDestination = BaseNodeHelper.NodeHelper.CreateSimpleQualifiedName(Text);
                        IQualifiedName Destination = new QualifiedName(BaseNodeDestination, DestinationType);

                        IScopeAttributeFeature DestinationAttributeFeature = new ScopeAttributeFeature(LeftExpression.Source, Text, DestinationType.ValueTypeName, DestinationType.ValueType);
                        ICSharpScopeAttributeFeature DestinationFeature = CSharpScopeAttributeFeature.Create(context, null, DestinationAttributeFeature);

                        ICSharpQualifiedName CSharpDestination = CSharpQualifiedName.Create(context, Destination, DestinationFeature, null, false);
                        LeftNameList.Add(CSharpDestination);
                    }
                }

                if (!RightExpression.IsSingleResult)
                {
                    IResultType RightResultType = RightExpression.Source.ResolvedResult.Item;
                    RightNameList = new List<ICSharpQualifiedName>();
                    RightResultNameIndex = RightResultType.ResultNameIndex;

                    for (int i = 0; i < RightResultType.Count; i++)
                    {
                        IExpressionType DestinationType = RightResultType.At(i);
                        string Text = DestinationType.Name;

                        BaseNode.IQualifiedName BaseNodeDestination = BaseNodeHelper.NodeHelper.CreateSimpleQualifiedName(Text);
                        IQualifiedName Destination = new QualifiedName(BaseNodeDestination, DestinationType);

                        IScopeAttributeFeature DestinationAttributeFeature = new ScopeAttributeFeature(RightExpression.Source, Text, DestinationType.ValueTypeName, DestinationType.ValueType);
                        ICSharpScopeAttributeFeature DestinationFeature = CSharpScopeAttributeFeature.Create(context, null, DestinationAttributeFeature);

                        ICSharpQualifiedName CSharpDestination = CSharpQualifiedName.Create(context, Destination, DestinationFeature, null, false);
                        RightNameList.Add(CSharpDestination);
                    }
                }
            }
        }

        private IList<ICSharpQualifiedName> LeftNameList;
        private int LeftResultNameIndex = -1;

        private IList<ICSharpQualifiedName> RightNameList;
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
        /// True if calling a feature of the Number class.
        /// </summary>
        public bool IsCallingNumberFeature { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public override string CSharpText(ICSharpWriter writer)
        {
            WriteCSharp(writer, false, false, new List<ICSharpQualifiedName>(), -1, out string LastExpressionText);
            return LastExpressionText;
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="destinationList">The list of destinations.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        /// <param name="lastExpressionText">The text to use for the expression upon return.</param>
        public override void WriteCSharp(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace, IList<ICSharpQualifiedName> destinationList, int skippedIndex, out string lastExpressionText)
        {
            string OperatorText = Operator.Name;

            if (IsCallingNumberFeature)
            {
                switch (OperatorText)
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
                }
            }
            else
                OperatorText = CSharpNames.ToCSharpIdentifier(OperatorText);

            if (IsCallingNumberFeature)
            {
                string LeftText = NestedExpressionText(writer, LeftExpression);
                string RightText = NestedExpressionText(writer, RightExpression);

                lastExpressionText = $"{LeftText} {OperatorText} {RightText}";
            }
            else if (LeftExpression.IsSingleResult && RightExpression.IsSingleResult)
            {
                string LeftText = NestedExpressionText(writer, LeftExpression);
                string RightText = RightExpression.CSharpText(writer);

                lastExpressionText = $"{LeftText}.{OperatorText}({RightText})";
            }
            else if (LeftExpression.IsSingleResult)
            {
                Debug.Assert(RightNameList != null);

                string LeftText = NestedExpressionText(writer, LeftExpression);
                RightExpression.WriteCSharp(writer, true, true, RightNameList, RightResultNameIndex, out string RightExpressionText);

                lastExpressionText = $"{LeftText }.{OperatorText}({RightExpressionText})";
            }
            else if (RightExpression.IsSingleResult)
            {
                Debug.Assert(LeftNameList != null);

                LeftExpression.WriteCSharp(writer, true, true, LeftNameList, LeftResultNameIndex, out string LeftExpressionText);
                string RightText = RightExpression.CSharpText(writer);

                lastExpressionText = $"{LeftExpressionText}.{OperatorText}({RightText})";
            }
            else
            {
                Debug.Assert(LeftNameList != null);
                Debug.Assert(RightNameList != null);

                LeftExpression.WriteCSharp(writer, true, true, LeftNameList, LeftResultNameIndex, out string LeftExpressionText);
                RightExpression.WriteCSharp(writer, true, true, RightNameList, RightResultNameIndex, out string RightExpressionText);

                lastExpressionText = $"{LeftExpressionText}.{OperatorText}({RightExpressionText})";
            }
        }

        private string NestedExpressionText(ICSharpWriter writer, ICSharpExpression expression)
        {
            string Result = expression.CSharpText(writer);

            if (expression.IsComplex)
                Result = $"({Result})";

            return Result;
        }
        #endregion
    }
}
