namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# body.
    /// </summary>
    public interface ICSharpEffectiveBody : ICSharpBody
    {
        /// <summary>
        /// The Easly body from which the C# body is created.
        /// </summary>
        new IEffectiveBody Source { get; }

        /// <summary>
        /// The list of instructions in the body.
        /// </summary>
        IList<ICSharpInstruction> BodyInstructionList { get; }

        /// <summary>
        /// The list of require C# assertions.
        /// </summary>
        IList<ICSharpAssertion> RequireList { get; }

        /// <summary>
        /// The list of ensure C# assertions.
        /// </summary>
        IList<ICSharpAssertion> EnsureList { get; }

        /// <summary>
        /// The list of local variables.
        /// </summary>
        IList<ICSharpScopeAttributeFeature> EntityDeclarationList { get; }

        /// <summary>
        /// Writes down the body source code.
        /// </summary>
        /// <param name="writer">The stream on which to write down.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        /// <param name="flags">Some flags.</param>
        /// <param name="resultType">Type of the result, if any.</param>
        /// <param name="skipFirstInstruction">Skip the first instruction.</param>
        /// <param name="initialisationStringList">List of initializations.</param>
        void WriteCSharp(ICSharpWriter writer, string outputNamespace, CSharpBodyFlags flags, string resultType, bool skipFirstInstruction, IList<string> initialisationStringList);
    }

    /// <summary>
    /// A C# body.
    /// </summary>
    public class CSharpEffectiveBody : CSharpBody, ICSharpEffectiveBody
    {
        #region Init
        /// <summary>
        /// Creates a new C# body.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly body from which the C# body is created.</param>
        public static ICSharpEffectiveBody Create(ICSharpContext context, ICSharpFeature parentFeature, IEffectiveBody source)
        {
            return new CSharpEffectiveBody(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpEffectiveBody"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly body from which the C# body is created.</param>
        protected CSharpEffectiveBody(ICSharpContext context, ICSharpFeature parentFeature, IEffectiveBody source)
            : base(context, parentFeature, source)
        {
            foreach (IInstruction Instruction in source.BodyInstructionList)
            {
                ICSharpInstruction NewInstruction = CSharpInstruction.Create(context, Instruction);
                BodyInstructionList.Add(NewInstruction);
            }

            foreach (IAssertion Assertion in source.EnsureList)
            {
                ICSharpAssertion NewAssertion = CSharpAssertion.Create(context, Assertion);
                EnsureList.Add(NewAssertion);
            }

            foreach (IAssertion Assertion in source.RequireList)
            {
                ICSharpAssertion NewAssertion = CSharpAssertion.Create(context, Assertion);
                RequireList.Add(NewAssertion);
            }

            ICSharpClass Owner = parentFeature.Owner;

            foreach (IEntityDeclaration Item in source.EntityDeclarationList)
            {
                ICSharpFeature NewDeclaration = CSharpScopeAttributeFeature.Create(Owner, Item.ValidEntity.Item);
                NewDeclaration.Init(context);

                EntityDeclarationList.Add((ICSharpScopeAttributeFeature)NewDeclaration);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly body from which the C# body is created.
        /// </summary>
        public new IEffectiveBody Source { get { return (IEffectiveBody)base.Source; } }

        /// <summary>
        /// The list of instructions in the body.
        /// </summary>
        public IList<ICSharpInstruction> BodyInstructionList { get; } = new List<ICSharpInstruction>();

        /// <summary>
        /// The list of require C# assertions.
        /// </summary>
        public IList<ICSharpAssertion> RequireList { get; } = new List<ICSharpAssertion>();

        /// <summary>
        /// The list of ensure C# assertions.
        /// </summary>
        public IList<ICSharpAssertion> EnsureList { get; } = new List<ICSharpAssertion>();

        /// <summary>
        /// The list of local variables.
        /// </summary>
        public IList<ICSharpScopeAttributeFeature> EntityDeclarationList { get; } = new List<ICSharpScopeAttributeFeature>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the body source code.
        /// </summary>
        /// <param name="writer">The stream on which to write down.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        /// <param name="flags">Some flags.</param>
        /// <param name="resultType">Type of the result, if any.</param>
        /// <param name="skipFirstInstruction">Skip the first instruction.</param>
        /// <param name="initialisationStringList">List of initializations.</param>
        public virtual void WriteCSharp(ICSharpWriter writer, string outputNamespace, CSharpBodyFlags flags, string resultType, bool skipFirstInstruction, IList<string> initialisationStringList)
        {
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();

            foreach (ICSharpAssertion Assertion in RequireList)
                Assertion.WriteCSharp(writer, outputNamespace);

            if (RequireList.Count > 0)
                writer.WriteLine();

            /*TODO
            List<AttachmentAlias> AttachmentVariableTable = new List<AttachmentAlias>();
            foreach (IInstruction Item in BodyInstructionList)
                Item.AddAttachmentVariables(Context, AttachmentVariableTable);
            */

            if (flags.HasFlag(CSharpBodyFlags.HasResult))
                writer.WriteIndentedLine($"{resultType} Result;");

            foreach (ICSharpScopeAttributeFeature Item in EntityDeclarationList)
                Item.WriteCSharp(writer, outputNamespace);

            /*TODO
            foreach (AttachmentAlias AliasItem in AttachmentVariableTable)
            {
                string AttachedVariableName = AliasItem.EntityName;
                string AttachmentTypeString = CSharpTypes.Type2CSharpString(AliasItem.EntityType, Context, AliasItem.AttachmentFormat, CSharpNamespaceFormats.None);

                writer.WriteIndentedLine(AttachmentTypeString + " " + AttachedVariableName + ";");
                Context.AttachmentVariableTable.Add(AliasItem);
            }
            */

            if (flags.HasFlag(CSharpBodyFlags.HasResult) || EntityDeclarationList.Count > 0/* || AttachmentVariableTable.Count > 0*/)
                writer.WriteLine();

            foreach (string s in initialisationStringList)
                writer.WriteIndentedLine(s);

            if (initialisationStringList.Count > 0)
                writer.WriteLine();

            for (int i = 0; i < BodyInstructionList.Count; i++)
            {
                if (i == 0 && skipFirstInstruction)
                    continue;

                ICSharpInstruction Item = BodyInstructionList[i];
                Item.WriteCSharp(writer, outputNamespace);
            }

            if (EnsureList.Count > 0)
            {
                writer.WriteLine();

                foreach (ICSharpAssertion Assertion in EnsureList)
                    Assertion.WriteCSharp(writer, outputNamespace);

                if (flags.HasFlag(CSharpBodyFlags.HasResult))
                    writer.WriteLine();
            }

            // TODO: ExceptionHandlerList

            if (flags.HasFlag(CSharpBodyFlags.HasResult))
                writer.WriteIndentedLine("return" + " " + "Result" + ";");

            /*TODO
            foreach (AttachmentAlias AliasItem in AttachmentVariableTable)
                Context.AttachmentVariableTable.Remove(AliasItem);
            */

            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");
        }
        #endregion
    }
}
