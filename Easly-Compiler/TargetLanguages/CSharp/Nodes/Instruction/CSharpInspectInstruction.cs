namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpInspectInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IInspectInstruction Source { get; }

        /// <summary>
        /// The inspect source.
        /// </summary>
        ICSharpExpression SourceExpression { get; }

        /// <summary>
        /// The list of cases.
        /// </summary>
        IList<ICSharpWith> WithList { get; }

        /// <summary>
        /// Instructions for the else case. Can be null.
        /// </summary>
        ICSharpScope ElseInstructions { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpInspectInstruction : CSharpInstruction, ICSharpInspectInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpInspectInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IInspectInstruction source)
        {
            return new CSharpInspectInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpInspectInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpInspectInstruction(ICSharpContext context, ICSharpFeature parentFeature, IInspectInstruction source)
            : base(context, parentFeature, source)
        {
            SourceExpression = CSharpExpression.Create(context, (IExpression)source.Source);

            foreach (IWith With in source.WithList)
            {
                ICSharpWith NewWith = CSharpWith.Create(context, parentFeature, With);
                WithList.Add(NewWith);
            }

            if (source.ElseInstructions.IsAssigned)
                ElseInstructions = CSharpScope.Create(context, parentFeature, (IScope)source.ElseInstructions.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IInspectInstruction Source { get { return (IInspectInstruction)base.Source; } }

        /// <summary>
        /// The inspect source.
        /// </summary>
        public ICSharpExpression SourceExpression { get; }

        /// <summary>
        /// The list of cases.
        /// </summary>
        public IList<ICSharpWith> WithList { get; } = new List<ICSharpWith>();

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
        /// <param name="outputNamespace">Namespace for the output code.</param>
        public override void WriteCSharp(ICSharpWriter writer, string outputNamespace)
        {
            string SourceString = SourceExpression.CSharpText(outputNamespace, new List<ICSharpQualifiedName>());

            writer.WriteIndentedLine($"switch ({SourceString})");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();

            bool WithInserted = false;
            foreach (ICSharpWith Item in WithList)
            {
                if (!WithInserted)
                    WithInserted = true;
                else
                    writer.WriteLine();

                Item.WriteCSharp(writer, outputNamespace);
            }

            if (WithInserted)
                writer.WriteLine();

            writer.WriteIndentedLine("default:");

            if (ElseInstructions != null)
            {
                ElseInstructions.WriteCSharp(writer, outputNamespace, CSharpCurlyBracketsInsertions.Indifferent, false);
                writer.WriteIndentedLine("break;");
            }
            else
            {
                writer.IncreaseIndent();
                writer.WriteIndentedLine("throw new ArgumentOutOfRangeException();");
                writer.DecreaseIndent();
            }

            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");
        }
        #endregion
    }
}
