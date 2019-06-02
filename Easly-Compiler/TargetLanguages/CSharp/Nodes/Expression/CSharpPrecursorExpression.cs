namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpPrecursorExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IPrecursorExpression Source { get; }

        /// <summary>
        /// The feature whose precursor is being called.
        /// </summary>
        ICSharpFeatureWithName Feature { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        ICSharpFeatureCall FeatureCall { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpPrecursorExpression : CSharpExpression, ICSharpPrecursorExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpPrecursorExpression Create(ICSharpContext context, IPrecursorExpression source)
        {
            return new CSharpPrecursorExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpPrecursorExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpPrecursorExpression(ICSharpContext context, IPrecursorExpression source)
            : base(context, source)
        {
            Feature = context.GetFeature((ICompiledFeature)source.EmbeddingFeature) as ICSharpFeatureWithName;
            Debug.Assert(Feature != null);

            FeatureCall = new CSharpFeatureCall(context, source.SelectedParameterList, source.ArgumentList, source.ArgumentStyle);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IPrecursorExpression Source { get { return (IPrecursorExpression)base.Source; } }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return false; } }

        /// <summary>
        /// The feature whose precursor is being called.
        /// </summary>
        public ICSharpFeatureWithName Feature { get; }

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
        public override string CSharpText(string cSharpNamespace, IList<ICSharpQualifiedName> destinationList)
        {
            string CoexistingPrecursorName = string.Empty;
            string CoexistingPrecursorRootName = Feature.CoexistingPrecursorName;

            if (!string.IsNullOrEmpty(CoexistingPrecursorRootName))
                CoexistingPrecursorName = CSharpNames.ToCSharpIdentifier(CoexistingPrecursorRootName + " " + "Base");

            string ArgumentListText = CSharpArgument.CSharpArgumentList(cSharpNamespace, FeatureCall, destinationList);

            if (!string.IsNullOrEmpty(CoexistingPrecursorRootName))
                return CoexistingPrecursorName + "(" + ArgumentListText + ")";
            else
            {
                string FunctionName = CSharpNames.ToCSharpIdentifier(Feature.Name);
                return "base" + "." + FunctionName + "(" + ArgumentListText + ")";
            }
        }
        #endregion
    }
}
