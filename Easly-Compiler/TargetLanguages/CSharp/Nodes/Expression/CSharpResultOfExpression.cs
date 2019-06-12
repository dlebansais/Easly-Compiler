namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpResultOfExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IResultOfExpression Source { get; }

        /// <summary>
        /// The source expression.
        /// </summary>
        ICSharpExpression SourceExpression { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpResultOfExpression : CSharpExpression, ICSharpResultOfExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpResultOfExpression Create(ICSharpContext context, IResultOfExpression source)
        {
            return new CSharpResultOfExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpResultOfExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpResultOfExpression(ICSharpContext context, IResultOfExpression source)
            : base(context, source)
        {
            SourceExpression = Create(context, (IExpression)source.Source);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IResultOfExpression Source { get { return (IResultOfExpression)base.Source; } }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return true; } }

        /// <summary>
        /// The source expression.
        /// </summary>
        public ICSharpExpression SourceExpression { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection)
        {
            return CSharpText(usingCollection, new List<ICSharpQualifiedName>());
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="destinationList">The list of destinations.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection, IList<ICSharpQualifiedName> destinationList)
        {
            return SourceExpression.CSharpText(usingCollection, destinationList);
        }
        #endregion
    }
}
