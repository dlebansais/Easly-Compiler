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
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpRaiseEventInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IRaiseEventInstruction source)
        {
            return new CSharpRaiseEventInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpRaiseEventInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpRaiseEventInstruction(ICSharpContext context, ICSharpFeature parentFeature, IRaiseEventInstruction source)
            : base(context, parentFeature, source)
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
        public override void WriteCSharp(ICSharpWriter writer)
        {
            // TODO: declare the event

            IIdentifier QueryIdentifierItem = (IIdentifier)Source.QueryIdentifier;
            string QueryIdentifierItemString = CSharpNames.ToCSharpIdentifier(QueryIdentifierItem.ValidText.Item);

            writer.WriteIndentedLine($"{QueryIdentifierItemString}.set();");
        }
        #endregion
    }
}
