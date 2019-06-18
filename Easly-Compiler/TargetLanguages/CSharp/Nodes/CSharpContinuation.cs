namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# continuation node.
    /// </summary>
    public interface ICSharpContinuation : ICSharpSource<IContinuation>
    {
        /// <summary>
        /// The parent feature.
        /// </summary>
        ICSharpFeature ParentFeature { get; }

        /// <summary>
        /// The continuation instructions.
        /// </summary>
        ICSharpScope Instructions { get; }

        /// <summary>
        /// The list of cleanup instructions.
        /// </summary>
        IList<ICSharpInstruction> CleanupList { get; }

        /// <summary>
        /// Writes down the C# continuation instructions.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        void WriteCSharpInstructions(ICSharpWriter writer);

        /// <summary>
        /// Writes down the C# continuation cleanup instructions.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        void WriteCSharpCleanupInstructions(ICSharpWriter writer);
    }

    /// <summary>
    /// A C# continuation node.
    /// </summary>
    public class CSharpContinuation : CSharpSource<IContinuation>, ICSharpContinuation
    {
        #region Init
        /// <summary>
        /// Create a new C# continuation.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        public static ICSharpContinuation Create(ICSharpContext context, ICSharpFeature parentFeature, IContinuation source)
        {
            return new CSharpContinuation(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpContinuation"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpContinuation(ICSharpContext context, ICSharpFeature parentFeature, IContinuation source)
            : base(source)
        {
            ParentFeature = parentFeature;

            Instructions = CSharpScope.Create(context, parentFeature, (IScope)source.Instructions);

            foreach (IInstruction Instruction in source.CleanupList)
            {
                ICSharpInstruction NewInstruction = CSharpInstruction.Create(context, parentFeature, Instruction);
                CleanupList.Add(NewInstruction);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The parent feature.
        /// </summary>
        public ICSharpFeature ParentFeature { get; }

        /// <summary>
        /// The continuation instructions.
        /// </summary>
        public ICSharpScope Instructions { get; }

        /// <summary>
        /// The list of cleanup instructions.
        /// </summary>
        public IList<ICSharpInstruction> CleanupList { get; } = new List<ICSharpInstruction>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# continuation instructions.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public virtual void WriteCSharpInstructions(ICSharpWriter writer)
        {
            Instructions.WriteCSharp(writer, CSharpCurlyBracketsInsertions.Indifferent, false);
        }

        /// <summary>
        /// Writes down the C# continuation cleanup instructions.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public virtual void WriteCSharpCleanupInstructions(ICSharpWriter writer)
        {
            foreach (ICSharpInstruction Item in CleanupList)
                Item.WriteCSharp(writer);
        }
        #endregion
    }
}
