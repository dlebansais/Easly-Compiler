namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpPrecursorExpression : ICSharpExpression, ICSharpExpressionAsConstant, ICSharpComputableExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IPrecursorExpression Source { get; }

        /// <summary>
        /// The precursor feature.
        /// </summary>
        ICSharpFeatureWithName PrecursorFeature { get; }

        /// <summary>
        /// The feature whose precursor is being called.
        /// </summary>
        ICSharpFeatureWithName ParentFeature { get; }

        /// <summary>
        /// The selected overload type. Can be null.
        /// </summary>
        ICSharpQueryOverloadType SelectedOverloadType { get; }

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
            PrecursorFeature = context.GetFeature(source.ResolvedPrecursor.Item.Feature) as ICSharpFeatureWithName;
            Debug.Assert(PrecursorFeature != null);

            ParentFeature = context.GetFeature((ICompiledFeature)source.EmbeddingFeature) as ICSharpFeatureWithName;
            Debug.Assert(ParentFeature != null);

            FeatureCall = new CSharpFeatureCall(context, source.FeatureCall.Item);

            if (Source.SelectedOverloadType.IsAssigned)
                SelectedOverloadType = CSharpQueryOverloadType.Create(context, Source.SelectedOverloadType.Item, PrecursorFeature.Owner);
            else
                SelectedOverloadType = null;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IPrecursorExpression Source { get { return (IPrecursorExpression)base.Source; } }

        /// <summary>
        /// The precursor feature.
        /// </summary>
        public ICSharpFeatureWithName PrecursorFeature { get; }

        /// <summary>
        /// The feature whose precursor is being called.
        /// </summary>
        public ICSharpFeatureWithName ParentFeature { get; }

        /// <summary>
        /// The selected overload type. Can be null.
        /// </summary>
        public ICSharpQueryOverloadType SelectedOverloadType { get; }

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
            string CoexistingPrecursorName = string.Empty;
            string CoexistingPrecursorRootName = ParentFeature.CoexistingPrecursorName;

            if (!string.IsNullOrEmpty(CoexistingPrecursorRootName))
                CoexistingPrecursorName = CSharpNames.ToCSharpIdentifier(CoexistingPrecursorRootName + " " + "Base");

            PrecursorFeature.GetOutputFormat(SelectedOverloadType, out int OutgoingParameterCount, out int ReturnValueIndex);

            CSharpArgument.CSharpArgumentList(writer, expressionContext, FeatureCall, ReturnValueIndex, false, out string ArgumentListText, out IList<string> OutgoingResultList);
            Debug.Assert(OutgoingParameterCount > 0);

            bool HasArguments = (ParentFeature is ICSharpFunctionFeature) || FeatureCall.ArgumentList.Count > 0;
            if (HasArguments)
                ArgumentListText = $"({ArgumentListText})";

            if (!string.IsNullOrEmpty(CoexistingPrecursorRootName))
                expressionContext.SetSingleReturnValue($"{CoexistingPrecursorName}{ArgumentListText}");
            else
            {
                string FunctionName = CSharpNames.ToCSharpIdentifier(ParentFeature.Name);

                if (OutgoingParameterCount == 1)
                {
                    if (ArgumentListText.Length > 0)
                        expressionContext.SetSingleReturnValue($"base.{FunctionName}{ArgumentListText}");
                    else if (SelectedOverloadType != null)
                        expressionContext.SetSingleReturnValue($"base.{FunctionName}()");
                    else
                        expressionContext.SetSingleReturnValue($"base.{FunctionName}");
                }
                else
                {
                    if (ReturnValueIndex >= 0)
                    {
                        string TemporaryResultName = writer.GetTemporaryName();
                        writer.WriteIndentedLine($"var {TemporaryResultName} = base.{FunctionName}{ArgumentListText};");

                        OutgoingResultList.Insert(ReturnValueIndex, TemporaryResultName);
                    }
                    else
                        writer.WriteIndentedLine($"base.{FunctionName}{ArgumentListText};");

                    expressionContext.SetMultipleResult(OutgoingResultList, ReturnValueIndex);
                }
            }
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
            ComputedValue = CSharpQueryExpression.ComputeQueryResult(writer, ParentFeature, FeatureCall);
        }
        #endregion
    }
}
