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
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpPrecursorIndexExpression Create(ICSharpContext context, IPrecursorIndexExpression source)
        {
            return new CSharpPrecursorIndexExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpPrecursorIndexExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpPrecursorIndexExpression(ICSharpContext context, IPrecursorIndexExpression source)
            : base(context, source)
        {
            Feature = context.GetFeature((ICompiledFeature)source.EmbeddingFeature) as ICSharpIndexerFeature;
            Debug.Assert(Feature != null);

            FeatureCall = new CSharpFeatureCall(context, source.FeatureCall.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IPrecursorIndexExpression Source { get { return (IPrecursorIndexExpression)base.Source; } }

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
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expressionContext">The context.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, bool isNeverSimple, bool isDeclaredInPlace, int skippedIndex)
        {
            CSharpArgument.CSharpArgumentList(writer, expressionContext, isNeverSimple, isDeclaredInPlace, FeatureCall, skippedIndex, false, out string ArgumentListText, out IList<string> OutgoingResultList);

            expressionContext.SetSingleReturnValue($"base[{ArgumentListText}]");
        }
        #endregion
    }
}
