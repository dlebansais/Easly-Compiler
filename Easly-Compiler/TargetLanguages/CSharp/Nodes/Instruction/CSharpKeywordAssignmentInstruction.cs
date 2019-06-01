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
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpKeywordAssignmentInstruction Create(ICSharpContext context, IKeywordAssignmentInstruction source)
        {
            return new CSharpKeywordAssignmentInstruction(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpKeywordAssignmentInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpKeywordAssignmentInstruction(ICSharpContext context, IKeywordAssignmentInstruction source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IKeywordAssignmentInstruction Source { get { return (IKeywordAssignmentInstruction)base.Source; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        public override void WriteCSharp(ICSharpWriter writer, string outputNamespace)
        {
            //TODO
        }
        #endregion
    }
}
