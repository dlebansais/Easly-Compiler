namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpManifestStringExpression : ICSharpExpression, ICSharpExpressionAsConstant
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IManifestStringExpression Source { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpManifestStringExpression : CSharpExpression, ICSharpManifestStringExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpManifestStringExpression Create(ICSharpContext context, IManifestStringExpression source)
        {
            return new CSharpManifestStringExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpManifestStringExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpManifestStringExpression(ICSharpContext context, IManifestStringExpression source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IManifestStringExpression Source { get { return (IManifestStringExpression)base.Source; } }
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
            string Result = string.Empty;

            foreach (char c in Source.ValidText.Item)
                switch (c)
                {
                    case '\"':
                        Result += "\\\"";
                        break;
                    case '\\':
                        Result += "\\\\";
                        break;
                    case '\a':
                        Result += "\\a";
                        break;
                    case '\b':
                        Result += "\\b";
                        break;
                    case '\f':
                        Result += "\\f";
                        break;
                    case '\n':
                        Result += "\\n";
                        break;
                    case '\r':
                        Result += "\\r";
                        break;
                    case '\t':
                        Result += "\\t";
                        break;
                    default:
                        Result += c;
                        break;
                }

            expressionContext.SetSingleReturnValue($"\"{Result}\"");
        }
        #endregion

        #region Implementation of ICSharpExpressionAsConstant
        /// <summary>
        /// True if the expression can provide its constant value directly.
        /// </summary>
        public bool IsDirectConstant { get { return true; } }
        #endregion
    }
}
