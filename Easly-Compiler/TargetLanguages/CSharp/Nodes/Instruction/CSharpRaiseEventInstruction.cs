namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpRaiseEventInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IRaiseEventInstruction Source { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpRaiseEventInstruction : CSharpInstruction, ICSharpRaiseEventInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpRaiseEventInstruction Create(ICSharpContext context, IRaiseEventInstruction source)
        {
            return new CSharpRaiseEventInstruction(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpRaiseEventInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpRaiseEventInstruction(ICSharpContext context, IRaiseEventInstruction source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IRaiseEventInstruction Source { get { return (IRaiseEventInstruction)base.Source; } }
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
