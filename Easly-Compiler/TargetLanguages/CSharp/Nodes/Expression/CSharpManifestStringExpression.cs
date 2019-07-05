namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpManifestStringExpression : ICSharpExpression
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
            lastExpressionText = string.Empty;

            foreach (char c in Source.ValidText.Item)
                switch (c)
                {
                    case '\"':
                        lastExpressionText += "\\\"";
                        break;
                    case '\\':
                        lastExpressionText += "\\\\";
                        break;
                    case '\a':
                        lastExpressionText += "\\a";
                        break;
                    case '\b':
                        lastExpressionText += "\\b";
                        break;
                    case '\f':
                        lastExpressionText += "\\f";
                        break;
                    case '\n':
                        lastExpressionText += "\\n";
                        break;
                    case '\r':
                        lastExpressionText += "\\r";
                        break;
                    case '\t':
                        lastExpressionText += "\\t";
                        break;
                    default:
                        lastExpressionText += c;
                        break;
                }

            lastExpressionText = $"\"{lastExpressionText}\"";
        }
        #endregion
    }
}
