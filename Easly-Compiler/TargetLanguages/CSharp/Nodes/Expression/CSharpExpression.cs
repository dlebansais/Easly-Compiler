namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpExpression
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
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        string CSharpText(ICSharpWriter writer);

        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="destinationList">List of destinations.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        /// <param name="lastExpressionText">The text to use for the expression upon return.</param>
        void WriteCSharp(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace, IList<ICSharpQualifiedName> destinationList, int skippedIndex, out string lastExpressionText);
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
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public abstract string CSharpText(ICSharpWriter writer);

        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="destinationList">List of destinations.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        /// <param name="lastExpressionText">The text to use for the expression upon return.</param>
        public abstract void WriteCSharp(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace, IList<ICSharpQualifiedName> destinationList, int skippedIndex, out string lastExpressionText);
        #endregion
    }
}
