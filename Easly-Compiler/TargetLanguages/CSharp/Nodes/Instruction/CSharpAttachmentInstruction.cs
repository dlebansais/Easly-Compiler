namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpAttachmentInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IAttachmentInstruction Source { get; }

        /// <summary>
        /// The expression source to the attachment.
        /// </summary>
        ICSharpExpression SourceExpression { get; }

        /// <summary>
        /// List of attached entities.
        /// </summary>
        IList<string> EntityNameList { get; }

        /// <summary>
        /// List of attachments.
        /// </summary>
        IList<ICSharpAttachment> AttachmentList { get; }

        /// <summary>
        /// Instructions for the else case. Can be null.
        /// </summary>
        ICSharpScope ElseInstructions { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpAttachmentInstruction : CSharpInstruction, ICSharpAttachmentInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpAttachmentInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IAttachmentInstruction source)
        {
            return new CSharpAttachmentInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAttachmentInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpAttachmentInstruction(ICSharpContext context, ICSharpFeature parentFeature, IAttachmentInstruction source)
            : base(context, parentFeature, source)
        {
            SourceExpression = CSharpExpression.Create(context, (IExpression)source.Source);

            IResultType ResolvedResult = SourceExpression.Source.ResolvedResult.Item;

            for (int i = 0; i < source.EntityNameList.Count; i++)
            {
                IName EntityName = source.EntityNameList[i];

                string ValidName = EntityName.ValidText.Item;
                EntityNameList.Add(ValidName);
            }

            foreach (IAttachment Attachment in source.AttachmentList)
            {
                ICSharpAttachment NewAttachment = CSharpAttachment.Create(context, this, Attachment);
                AttachmentList.Add(NewAttachment);
            }

            if (source.ElseInstructions.IsAssigned)
                ElseInstructions = CSharpScope.Create(context, parentFeature, (IScope)source.ElseInstructions.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IAttachmentInstruction Source { get { return (IAttachmentInstruction)base.Source; } }

        /// <summary>
        /// The expression source to the attachment.
        /// </summary>
        public ICSharpExpression SourceExpression { get; }

        /// <summary>
        /// List of attached entities.
        /// </summary>
        public IList<string> EntityNameList { get; } = new List<string>();

        /// <summary>
        /// List of attachments.
        /// </summary>
        public IList<ICSharpAttachment> AttachmentList { get; } = new List<ICSharpAttachment>();

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
            if (EntityNameList.Count == 1)
                WriteCSharpSwitch(writer);
            else
                WriteCSharpIf(writer);
        }

        private void WriteCSharpSwitch(ICSharpWriter writer)
        {
            ICSharpExpressionContext ExpressionContext = new CSharpExpressionContext(EntityNameList);
            SourceExpression.WriteCSharp(writer, ExpressionContext, true, true, -1);

            Debug.Assert(ExpressionContext.FilledDestinationTable.Count == 1);
            string EntityName = EntityNameList[0];
            string LastExpressionText = ExpressionContext.FilledDestinationTable[EntityName];
            if (LastExpressionText == null)
                LastExpressionText = ExpressionContext.ReturnValue;

            writer.WriteIndentedLine($"switch ({LastExpressionText})");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();

            for (int i = 0; i < AttachmentList.Count; i++)
            {
                ICSharpAttachment Attachment = AttachmentList[i];
                Attachment.WriteCSharpCase(writer, EntityName);
            }

            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");
        }

        private void WriteCSharpIf(ICSharpWriter writer)
        {
            ICSharpExpressionContext ExpressionContext = new CSharpExpressionContext(EntityNameList);
            SourceExpression.WriteCSharp(writer, ExpressionContext, true, true, -1);

            for (int i = 0; i < AttachmentList.Count; i++)
            {
                ICSharpAttachment Attachment = AttachmentList[i];
                Attachment.WriteCSharpIf(writer, i, EntityNameList);
            }

            if (ElseInstructions != null)
            {
                writer.WriteIndentedLine("else");
                ElseInstructions.WriteCSharp(writer, CSharpCurlyBracketsInsertions.Mandatory, false);
            }
        }
        #endregion
    }
}
