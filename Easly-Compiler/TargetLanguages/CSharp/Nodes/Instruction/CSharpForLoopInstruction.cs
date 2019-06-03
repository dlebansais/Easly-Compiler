namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpForLoopInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IForLoopInstruction Source { get; }

        /// <summary>
        /// The list of local entities.
        /// </summary>
        IList<ICSharpScopeAttributeFeature> EntityDeclarationList { get; }

        /// <summary>
        /// The list of initialization instructions.
        /// </summary>
        IList<ICSharpInstruction> InitInstructionList { get; }

        /// <summary>
        /// The loop condition.
        /// </summary>
        ICSharpExpression WhileCondition { get; }

        /// <summary>
        /// The list of loop instructions.
        /// </summary>
        IList<ICSharpInstruction> LoopInstructionList { get; }

        /// <summary>
        /// The list of iteration instructions.
        /// </summary>
        IList<ICSharpInstruction> IterationInstructionList { get; }

        /// <summary>
        /// The loop variant. Can be null.
        /// </summary>
        ICSharpExpression VariantExpression { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpForLoopInstruction : CSharpInstruction, ICSharpForLoopInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpForLoopInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IForLoopInstruction source)
        {
            return new CSharpForLoopInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpForLoopInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpForLoopInstruction(ICSharpContext context, ICSharpFeature parentFeature, IForLoopInstruction source)
            : base(context, parentFeature, source)
        {
            foreach (IEntityDeclaration Declaration in source.EntityDeclarationList)
            {
                ICSharpScopeAttributeFeature NewDeclaration = CSharpScopeAttributeFeature.Create(context, parentFeature.Owner, Declaration.ValidEntity.Item);
                EntityDeclarationList.Add(NewDeclaration);
            }

            foreach (IInstruction Instruction in source.InitInstructionList)
            {
                ICSharpInstruction NewInstruction = Create(context, parentFeature, Instruction);
                InitInstructionList.Add(NewInstruction);
            }

            WhileCondition = CSharpExpression.Create(context, (IExpression)source.WhileCondition);

            foreach (IInstruction Instruction in source.LoopInstructionList)
            {
                ICSharpInstruction NewInstruction = Create(context, parentFeature, Instruction);
                LoopInstructionList.Add(NewInstruction);
            }

            foreach (IInstruction Instruction in source.IterationInstructionList)
            {
                ICSharpInstruction NewInstruction = Create(context, parentFeature, Instruction);
                IterationInstructionList.Add(NewInstruction);
            }

            if (source.Variant.IsAssigned)
                VariantExpression = CSharpExpression.Create(context, (IExpression)source.Variant.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IForLoopInstruction Source { get { return (IForLoopInstruction)base.Source; } }

        /// <summary>
        /// The list of local entities.
        /// </summary>
        public IList<ICSharpScopeAttributeFeature> EntityDeclarationList { get; } = new List<ICSharpScopeAttributeFeature>();

        /// <summary>
        /// The list of initialization instructions.
        /// </summary>
        public IList<ICSharpInstruction> InitInstructionList { get; } = new List<ICSharpInstruction>();

        /// <summary>
        /// The loop condition.
        /// </summary>
        public ICSharpExpression WhileCondition { get; }

        /// <summary>
        /// The list of loop instructions.
        /// </summary>
        public IList<ICSharpInstruction> LoopInstructionList { get; } = new List<ICSharpInstruction>();

        /// <summary>
        /// The list of iteration instructions.
        /// </summary>
        public IList<ICSharpInstruction> IterationInstructionList { get; } = new List<ICSharpInstruction>();

        /// <summary>
        /// The loop variant. Can be null.
        /// </summary>
        public ICSharpExpression VariantExpression { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        public override void WriteCSharp(ICSharpWriter writer, string outputNamespace)
        {
            bool UseCurlyBrackets = false;

            /*TODO
            List<AttachmentAlias> AttachmentVariableTable = new List<AttachmentAlias>();
            foreach (IInstruction Item in InitInstructionList)
                Item.AddAttachmentVariables(Context, AttachmentVariableTable);
            foreach (IInstruction Item in LoopInstructionList)
                Item.AddAttachmentVariables(Context, AttachmentVariableTable);
            foreach (IInstruction Item in IterationInstructionList)
                Item.AddAttachmentVariables(Context, AttachmentVariableTable);
            */

            if (EntityDeclarationList.Count > 0/* || AttachmentVariableTable.Count > 0*/ || InitInstructionList.Count > 1)
                UseCurlyBrackets = true;

            if (UseCurlyBrackets)
            {
                writer.WriteIndentedLine("{");
                writer.IncreaseIndent();
            }

            foreach (ICSharpScopeAttributeFeature Item in EntityDeclarationList)
                Item.WriteCSharp(writer, outputNamespace);

            if (Source.Variant.IsAssigned)
                writer.WriteIndentedLine("double LoopVariant = double.NaN;");

            /*
            foreach (AttachmentAlias AliasItem in AttachmentVariableTable)
            {
                string AttachedVariableName = AliasItem.EntityName;
                string AttachmentTypeString = CSharpTypes.Type2CSharpString(AliasItem.EntityType, Context, AliasItem.AttachmentFormat, CSharpNamespaceFormats.None);

                writer.WriteIndentedLine(AttachmentTypeString + " " + AttachedVariableName + ";");
                Context.AttachmentVariableTable.Add(AliasItem);
            }*/

            if (EntityDeclarationList.Count > 0/* || AttachmentVariableTable.Count > 0*/)
                writer.WriteLine();

            foreach (ICSharpInstruction Item in InitInstructionList)
                Item.WriteCSharp(writer, outputNamespace);

            string WhileString = WhileCondition.CSharpText(outputNamespace);

            writer.WriteIndentedLine($"while ({WhileString})");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();

            foreach (ICSharpInstruction Item in LoopInstructionList)
                Item.WriteCSharp(writer, outputNamespace);

            if (LoopInstructionList.Count > 0 && IterationInstructionList.Count > 0)
                writer.WriteLine();

            foreach (ICSharpInstruction Item in IterationInstructionList)
                Item.WriteCSharp(writer, outputNamespace);

            if (VariantExpression != null)
            {
                string ExpressionText = VariantExpression.CSharpText(outputNamespace);

                writer.WriteIndentedLine($"double NewVariantResult = {ExpressionText};");
                writer.WriteIndentedLine("if (NewVariantResult >= LoopVariant)// Takes advantage of the fact that 'x >= NaN' is always false");
                writer.IncreaseIndent();
                writer.WriteIndentedLine("throw new InvalidOperationException();");
                writer.DecreaseIndent();
                writer.WriteIndentedLine("LoopVariant = NewVariantResult;");
            }

            // TODO: Invariants

            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");

            /*
            foreach (AttachmentAlias AliasItem in AttachmentVariableTable)
                Context.AttachmentVariableTable.Remove(AliasItem);
            */

            if (UseCurlyBrackets)
            {
                writer.DecreaseIndent();
                writer.WriteIndentedLine("}");
            }
        }
        #endregion
    }
}
