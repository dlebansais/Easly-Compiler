namespace EaslyCompiler
{
    using System.Collections.Generic;
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

            foreach (IName EntityName in source.EntityNameList)
            {
                string ValidName = EntityName.ValidText.Item;
                EntityNameList.Add(ValidName);
            }

            foreach (IAttachment Attachment in source.AttachmentList)
            {
                ICSharpAttachment NewAttachment = CSharpAttachment.Create(context, Attachment);
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
            string AttachedSourceString = SourceExpression.CSharpText(writer);

            string NameString = EntityNameList[0];

            bool IsElseIf = false;
            foreach (ICSharpAttachment Attachment in AttachmentList)
            {
                if (IsElseIf)
                    writer.WriteEmptyLine();

                ICSharpType FirstType = Attachment.AttachTypeList[0];

                /*TODO
                foreach (AttachmentAlias AliasItem in Context.AttachmentVariableTable)
                    if (AliasItem.SourceName == NameString && AliasItem.EntityType == ResolvedAttachmentType)
                    {
                        AttachmentItem.WriteCSharp(sw, Context, AliasItem, AttachedSourceString, Flags);
                        IsElseIf = true;
                        break;
                    }*/
            }

            writer.WriteIndentedLine("else");

            if (ElseInstructions != null)
                ElseInstructions.WriteCSharp(writer, CSharpCurlyBracketsInsertions.Indifferent, false);
            else
            {
                writer.IncreaseIndent();
                writer.WriteIndentedLine("throw new InvalidCastException();");
                writer.DecreaseIndent();
            }
        }
        #endregion
    }
}
