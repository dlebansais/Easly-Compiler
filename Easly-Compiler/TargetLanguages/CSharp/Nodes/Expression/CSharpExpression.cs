﻿namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpExpression : ICSharpOutputNode
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        IExpression Source { get; }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        bool IsComplex { get; }

        /// <summary>
        /// True if the expression returns only one result.
        /// </summary>
        bool IsSingleResult { get; }

        /// <summary>
        /// True if the condition is on events.
        /// </summary>
        bool IsEventExpression { get; }

        /// <summary>
        /// The number type if the result is a single number.
        /// </summary>
        CSharpNumberTypes NumberType { get; set; }

        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expressionContext">The context.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex);
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public abstract class CSharpExpression : ICSharpExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpExpression Create(ICSharpContext context, IExpression source)
        {
            ICSharpExpression Result = null;

            switch (source)
            {
                case IAgentExpression AsAgentExpression:
                    Result = CSharpAgentExpression.Create(context, AsAgentExpression);
                    break;

                case IAssertionTagExpression AsAssertionTagExpression:
                    Result = CSharpAssertionTagExpression.Create(context, AsAssertionTagExpression);
                    break;

                case IBinaryConditionalExpression AsBinaryConditionalExpression:
                    Result = CSharpBinaryConditionalExpression.Create(context, AsBinaryConditionalExpression);
                    break;

                case IBinaryOperatorExpression AsBinaryOperatorExpression:
                    Result = CSharpBinaryOperatorExpression.Create(context, AsBinaryOperatorExpression);
                    break;

                case IClassConstantExpression AsClassConstantExpression:
                    Result = CSharpClassConstantExpression.Create(context, AsClassConstantExpression);
                    break;

                case ICloneOfExpression AsCloneOfExpression:
                    Result = CSharpCloneOfExpression.Create(context, AsCloneOfExpression);
                    break;

                case IEntityExpression AsEntityExpression:
                    Result = CSharpEntityExpression.Create(context, AsEntityExpression);
                    break;

                case IEqualityExpression AsEqualityExpression:
                    Result = CSharpEqualityExpression.Create(context, AsEqualityExpression);
                    break;

                case IIndexQueryExpression AsIndexQueryExpression:
                    Result = CSharpIndexQueryExpression.Create(context, AsIndexQueryExpression);
                    break;

                case IInitializedObjectExpression AsInitializedObjectExpression:
                    Result = CSharpInitializedObjectExpression.Create(context, AsInitializedObjectExpression);
                    break;

                case IKeywordEntityExpression AsKeywordEntityExpression:
                    Result = CSharpKeywordEntityExpression.Create(context, AsKeywordEntityExpression);
                    break;

                case IKeywordExpression AsKeywordExpression:
                    Result = CSharpKeywordExpression.Create(context, AsKeywordExpression);
                    break;

                case IManifestCharacterExpression AsManifestCharacterExpression:
                    Result = CSharpManifestCharacterExpression.Create(context, AsManifestCharacterExpression);
                    break;

                case IManifestNumberExpression AsManifestNumberExpression:
                    Result = CSharpManifestNumberExpression.Create(context, AsManifestNumberExpression);
                    break;

                case IManifestStringExpression AsManifestStringExpression:
                    Result = CSharpManifestStringExpression.Create(context, AsManifestStringExpression);
                    break;

                case INewExpression AsNewExpression:
                    Result = CSharpNewExpression.Create(context, AsNewExpression);
                    break;

                case IOldExpression AsOldExpression:
                    Result = CSharpOldExpression.Create(context, AsOldExpression);
                    break;

                case IPrecursorExpression AsPrecursorExpression:
                    Result = CSharpPrecursorExpression.Create(context, AsPrecursorExpression);
                    break;

                case IPrecursorIndexExpression AsPrecursorIndexExpression:
                    Result = CSharpPrecursorIndexExpression.Create(context, AsPrecursorIndexExpression);
                    break;

                case IQueryExpression AsQueryExpression:
                    Result = CSharpQueryExpression.Create(context, AsQueryExpression);
                    break;

                case IResultOfExpression AsResultOfExpression:
                    Result = CSharpResultOfExpression.Create(context, AsResultOfExpression);
                    break;

                case IUnaryNotExpression AsUnaryNotExpression:
                    Result = CSharpUnaryNotExpression.Create(context, AsUnaryNotExpression);
                    break;

                case IUnaryOperatorExpression AsUnaryOperatorExpression:
                    Result = CSharpUnaryOperatorExpression.Create(context, AsUnaryOperatorExpression);
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpExpression(ICSharpContext context, IExpression source)
        {
            Debug.Assert(source != null);

            Source = source;
            NumberType = CSharpNumberTypes.NotApplicable;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public IExpression Source { get; }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public virtual bool IsComplex { get { return Source.IsComplex; } }

        /// <summary>
        /// True if the expression returns only one result.
        /// </summary>
        public virtual bool IsSingleResult { get { return Source.ResolvedResult.Item.Count == 1; } }

        /// <summary>
        /// True if the condition is on events.
        /// </summary>
        public virtual bool IsEventExpression
        {
            get
            {
                return Expression.IsLanguageTypeAvailable(LanguageClasses.Event.Guid, Source, out ITypeName EventTypeName, out ICompiledType EventType) && IsSingleResult && Source.ResolvedResult.Item.At(0).ValueType == EventType;
            }
        }

        /// <summary>
        /// The number type if the result is a single number.
        /// </summary>
        public CSharpNumberTypes NumberType { get; set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expressionContext">The context.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public abstract void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex);

        /// <summary>
        /// Gets the return value of an expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expression">The expression.</param>
        public static string SingleResultExpressionText(ICSharpWriter writer, ICSharpExpression expression)
        {
            ICSharpExpressionContext SourceExpressionContext = new CSharpExpressionContext();
            expression.WriteCSharp(writer, SourceExpressionContext, -1);

            Debug.Assert(SourceExpressionContext.ReturnValue != null);
            string Result = SourceExpressionContext.ReturnValue;

            if (expression.IsComplex)
                Result = $"({Result})";

            return Result;
        }
        #endregion

        #region Descendant Interface
        /// <summary>
        /// Computes the constant value of a nested expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expression">The expression to compute.</param>
        protected static string ComputeNestedExpression(ICSharpWriter writer, ICSharpExpression expression)
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

            return ValueString;
        }

        /// <summary>
        /// Updates the number type of the expression from a type.
        /// </summary>
        /// <param name="type">The type that may be a number.</param>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        protected void UpdateNumberType(ICSharpType type, ref bool isChanged)
        {
            if (type.IsNumberType && (NumberType == CSharpNumberTypes.NotApplicable || NumberType == CSharpNumberTypes.Unknown))
            {
                if (type.NumberType == CSharpNumberTypes.Integer || type.NumberType == CSharpNumberTypes.Real)
                {
                    NumberType = type.NumberType;
                    isChanged = true;
                }
            }
        }

        /// <summary>
        /// Updates the number type of the expression from an expression.
        /// </summary>
        /// <param name="expression">The expression that may be a number.</param>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        protected void UpdateNumberType(ICSharpExpression expression, ref bool isChanged)
        {
            if ((expression.NumberType != CSharpNumberTypes.NotApplicable && expression.NumberType != CSharpNumberTypes.Unknown) && (NumberType == CSharpNumberTypes.NotApplicable || NumberType == CSharpNumberTypes.Unknown))
            {
                NumberType = expression.NumberType;
                isChanged = true;
            }
        }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// True if the node should be produced.
        /// </summary>
        public bool WriteDown { get; protected set; }

        /// <summary>
        /// Sets the <see cref="WriteDown"/> flag.
        /// </summary>
        public abstract void SetWriteDown();
        #endregion
    }
}
