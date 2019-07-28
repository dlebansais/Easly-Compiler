namespace EaslyCompiler
{
    using CompilerNode;
    using System.Diagnostics;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpCheckInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new ICheckInstruction Source { get; }

        /// <summary>
        /// The checked expression.
        /// </summary>
        ICSharpExpression BooleanExpression { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpCheckInstruction : CSharpInstruction, ICSharpCheckInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpCheckInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, ICheckInstruction source)
        {
            return new CSharpCheckInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCheckInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpCheckInstruction(ICSharpContext context, ICSharpFeature parentFeature, ICheckInstruction source)
            : base(context, parentFeature, source)
        {
            BooleanExpression = CSharpExpression.Create(context, (IExpression)source.BooleanExpression);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new ICheckInstruction Source { get { return (ICheckInstruction)base.Source; } }

        /// <summary>
        /// The checked expression.
        /// </summary>
        public ICSharpExpression BooleanExpression { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public override void WriteCSharp(ICSharpWriter writer)
        {
            Debug.Assert(WriteDown);

            ICSharpExpressionContext SourceExpressionContext = new CSharpExpressionContext();
            BooleanExpression.WriteCSharp(writer, SourceExpressionContext, -1);

            string ExpressionText = SourceExpressionContext.ReturnValue;

            if (BooleanExpression.IsEventExpression)
                if (BooleanExpression.IsComplex)
                    ExpressionText = $"({ExpressionText}).IsSignaled";
                else
                    ExpressionText += ".IsSignaled";

            writer.WriteIndentedLine($"Debug.Assert({ExpressionText});");
            writer.AddUsing("System.Diagnostics");
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

            BooleanExpression.SetWriteDown();
        }
        #endregion
    }
}
