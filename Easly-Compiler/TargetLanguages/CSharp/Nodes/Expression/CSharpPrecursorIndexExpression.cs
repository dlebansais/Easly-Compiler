namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpPrecursorIndexExpression : ICSharpExpression, ICSharpExpressionAsConstant, ICSharpComputableExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IPrecursorIndexExpression Source { get; }

        /// <summary>
        /// The feature whose precursor is being called.
        /// </summary>
        ICSharpIndexerFeature ParentFeature { get; }

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
            ParentFeature = context.GetFeature((ICompiledFeature)source.EmbeddingFeature) as ICSharpIndexerFeature;
            Debug.Assert(ParentFeature != null);

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
        public ICSharpIndexerFeature ParentFeature { get; }

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
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex)
        {
            Debug.Assert(WriteDown);

            CSharpArgument.CSharpArgumentList(writer, expressionContext, FeatureCall, skippedIndex, false, out string ArgumentListText, out IList<string> OutgoingResultList);

            expressionContext.SetSingleReturnValue($"base[{ArgumentListText}]");
        }
        #endregion

        #region Implementation of ICSharpExpressionAsConstant
        /// <summary>
        /// True if the expression can provide its constant value directly.
        /// </summary>
        public bool IsDirectConstant { get { return false; } }
        #endregion

        #region Implementation of ICSharpComputableExpression
        /// <summary>
        /// The expression computed constant value.
        /// </summary>
        public string ComputedValue { get; private set; }

        /// <summary>
        /// Runs the compiler to compute the value as a string.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public void Compute(ICSharpWriter writer)
        {
            //TODO use the precusor instead
            ComputedValue = CSharpQueryExpression.ComputeQueryResult(writer, ParentFeature as ICSharpFeatureWithName, FeatureCall);
        }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// Sets the <see cref="ICSharpOutputNode.WriteDown"/> flag.
        /// </summary>
        public override void SetWriteDown()
        {
            if (WriteDown)
                return;

            WriteDown = true;

            FeatureCall.SetWriteDown();
        }
        #endregion
    }
}
