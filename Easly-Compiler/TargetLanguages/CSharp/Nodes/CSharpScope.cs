namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# scope node.
    /// </summary>
    public interface ICSharpScope : ICSharpSource<IScope>, ICSharpOutputNode
    {
        /// <summary>
        /// The parent feature.
        /// </summary>
        ICSharpFeature ParentFeature { get; }

        /// <summary>
        /// The list of local variables.
        /// </summary>
        IList<ICSharpScopeAttributeFeature> EntityDeclarationList { get; }

        /// <summary>
        /// The list of instructions.
        /// </summary>
        IList<ICSharpInstruction> InstructionList { get; }

        /// <summary>
        /// Writes down the C# scope.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="curlyBracketsInsertion">The mode to use to write the C# scope..</param>
        /// <param name="endWithBreak">Add a break instruction at the end.</param>
        void WriteCSharp(ICSharpWriter writer, CSharpCurlyBracketsInsertions curlyBracketsInsertion, bool endWithBreak);
    }

    /// <summary>
    /// A C# scope node.
    /// </summary>
    public class CSharpScope : CSharpSource<IScope>, ICSharpScope
    {
        #region Init
        /// <summary>
        /// Create a new C# scope.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        public static ICSharpScope Create(ICSharpContext context, ICSharpFeature parentFeature, IScope source)
        {
            return new CSharpScope(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpScope"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpScope(ICSharpContext context, ICSharpFeature parentFeature, IScope source)
            : base(source)
        {
            ParentFeature = parentFeature;

            foreach (IEntityDeclaration Declaration in source.EntityDeclarationList)
            {
                ICSharpScopeAttributeFeature NewDeclaration = CSharpScopeAttributeFeature.Create(context, parentFeature.Owner, Declaration.ValidEntity.Item);
                EntityDeclarationList.Add(NewDeclaration);
            }

            foreach (IInstruction Instruction in source.InstructionList)
            {
                ICSharpInstruction NewInstruction = CSharpInstruction.Create(context, parentFeature, Instruction);
                InstructionList.Add(NewInstruction);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The parent feature.
        /// </summary>
        public ICSharpFeature ParentFeature { get; }

        /// <summary>
        /// The list of local variables.
        /// </summary>
        public IList<ICSharpScopeAttributeFeature> EntityDeclarationList { get; } = new List<ICSharpScopeAttributeFeature>();

        /// <summary>
        /// The list of instructions.
        /// </summary>
        public IList<ICSharpInstruction> InstructionList { get; } = new List<ICSharpInstruction>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# scope.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="curlyBracketsInsertion">The mode to use to write the C# scope..</param>
        /// <param name="endWithBreak">Add a break instruction at the end.</param>
        public virtual void WriteCSharp(ICSharpWriter writer, CSharpCurlyBracketsInsertions curlyBracketsInsertion, bool endWithBreak)
        {
            Debug.Assert(WriteDown);

            bool UseCurlyBrackets = false;

            if (curlyBracketsInsertion.HasFlag(CSharpCurlyBracketsInsertions.Mandatory))
                UseCurlyBrackets = true;

            /*
            List<AttachmentAlias> AttachmentVariableTable = new List<AttachmentAlias>();
            foreach (IInstruction Item in InstructionList)
                Item.AddAttachmentVariables(Context, AttachmentVariableTable);
            */

            if (EntityDeclarationList.Count > 0/* || AttachmentVariableTable.Count > 0*/ || InstructionList.Count != 1)
                UseCurlyBrackets = true;

            if (curlyBracketsInsertion.HasFlag(CSharpCurlyBracketsInsertions.AlreadyInserted))
                UseCurlyBrackets = false;

            if (UseCurlyBrackets)
                writer.WriteIndentedLine("{");
            writer.IncreaseIndent();

            foreach (ICSharpScopeAttributeFeature Item in EntityDeclarationList)
                Item.WriteCSharp(writer);

            /*
            foreach (AttachmentAlias AliasItem in AttachmentVariableTable)
            {
                string AttachedVariableName = AliasItem.EntityName;
                string AttachmentTypeString = CSharpTypes.Type2CSharpString(AliasItem.EntityType, Context, AliasItem.AttachmentFormat, CSharpNamespaceFormats.None);

                writer.WriteIndentedLine(AttachmentTypeString + " " + AttachedVariableName + ";");
                Context.AttachmentVariableTable.Add(AliasItem);
            }
            */

            if (EntityDeclarationList.Count > 0/* || AttachmentVariableTable.Count > 0*/)
                writer.WriteEmptyLine();

            foreach (ICSharpInstruction Item in InstructionList)
                Item.WriteCSharp(writer);

            if (endWithBreak)
                writer.WriteIndentedLine("break;");

            /*
            foreach (AttachmentAlias AliasItem in AttachmentVariableTable)
                Context.AttachmentVariableTable.Remove(AliasItem);
            */

            writer.DecreaseIndent();
            if (UseCurlyBrackets)
                writer.WriteIndentedLine("}");
        }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// True if the node should be produced.
        /// </summary>
        public bool WriteDown { get; private set; }

        /// <summary>
        /// Sets the <see cref="WriteDown"/> flag.
        /// </summary>
        public void SetWriteDown()
        {
            if (WriteDown)
                return;

            WriteDown = true;

            foreach (ICSharpScopeAttributeFeature Item in EntityDeclarationList)
                Item.SetWriteDown();

            foreach (ICSharpInstruction Instruction in InstructionList)
                Instruction.SetWriteDown();
        }
        #endregion
    }
}
