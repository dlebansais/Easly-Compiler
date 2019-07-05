namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpClassConstantExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IClassConstantExpression Source { get; }

        /// <summary>
        /// The constant feature.
        /// </summary>
        ICSharpConstantFeature Feature { get; }

        /// <summary>
        /// The constant discrete.
        /// </summary>
        ICSharpDiscrete Discrete { get; }

        /// <summary>
        /// The feature class.
        /// </summary>
        ICSharpClass Class { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpClassConstantExpression : CSharpExpression, ICSharpClassConstantExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpClassConstantExpression Create(ICSharpContext context, IClassConstantExpression source)
        {
            return new CSharpClassConstantExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpClassConstantExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpClassConstantExpression(ICSharpContext context, IClassConstantExpression source)
            : base(context, source)
        {
            if (source.ResolvedFinalFeature.IsAssigned)
                Feature = context.GetFeature(source.ResolvedFinalFeature.Item) as ICSharpConstantFeature;

            if (source.ResolvedFinalDiscrete.IsAssigned)
                Discrete = CSharpDiscrete.Create(context, source.ResolvedFinalDiscrete.Item);

            Debug.Assert((Feature != null && Discrete == null) || (Feature == null && Discrete != null));
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IClassConstantExpression Source { get { return (IClassConstantExpression)base.Source; } }

        /// <summary>
        /// The constant feature.
        /// </summary>
        public ICSharpConstantFeature Feature { get; }

        /// <summary>
        /// The constant discrete.
        /// </summary>
        public ICSharpDiscrete Discrete { get; }

        /// <summary>
        /// The feature class.
        /// </summary>
        public ICSharpClass Class { get { return Feature?.Owner; } }
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
            if (Feature != null)
                if (Class.ValidSourceName == "Microsoft .NET")
                    lastExpressionText = CSharpNames.ToDotNetIdentifier(Class.ValidClassName) + "." + CSharpNames.ToDotNetIdentifier(Feature.Name);
                else
                    lastExpressionText = CSharpNames.ToCSharpIdentifier(Class.ValidClassName) + "." + CSharpNames.ToCSharpIdentifier(Feature.Name);
            else
                lastExpressionText = CSharpNames.ToCSharpIdentifier(Discrete.Name);
        }
        #endregion
    }
}
