namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpDebugInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IDebugInstruction Source { get; }

        /// <summary>
        /// The debugged instructions.
        /// </summary>
        ICSharpScope Instructions { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpDebugInstruction : CSharpInstruction, ICSharpDebugInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpDebugInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IDebugInstruction source)
        {
            return new CSharpDebugInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpDebugInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpDebugInstruction(ICSharpContext context, ICSharpFeature parentFeature, IDebugInstruction source)
            : base(context, parentFeature, source)
        {
            Instructions = CSharpScope.Create(context, parentFeature, (IScope)source.Instructions);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IDebugInstruction Source { get { return (IDebugInstruction)base.Source; } }

        /// <summary>
        /// The debugged instructions.
        /// </summary>
        public ICSharpScope Instructions { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        public override void WriteCSharp(ICSharpWriter writer, string outputNamespace)
        {
            writer.WriteIndentedLine("#if DEBUG");
            Instructions.WriteCSharp(writer, outputNamespace, CSharpCurlyBracketsInsertions.Indifferent, false);
            writer.WriteIndentedLine("#endif");
        }
        #endregion
    }
}
