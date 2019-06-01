namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        IInstruction Source { get; }

        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        void WriteCSharp(ICSharpWriter writer, string outputNamespace);
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public abstract class CSharpInstruction : ICSharpInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpInstruction Create(ICSharpContext context, IInstruction source)
        {
            ICSharpInstruction Result = null;

            switch (source)
            {
                case IAsLongAsInstruction AsAsLongAsInstruction:
                    Result = CSharpAsLongAsInstruction.Create(context, AsAsLongAsInstruction);
                    break;

                case IAssignmentInstruction AsAssignmentInstruction:
                    Result = CSharpAssignmentInstruction.Create(context, AsAssignmentInstruction);
                    break;

                case IAttachmentInstruction AsAttachmentInstruction:
                    Result = CSharpAttachmentInstruction.Create(context, AsAttachmentInstruction);
                    break;

                case ICheckInstruction AsCheckInstruction:
                    Result = CSharpCheckInstruction.Create(context, AsCheckInstruction);
                    break;

                case ICommandInstruction AsCommandInstruction:
                    Result = CSharpCommandInstruction.Create(context, AsCommandInstruction);
                    break;

                case ICreateInstruction AsCreateInstruction:
                    Result = CSharpCreateInstruction.Create(context, AsCreateInstruction);
                    break;

                case IDebugInstruction AsDebugInstruction:
                    Result = CSharpDebugInstruction.Create(context, AsDebugInstruction);
                    break;

                case IForLoopInstruction AsForLoopInstruction:
                    Result = CSharpForLoopInstruction.Create(context, AsForLoopInstruction);
                    break;

                case IIfThenElseInstruction AsIfThenElseInstruction:
                    Result = CSharpIfThenElseInstruction.Create(context, AsIfThenElseInstruction);
                    break;

                case IIndexAssignmentInstruction AsIndexAssignmentInstruction:
                    Result = CSharpIndexAssignmentInstruction.Create(context, AsIndexAssignmentInstruction);
                    break;

                case IInspectInstruction AsInspectInstruction:
                    Result = CSharpInspectInstruction.Create(context, AsInspectInstruction);
                    break;

                case IKeywordAssignmentInstruction AsKeywordAssignmentInstruction:
                    Result = CSharpKeywordAssignmentInstruction.Create(context, AsKeywordAssignmentInstruction);
                    break;

                case IOverLoopInstruction AsOverLoopInstruction:
                    Result = CSharpOverLoopInstruction.Create(context, AsOverLoopInstruction);
                    break;

                case IPrecursorIndexAssignmentInstruction AsPrecursorIndexAssignmentInstruction:
                    Result = CSharpPrecursorIndexAssignmentInstruction.Create(context, AsPrecursorIndexAssignmentInstruction);
                    break;

                case IPrecursorInstruction AsPrecursorInstruction:
                    Result = CSharpPrecursorInstruction.Create(context, AsPrecursorInstruction);
                    break;

                case IRaiseEventInstruction AsRaiseEventInstruction:
                    Result = CSharpRaiseEventInstruction.Create(context, AsRaiseEventInstruction);
                    break;

                case IReleaseInstruction AsReleaseInstruction:
                    Result = CSharpReleaseInstruction.Create(context, AsReleaseInstruction);
                    break;

                case IThrowInstruction AsThrowInstruction:
                    Result = CSharpThrowInstruction.Create(context, AsThrowInstruction);
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpInstruction(ICSharpContext context, IInstruction source)
        {
            Debug.Assert(source != null);

            Source = source;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public IInstruction Source { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        public abstract void WriteCSharp(ICSharpWriter writer, string outputNamespace);
        #endregion
    }
}
