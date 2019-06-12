namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpIndexQueryExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IIndexQueryExpression Source { get; }

        /// <summary>
        /// The indexed expression.
        /// </summary>
        ICSharpExpression IndexedExpression { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        ICSharpFeatureCall FeatureCall { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpIndexQueryExpression : CSharpExpression, ICSharpIndexQueryExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpIndexQueryExpression Create(ICSharpContext context, IIndexQueryExpression source)
        {
            return new CSharpIndexQueryExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpIndexQueryExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpIndexQueryExpression(ICSharpContext context, IIndexQueryExpression source)
            : base(context, source)
        {
            IndexedExpression = Create(context, (IExpression)source.IndexedExpression);
            FeatureCall = new CSharpFeatureCall(context, source.FeatureCall.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IIndexQueryExpression Source { get { return (IIndexQueryExpression)base.Source; } }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return false; } }

        /// <summary>
        /// The indexed expression.
        /// </summary>
        public ICSharpExpression IndexedExpression { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        public ICSharpFeatureCall FeatureCall { get; }
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
            string IndexedText = IndexedExpression.CSharpText(usingCollection);
            string ArgumentListText = CSharpArgument.CSharpArgumentList(usingCollection, FeatureCall, destinationList);

            return $"{IndexedText}[{ArgumentListText}]";
        }
        #endregion
    }
}
