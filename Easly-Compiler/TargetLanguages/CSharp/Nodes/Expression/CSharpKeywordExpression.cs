namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpKeywordExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IKeywordExpression Source { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpKeywordExpression : CSharpExpression, ICSharpKeywordExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpKeywordExpression Create(ICSharpContext context, IKeywordExpression source)
        {
            return new CSharpKeywordExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpKeywordExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpKeywordExpression(ICSharpContext context, IKeywordExpression source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IKeywordExpression Source { get { return (IKeywordExpression)base.Source; } }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return false; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        public override string CSharpText(string cSharpNamespace)
        {
            return CSharpText(cSharpNamespace, new List<ICSharpQualifiedName>());
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="destinationList">The list of destinations.</param>
        public virtual string CSharpText(string cSharpNamespace, IList<ICSharpQualifiedName> destinationList)
        {
            string Result = null;

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
            }

            Debug.Assert(Result != null);

            return Result;
        }
        #endregion
    }
}
