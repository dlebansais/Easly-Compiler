namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpKeywordAssignmentInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IKeywordAssignmentInstruction Source { get; }

        /// <summary>
        /// The assignment source.
        /// </summary>
        ICSharpExpression SourceExpression { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpKeywordAssignmentInstruction : CSharpInstruction, ICSharpKeywordAssignmentInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpKeywordAssignmentInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IKeywordAssignmentInstruction source)
        {
            return new CSharpKeywordAssignmentInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpKeywordAssignmentInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpKeywordAssignmentInstruction(ICSharpContext context, ICSharpFeature parentFeature, IKeywordAssignmentInstruction source)
            : base(context, parentFeature, source)
        {
            SourceExpression = CSharpExpression.Create(context, (IExpression)source.Source);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IKeywordAssignmentInstruction Source { get { return (IKeywordAssignmentInstruction)base.Source; } }

        /// <summary>
        /// The assignment source.
        /// </summary>
        public ICSharpExpression SourceExpression { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public override void WriteCSharp(ICSharpWriter writer)
        {
            string DestinationString = Source.Destination.ToString();

            ICSharpExpressionContext SourceExpressionContext = new CSharpExpressionContext();
            SourceExpression.WriteCSharp(writer, SourceExpressionContext, false, -1);

            string SourceString = SourceExpressionContext.ReturnValue;

            writer.WriteIndentedLine($"{DestinationString} = {SourceString};");
        }
        #endregion
    }
}
