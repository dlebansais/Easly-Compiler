namespace EaslyCompiler
{
    using System.Collections.Generic;
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

        /// <summary>
        /// The list of conditions.
        /// </summary>
        IList<ICSharpConditional> ConditionalList { get; }

        /// <summary>
        /// Instructions for the else case. Can be null.
        /// </summary>
        ICSharpScope ElseInstructions { get; }
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
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpIfThenElseInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IIfThenElseInstruction source)
        {
            return new CSharpIfThenElseInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpIfThenElseInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpIfThenElseInstruction(ICSharpContext context, ICSharpFeature parentFeature, IIfThenElseInstruction source)
            : base(context, parentFeature, source)
        {
            foreach (IConditional Conditional in source.ConditionalList)
            {
                ICSharpConditional NewConditional = CSharpConditional.Create(context, parentFeature, Conditional);
                ConditionalList.Add(NewConditional);
            }

            if (source.ElseInstructions.IsAssigned)
                ElseInstructions = CSharpScope.Create(context, parentFeature, (IScope)source.ElseInstructions.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IIfThenElseInstruction Source { get { return (IIfThenElseInstruction)base.Source; } }

        /// <summary>
        /// The list of conditions.
        /// </summary>
        public IList<ICSharpConditional> ConditionalList { get; } = new List<ICSharpConditional>();

        /// <summary>
        /// Instructions for the else case. Can be null.
        /// </summary>
        public ICSharpScope ElseInstructions { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public override void WriteCSharp(ICSharpWriter writer)
        {
            bool IsElseIf = false;

            foreach (ICSharpConditional Item in ConditionalList)
            {
                if (IsElseIf)
                    writer.WriteEmptyLine();

                Item.WriteCSharp(writer, IsElseIf);
                IsElseIf = true;
            }

            if (ElseInstructions != null)
            {
                writer.WriteIndentedLine("else");
                ElseInstructions.WriteCSharp(writer, CSharpCurlyBracketsInsertions.Indifferent, false);
            }
        }
        #endregion
    }
}
