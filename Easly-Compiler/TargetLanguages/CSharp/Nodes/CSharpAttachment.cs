namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# attachment node.
    /// </summary>
    public interface ICSharpAttachment : ICSharpSource<IAttachment>
    {
        /// <summary>
        /// The parent instruction.
        /// </summary>
        ICSharpAttachmentInstruction ParentInstruction { get; }

        /// <summary>
        /// The list of attaching types.
        /// </summary>
        IList<ICSharpType> AttachTypeList { get; }

        /// <summary>
        /// The attachment instructions;
        /// </summary>
        ICSharpScope Instructions { get; }

        /// <summary>
        /// Writes down the C# attachment.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="index">Index of the attachment in the list.</param>
        /// <param name="destinationEntityList">The list of entities to attach.</param>
        void WriteCSharpIf(ICSharpWriter writer, int index, IList<string> destinationEntityList);

        /// <summary>
        /// Writes down the C# attachment.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="destinationEntity">The entity to attach.</param>
        void WriteCSharpCase(ICSharpWriter writer, string destinationEntity);
    }

    /// <summary>
    /// A C# attachment node.
    /// </summary>
    public class CSharpAttachment : CSharpSource<IAttachment>, ICSharpAttachment
    {
        #region Init
        /// <summary>
        /// Create a new C# attachment.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentInstruction">The parent instruction.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        public static ICSharpAttachment Create(ICSharpContext context, ICSharpAttachmentInstruction parentInstruction, IAttachment source)
        {
            return new CSharpAttachment(context, parentInstruction, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAttachment"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentInstruction">The parent instruction.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpAttachment(ICSharpContext context, ICSharpAttachmentInstruction parentInstruction, IAttachment source)
            : base(source)
        {
            ParentInstruction = parentInstruction;

            foreach (IScopeAttributeFeature Entity in source.ResolvedLocalEntitiesList)
            {
                ICSharpType NewType = CSharpType.Create(context, Entity.ResolvedEffectiveType.Item);
                AttachTypeList.Add(NewType);
            }

            Instructions = CSharpScope.Create(context, parentInstruction.ParentFeature, (IScope)source.Instructions);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The parent instruction.
        /// </summary>
        public ICSharpAttachmentInstruction ParentInstruction { get; }

        /// <summary>
        /// The list of attaching types.
        /// </summary>
        public IList<ICSharpType> AttachTypeList { get; } = new List<ICSharpType>();

        /// <summary>
        /// The attachment instructions;
        /// </summary>
        public ICSharpScope Instructions { get; }

        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# attachment.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="index">Index of the attachment in the list.</param>
        /// <param name="destinationEntityList">The list of entities to attach.</param>
        public void WriteCSharpIf(ICSharpWriter writer, int index, IList<string> destinationEntityList)
        {
            Debug.Assert(destinationEntityList.Count >= AttachTypeList.Count);

            string ElseIfText = index > 0 ? "else " : string.Empty;
            string AttachmentText = string.Empty;

            for (int i = 0; i < AttachTypeList.Count; i++)
            {
                string EntityText = $"Temp_{destinationEntityList[i]}";
                string AttachedEntityText = $"{destinationEntityList[i]}{index}";
                string TypeText = AttachTypeList[i].Type2CSharpString(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
                string NameAttached = $"As{TypeText}{AttachedEntityText}";

                string TypeAttachmentText = $"({EntityText} is {TypeText} {NameAttached})";

                if (AttachmentText.Length > 0)
                    AttachmentText += " && ";

                AttachmentText += TypeAttachmentText;

                writer.AddAttachment(destinationEntityList[i], NameAttached);
            }

            string IfLine = $"{ElseIfText}if ({AttachmentText})";

            writer.WriteIndentedLine(IfLine);
            Instructions.WriteCSharp(writer, CSharpCurlyBracketsInsertions.Mandatory, false);

            for (int i = 0; i < AttachTypeList.Count; i++)
                writer.RemoveAttachment(destinationEntityList[i]);
        }

        /// <summary>
        /// Writes down the C# attachment.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="destinationEntity">The entity to attach.</param>
        public void WriteCSharpCase(ICSharpWriter writer, string destinationEntity)
        {
            Debug.Assert(AttachTypeList.Count == 1);

            string TypeText = AttachTypeList[0].Type2CSharpString(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
            string NameAttached = $"As{TypeText}";
            string AttachmentLine = $"case {TypeText} {NameAttached}:";

            writer.WriteIndentedLine(AttachmentLine);

            writer.AddAttachment(destinationEntity, NameAttached);
            Instructions.WriteCSharp(writer, CSharpCurlyBracketsInsertions.AlreadyInserted, true);
            writer.RemoveAttachment(destinationEntity);
        }
        #endregion
    }
}
