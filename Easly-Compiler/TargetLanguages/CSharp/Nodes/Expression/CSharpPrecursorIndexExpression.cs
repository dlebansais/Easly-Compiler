﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpPrecursorIndexExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IPrecursorIndexExpression Source { get; }

        /// <summary>
        /// The feature whose precursor is being called.
        /// </summary>
        ICSharpIndexerFeature Feature { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        ICSharpFeatureCall FeatureCall { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpPrecursorIndexExpression : CSharpExpression, ICSharpPrecursorIndexExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpPrecursorIndexExpression Create(IPrecursorIndexExpression source, ICSharpContext context)
        {
            return new CSharpPrecursorIndexExpression(source, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpPrecursorIndexExpression"/> class.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpPrecursorIndexExpression(IPrecursorIndexExpression source, ICSharpContext context)
            : base(source, context)
        {
            Feature = context.GetFeature((ICompiledFeature)source.EmbeddingFeature) as ICSharpIndexerFeature;
            Debug.Assert(Feature != null);

            FeatureCall = new CSharpFeatureCall(source.SelectedParameterList, source.ArgumentList, source.ArgumentStyle, context);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IPrecursorIndexExpression Source { get { return (IPrecursorIndexExpression)base.Source; } }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return false; } }

        /// <summary>
        /// The feature whose precursor is being called.
        /// </summary>
        public ICSharpIndexerFeature Feature { get; }

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
            string ArgumentListText = CSharpArgument.CSharpArgumentList(cSharpNamespace, FeatureCall, destinationList);

            return $"base[{ArgumentListText}]";
        }
        #endregion
    }
}