﻿namespace EaslyCompiler
{
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
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        string CSharpText(string cSharpNamespace);
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
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpExpression Create(IExpression source, ICSharpContext context)
        {
            ICSharpExpression Result = null;

            switch (source)
            {
                case IAgentExpression AsAgentExpression:
                    Result = CSharpAgentExpression.Create(AsAgentExpression, context);
                    break;

                case IAssertionTagExpression AsAssertionTagExpression:
                    Result = CSharpAssertionTagExpression.Create(AsAssertionTagExpression, context);
                    break;

                case IBinaryConditionalExpression AsBinaryConditionalExpression:
                    Result = CSharpBinaryConditionalExpression.Create(AsBinaryConditionalExpression, context);
                    break;

                case IBinaryOperatorExpression AsBinaryOperatorExpression:
                    Result = CSharpBinaryOperatorExpression.Create(AsBinaryOperatorExpression, context);
                    break;

                case IClassConstantExpression AsClassConstantExpression:
                    Result = CSharpClassConstantExpression.Create(AsClassConstantExpression, context);
                    break;

                case ICloneOfExpression AsCloneOfExpression:
                    Result = CSharpCloneOfExpression.Create(AsCloneOfExpression, context);
                    break;

                case IEntityExpression AsEntityExpression:
                    Result = CSharpEntityExpression.Create(AsEntityExpression, context);
                    break;

                case IEqualityExpression AsEqualityExpression:
                    Result = CSharpEqualityExpression.Create(AsEqualityExpression, context);
                    break;

                case IIndexQueryExpression AsIndexQueryExpression:
                    Result = CSharpIndexQueryExpression.Create(AsIndexQueryExpression, context);
                    break;

                case IInitializedObjectExpression AsInitializedObjectExpression:
                    Result = CSharpInitializedObjectExpression.Create(AsInitializedObjectExpression, context);
                    break;

                case IKeywordExpression AsKeywordExpression:
                    Result = CSharpKeywordExpression.Create(AsKeywordExpression, context);
                    break;

                case IManifestCharacterExpression AsManifestCharacterExpression:
                    Result = CSharpManifestCharacterExpression.Create(AsManifestCharacterExpression, context);
                    break;

                case IManifestNumberExpression AsManifestNumberExpression:
                    Result = CSharpManifestNumberExpression.Create(AsManifestNumberExpression, context);
                    break;

                case IManifestStringExpression AsManifestStringExpression:
                    Result = CSharpManifestStringExpression.Create(AsManifestStringExpression, context);
                    break;

                case INewExpression AsNewExpression:
                    Result = CSharpNewExpression.Create(AsNewExpression, context);
                    break;

                case IOldExpression AsOldExpression:
                    Result = CSharpOldExpression.Create(AsOldExpression, context);
                    break;

                case IPrecursorExpression AsPrecursorExpression:
                    Result = CSharpPrecursorExpression.Create(AsPrecursorExpression, context);
                    break;

                case IPrecursorIndexExpression AsPrecursorIndexExpression:
                    Result = CSharpPrecursorIndexExpression.Create(AsPrecursorIndexExpression, context);
                    break;

                case IQueryExpression AsQueryExpression:
                    Result = CSharpQueryExpression.Create(AsQueryExpression, context);
                    break;

                case IResultOfExpression AsResultOfExpression:
                    Result = CSharpResultOfExpression.Create(AsResultOfExpression, context);
                    break;

                case IUnaryNotExpression AsUnaryNotExpression:
                    Result = CSharpUnaryNotExpression.Create(AsUnaryNotExpression, context);
                    break;

                case IUnaryOperatorExpression AsUnaryOperatorExpression:
                    Result = CSharpUnaryOperatorExpression.Create(AsUnaryOperatorExpression, context);
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpExpression"/> class.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpExpression(IExpression source, ICSharpContext context)
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
        public abstract bool IsComplex { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        public abstract string CSharpText(string cSharpNamespace);
        #endregion
    }
}
