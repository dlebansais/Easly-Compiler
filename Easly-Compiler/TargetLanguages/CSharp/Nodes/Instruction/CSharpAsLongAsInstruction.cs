namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpAsLongAsInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IAsLongAsInstruction Source { get; }

        /// <summary>
        /// The loop condition expression.
        /// </summary>
        ICSharpExpression ContinueCondition { get; }

        /// <summary>
        /// The list of C# continuations.
        /// </summary>
        IList<ICSharpContinuation> ContinuationList { get; }

        /// <summary>
        /// Instructions for the else case. Can be null.
        /// </summary>
        ICSharpScope ElseInstructions { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpAsLongAsInstruction : CSharpInstruction, ICSharpAsLongAsInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpAsLongAsInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IAsLongAsInstruction source)
        {
            return new CSharpAsLongAsInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAsLongAsInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpAsLongAsInstruction(ICSharpContext context, ICSharpFeature parentFeature, IAsLongAsInstruction source)
            : base(context, parentFeature, source)
        {
            ContinueCondition = CSharpExpression.Create(context, (IExpression)source.ContinueCondition);

            foreach (IContinuation Continuation in source.ContinuationList)
            {
                ICSharpContinuation NewContinuation = CSharpContinuation.Create(context, parentFeature, Continuation);
                ContinuationList.Add(NewContinuation);
            }

            if (source.ElseInstructions.IsAssigned)
                ElseInstructions = CSharpScope.Create(context, parentFeature, (IScope)source.ElseInstructions.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IAsLongAsInstruction Source { get { return (IAsLongAsInstruction)base.Source; } }

        /// <summary>
        /// The loop condition expression.
        /// </summary>
        public ICSharpExpression ContinueCondition { get; }

        /// <summary>
        /// The list of C# continuations.
        /// </summary>
        public IList<ICSharpContinuation> ContinuationList { get; } = new List<ICSharpContinuation>();

        /// <summary>
        /// Instructions for the else case. Can be null.
        /// </summary>
        public ICSharpScope ElseInstructions { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public override void WriteCSharp(ICSharpWriter writer)
        {
            string ContinueExpressionText;

            for (int i = 0; i < ContinuationList.Count; i++)
            {
                ICSharpContinuation Item = ContinuationList[i];

                if (i > 0)
                    writer.WriteEmptyLine();

                WriteCSharpContinueCondition(writer, out ContinueExpressionText);
                writer.WriteIndentedLine($"if ({ContinueExpressionText})");

                Item.WriteCSharpInstructions(writer);

                int CleanupInstructionCount = 0;
                for (int j = i; j > 0; j--)
                {
                    ICSharpContinuation PreviousItem = ContinuationList[j - 1];
                    CleanupInstructionCount += PreviousItem.CleanupList.Count;
                }

                if (CleanupInstructionCount > 0)
                {
                    writer.WriteIndentedLine("else");

                    if (CleanupInstructionCount > 1)
                        writer.WriteIndentedLine("{");

                    writer.IncreaseIndent();

                    for (int j = i; j > 0; j--)
                    {
                        if (j < i)
                            writer.WriteEmptyLine();

                        ICSharpContinuation PreviousItem = ContinuationList[j - 1];
                        PreviousItem.WriteCSharpCleanupInstructions(writer);
                    }

                    writer.DecreaseIndent();

                    if (CleanupInstructionCount > 1)
                        writer.WriteIndentedLine("}");
                }
            }

            if (ElseInstructions != null)
            {
                writer.WriteEmptyLine();

                WriteCSharpContinueCondition(writer, out ContinueExpressionText);
                writer.WriteIndentedLine($"if (!({ContinueExpressionText}))");

                ElseInstructions.WriteCSharp(writer, CSharpCurlyBracketsInsertions.Mandatory, false);
            }
        }

        private void WriteCSharpContinueCondition(ICSharpWriter writer, out string continueExpressionText)
        {
            ICSharpExpressionContext ExpressionContext = new CSharpExpressionContext();
            ContinueCondition.WriteCSharp(writer, ExpressionContext, -1);

            if (ExpressionContext.CompleteDestinationNameList.Count > 1)
            {
                continueExpressionText = string.Empty;

                foreach (string DestinationName in ExpressionContext.CompleteDestinationNameList)
                {
                    if (continueExpressionText.Length > 0)
                        continueExpressionText += " && ";

                    continueExpressionText += DestinationName;
                }
            }
            else
            {
                Debug.Assert(ExpressionContext.ReturnValue != null);
                continueExpressionText = ExpressionContext.ReturnValue;
            }
        }
        #endregion
    }
}
