﻿namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpPrecursorIndexAssignmentInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IPrecursorIndexAssignmentInstruction Source { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpPrecursorIndexAssignmentInstruction : CSharpInstruction, ICSharpPrecursorIndexAssignmentInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpPrecursorIndexAssignmentInstruction Create(ICSharpContext context, IPrecursorIndexAssignmentInstruction source)
        {
            return new CSharpPrecursorIndexAssignmentInstruction(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpPrecursorIndexAssignmentInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpPrecursorIndexAssignmentInstruction(ICSharpContext context, IPrecursorIndexAssignmentInstruction source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IPrecursorIndexAssignmentInstruction Source { get { return (IPrecursorIndexAssignmentInstruction)base.Source; } }
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