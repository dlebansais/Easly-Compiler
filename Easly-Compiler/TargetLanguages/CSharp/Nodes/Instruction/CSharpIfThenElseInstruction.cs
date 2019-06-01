namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpIfThenElseInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IIfThenElseInstruction Source { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpIfThenElseInstruction : CSharpInstruction, ICSharpIfThenElseInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpIfThenElseInstruction Create(ICSharpContext context, IIfThenElseInstruction source)
        {
            return new CSharpIfThenElseInstruction(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpIfThenElseInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpIfThenElseInstruction(ICSharpContext context, IIfThenElseInstruction source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IIfThenElseInstruction Source { get { return (IIfThenElseInstruction)base.Source; } }
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
