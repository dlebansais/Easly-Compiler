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
        /// The parent feature.
        /// </summary>
        ICSharpFeature ParentFeature { get; }

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
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IInstruction source)
        {
            ICSharpInstruction Result = null;

            switch (source)
            {
                case IAsLongAsInstruction AsAsLongAsInstruction:
                    Result = CSharpAsLongAsInstruction.Create(context, parentFeature, AsAsLongAsInstruction);
                    break;

                case IAssignmentInstruction AsAssignmentInstruction:
                    Result = CSharpAssignmentInstruction.Create(context, parentFeature, AsAssignmentInstruction);
                    break;

                case IAttachmentInstruction AsAttachmentInstruction:
                    Result = CSharpAttachmentInstruction.Create(context, parentFeature, AsAttachmentInstruction);
                    break;

                case ICheckInstruction AsCheckInstruction:
                    Result = CSharpCheckInstruction.Create(context, parentFeature, AsCheckInstruction);
                    break;

                case ICommandInstruction AsCommandInstruction:
                    Result = CSharpCommandInstruction.Create(context, parentFeature, AsCommandInstruction);
                    break;

                case ICreateInstruction AsCreateInstruction:
                    Result = CSharpCreateInstruction.Create(context, parentFeature, AsCreateInstruction);
                    break;

                case IDebugInstruction AsDebugInstruction:
                    Result = CSharpDebugInstruction.Create(context, parentFeature, AsDebugInstruction);
                    break;

                case IForLoopInstruction AsForLoopInstruction:
                    Result = CSharpForLoopInstruction.Create(context, parentFeature, AsForLoopInstruction);
                    break;

                case IIfThenElseInstruction AsIfThenElseInstruction:
                    Result = CSharpIfThenElseInstruction.Create(context, parentFeature, AsIfThenElseInstruction);
                    break;

                case IIndexAssignmentInstruction AsIndexAssignmentInstruction:
                    Result = CSharpIndexAssignmentInstruction.Create(context, parentFeature, AsIndexAssignmentInstruction);
                    break;

                case IInspectInstruction AsInspectInstruction:
                    Result = CSharpInspectInstruction.Create(context, parentFeature, AsInspectInstruction);
                    break;

                case IKeywordAssignmentInstruction AsKeywordAssignmentInstruction:
                    Result = CSharpKeywordAssignmentInstruction.Create(context, parentFeature, AsKeywordAssignmentInstruction);
                    break;

                case IOverLoopInstruction AsOverLoopInstruction:
                    Result = CSharpOverLoopInstruction.Create(context, parentFeature, AsOverLoopInstruction);
                    break;

                case IPrecursorIndexAssignmentInstruction AsPrecursorIndexAssignmentInstruction:
                    Result = CSharpPrecursorIndexAssignmentInstruction.Create(context, parentFeature, AsPrecursorIndexAssignmentInstruction);
                    break;

                case IPrecursorInstruction AsPrecursorInstruction:
                    Result = CSharpPrecursorInstruction.Create(context, parentFeature, AsPrecursorInstruction);
                    break;

                case IRaiseEventInstruction AsRaiseEventInstruction:
                    Result = CSharpRaiseEventInstruction.Create(context, parentFeature, AsRaiseEventInstruction);
                    break;

                case IReleaseInstruction AsReleaseInstruction:
                    Result = CSharpReleaseInstruction.Create(context, parentFeature, AsReleaseInstruction);
                    break;

                case IThrowInstruction AsThrowInstruction:
                    Result = CSharpThrowInstruction.Create(context, parentFeature, AsThrowInstruction);
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpInstruction(ICSharpContext context, ICSharpFeature parentFeature, IInstruction source)
        {
            Debug.Assert(source != null);

            ParentFeature = parentFeature;
            Source = source;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The parent feature.
        /// </summary>
        public ICSharpFeature ParentFeature { get; }

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
