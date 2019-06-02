namespace EaslyCompiler
{
    using CompilerNode;

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
        /// <param name="outputNamespace">Namespace for the output code.</param>
        public override void WriteCSharp(ICSharpWriter writer, string outputNamespace)
        {
            string ExpressionText = BooleanExpression.CSharpText(outputNamespace);

            writer.WriteIndentedLine("Debug.Assert({ExpressionText});");
        }
        #endregion
    }
}
