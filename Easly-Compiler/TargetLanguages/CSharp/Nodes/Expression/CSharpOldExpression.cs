namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpOldExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IOldExpression Source { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpOldExpression : CSharpExpression, ICSharpOldExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpOldExpression Create(ICSharpContext context, IOldExpression source)
        {
            return new CSharpOldExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpOldExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpOldExpression(ICSharpContext context, IOldExpression source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IOldExpression Source { get { return (IOldExpression)base.Source; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection)
        {
            return CSharpText(usingCollection, new List<ICSharpQualifiedName>(), -1);
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="destinationList">The list of destinations.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection, IList<ICSharpQualifiedName> destinationList, int skippedIndex)
        {
            return string.Empty; // TODO
        }
        #endregion
    }
}
