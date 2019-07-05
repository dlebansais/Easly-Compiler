namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpKeywordEntityExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IKeywordEntityExpression Source { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpKeywordEntityExpression : CSharpExpression, ICSharpKeywordEntityExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpKeywordEntityExpression Create(ICSharpContext context, IKeywordEntityExpression source)
        {
            return new CSharpKeywordEntityExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpKeywordEntityExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpKeywordEntityExpression(ICSharpContext context, IKeywordEntityExpression source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IKeywordEntityExpression Source { get { return (IKeywordEntityExpression)base.Source; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expressionContext">The context.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="destinationList">The list of destinations.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        /// <param name="lastExpressionText">The text to use for the expression upon return.</param>
        public override void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, bool isNeverSimple, bool isDeclaredInPlace, IList<ICSharpQualifiedName> destinationList, int skippedIndex, out string lastExpressionText)
        {
            lastExpressionText = null;
            //TODO

            switch (Source.Value)
            {
                case BaseNode.Keyword.True:
                    lastExpressionText = "true";
                    break;

                case BaseNode.Keyword.False:
                    lastExpressionText = "false";
                    break;

                case BaseNode.Keyword.Current:
                    lastExpressionText = "this";
                    break;

                case BaseNode.Keyword.Value:
                    lastExpressionText = "value";
                    break;

                case BaseNode.Keyword.Result:
                    lastExpressionText = "Result";
                    break;

                case BaseNode.Keyword.Retry:
                    lastExpressionText = "Retry";
                    break;

                case BaseNode.Keyword.Exception:
                    lastExpressionText = "CaughtException";
                    break;

                case BaseNode.Keyword.Indexer:
                    lastExpressionText = "Indexer";
                    break;
            }

            Debug.Assert(lastExpressionText != null);
        }
        #endregion
    }
}
