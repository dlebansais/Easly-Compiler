namespace EaslyCompiler
{
    using CompilerNode;
    using System.Diagnostics;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpReleaseInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IReleaseInstruction Source { get; }

        /// <summary>
        /// The released entity.
        /// </summary>
        ICSharpQualifiedName ReleasedEntity { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpReleaseInstruction : CSharpInstruction, ICSharpReleaseInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpReleaseInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IReleaseInstruction source)
        {
            return new CSharpReleaseInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpReleaseInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpReleaseInstruction(ICSharpContext context, ICSharpFeature parentFeature, IReleaseInstruction source)
            : base(context, parentFeature, source)
        {
            ReleasedEntity = CSharpQualifiedName.Create(context, (IQualifiedName)source.EntityName, ParentFeature, null, false);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IReleaseInstruction Source { get { return (IReleaseInstruction)base.Source; } }

        /// <summary>
        /// The released entity.
        /// </summary>
        public ICSharpQualifiedName ReleasedEntity { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public override void WriteCSharp(ICSharpWriter writer)
        {
            Debug.Assert(WriteDown);

            string ReleasedLink = ReleasedEntity.CSharpText(writer, 0);

            writer.WriteIndentedLine($"{ReleasedLink} = null;");
        }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// Sets the <see cref="ICSharpOutputNode.WriteDown"/> flag.
        /// </summary>
        public override void SetWriteDown()
        {
            WriteDown = true;
        }
        #endregion
    }
}
