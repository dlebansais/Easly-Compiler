namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpManifestCharacterExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IManifestCharacterExpression Source { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpManifestCharacterExpression : CSharpExpression, ICSharpManifestCharacterExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpManifestCharacterExpression Create(ICSharpContext context, IManifestCharacterExpression source)
        {
            return new CSharpManifestCharacterExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpManifestCharacterExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpManifestCharacterExpression(ICSharpContext context, IManifestCharacterExpression source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IManifestCharacterExpression Source { get { return (IManifestCharacterExpression)base.Source; } }
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
            return $"'{Source.ValidText.Item}'";
        }
        #endregion
    }
}
