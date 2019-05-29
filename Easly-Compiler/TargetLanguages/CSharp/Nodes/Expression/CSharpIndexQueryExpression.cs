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
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpIndexQueryExpression Create(IIndexQueryExpression source, ICSharpContext context)
        {
            return new CSharpIndexQueryExpression(source, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpIndexQueryExpression"/> class.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpIndexQueryExpression(IIndexQueryExpression source, ICSharpContext context)
            : base(source, context)
        {
            IndexedExpression = Create((IExpression)source.IndexedExpression, context);
            FeatureCall = new CSharpFeatureCall(source.SelectedParameterList, source.ArgumentList, source.ArgumentStyle, context);
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
            string IndexedText = IndexedExpression.CSharpText(cSharpNamespace);
            string ArgumentListText = CSharpArgument.CSharpArgumentList(cSharpNamespace, FeatureCall, destinationList);

            return $"{IndexedText}[{ArgumentListText}]";
        }
        #endregion
    }
}
