﻿namespace EaslyCompiler
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
        ICSharpFeatureWithName ParentFeature { get; }

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
            ParentFeature = context.GetFeature((ICompiledFeature)source.EmbeddingFeature) as ICSharpFeatureWithName;
            Debug.Assert(ParentFeature != null);

            FeatureCall = new CSharpFeatureCall(context, source.FeatureCall.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IPrecursorExpression Source { get { return (IPrecursorExpression)base.Source; } }

        /// <summary>
        /// The feature whose precursor is being called.
        /// </summary>
        public ICSharpFeatureWithName ParentFeature { get; }

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
            string CoexistingPrecursorName = string.Empty;
            string CoexistingPrecursorRootName = ParentFeature.CoexistingPrecursorName;

            if (!string.IsNullOrEmpty(CoexistingPrecursorRootName))
                CoexistingPrecursorName = CSharpNames.ToCSharpIdentifier(CoexistingPrecursorRootName + " " + "Base");

            string ArgumentListText = CSharpArgument.CSharpArgumentList(usingCollection, FeatureCall, destinationList, skippedIndex);

            bool HasArguments = (ParentFeature is ICSharpFunctionFeature) || FeatureCall.ArgumentList.Count > 0;
            if (HasArguments)
                ArgumentListText = $"({ArgumentListText})";

            if (!string.IsNullOrEmpty(CoexistingPrecursorRootName))
                return $"{CoexistingPrecursorName}{ArgumentListText}";
            else
            {
                string FunctionName = CSharpNames.ToCSharpIdentifier(ParentFeature.Name);
                return $"base.{FunctionName}{ArgumentListText}";
            }
        }
        #endregion
    }
}
