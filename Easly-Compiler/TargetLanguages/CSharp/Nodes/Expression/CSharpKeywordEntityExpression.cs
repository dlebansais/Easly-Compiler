namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpKeywordEntityExpression : ICSharpExpression, ICSharpExpressionAsConstant
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

            string Result = null;
            //TODO

            switch (Source.Value)
            {
                case BaseNode.Keyword.True:
                    Result = "true";
                    break;

                case BaseNode.Keyword.False:
                    Result = "false";
                    break;

                case BaseNode.Keyword.Current:
                    Result = "this";
                    break;

                case BaseNode.Keyword.Value:
                    Result = "value";
                    break;

                case BaseNode.Keyword.Result:
                    Result = "Result";
                    break;

                case BaseNode.Keyword.Retry:
                    Result = "Retry";
                    break;

                case BaseNode.Keyword.Exception:
                    Result = "CaughtException";
                    break;

                case BaseNode.Keyword.Indexer:
                    Result = "Indexer";
                    break;
            }

            Debug.Assert(Result != null);

            expressionContext.SetSingleReturnValue(Result);
        }
        #endregion

        #region Implementation of ICSharpExpressionAsConstant
        /// <summary>
        /// True if the expression can provide its constant value directly.
        /// </summary>
        public bool IsDirectConstant { get { return true; } }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// Sets the <see cref="ICSharpOutputNode.WriteDown"/> flag.
        /// </summary>
        public override void SetWriteDown()
        {
            WriteDown = true;
        }
        #endregion
    }
}
