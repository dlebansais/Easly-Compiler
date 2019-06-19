namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
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
        /// The list of local variables.
        /// </summary>
        IList<ICSharpScopeAttributeFeature> EntityDeclarationList { get; }

        /// <summary>
        /// Writes down the body source code.
        /// </summary>
        /// <param name="writer">The stream on which to write down.</param>
        /// <param name="flags">Some flags.</param>
        /// <param name="resultType">Type of the result, if any.</param>
        /// <param name="skipFirstInstruction">Skip the first instruction.</param>
        /// <param name="initialisationStringList">List of initializations.</param>
        void WriteCSharp(ICSharpWriter writer, CSharpBodyFlags flags, string resultType, bool skipFirstInstruction, IList<string> initialisationStringList);
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
                ICSharpInstruction NewInstruction = CSharpInstruction.Create(context, parentFeature, Instruction);
                BodyInstructionList.Add(NewInstruction);
            }

            ICSharpClass Owner = parentFeature.Owner;

            foreach (IEntityDeclaration Item in source.EntityDeclarationList)
            {
                ICSharpScopeAttributeFeature NewDeclaration = CSharpScopeAttributeFeature.Create(context, Owner, Item.ValidEntity.Item);
                EntityDeclarationList.Add(NewDeclaration);
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
        /// The list of local variables.
        /// </summary>
        public IList<ICSharpScopeAttributeFeature> EntityDeclarationList { get; } = new List<ICSharpScopeAttributeFeature>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the body source code.
        /// </summary>
        /// <param name="writer">The stream on which to write down.</param>
        /// <param name="flags">Some flags.</param>
        /// <param name="resultType">Type of the result, if any.</param>
        /// <param name="skipFirstInstruction">Skip the first instruction.</param>
        /// <param name="initialisationStringList">List of initializations.</param>
        public virtual void WriteCSharp(ICSharpWriter writer, CSharpBodyFlags flags, string resultType, bool skipFirstInstruction, IList<string> initialisationStringList)
        {
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();

            IList<ICSharpAssertion> EffectiveRequireList = RequireList;
            IList<ICSharpAssertion> EffectiveEnsureList = EnsureList;

            switch (ParentFeature)
            {
                case ICSharpFunctionFeature AsFunctionFeature:
                    if (AsFunctionFeature.OriginalPrecursor != null)
                    {
                        ICSharpQueryOverload ParentOverload = null;
                        foreach (ICSharpQueryOverload Overload in AsFunctionFeature.OverloadList)
                            if (Overload.Body == this)
                            {
                                ParentOverload = Overload;
                                break;
                            }

                        Debug.Assert(ParentOverload != null);

                        ICSharpQueryOverload ParentPrecursorOverload = ParentOverload.Precursor;
                        if (ParentPrecursorOverload != null)
                        {
                            ICSharpBody PrecursorBody = ParentPrecursorOverload.Body;

                            if (RequireList.Count == 0 && PrecursorBody.RequireList.Count > 0)
                                EffectiveRequireList = PrecursorBody.RequireList;

                            if (EnsureList.Count == 0 && PrecursorBody.EnsureList.Count > 0)
                                EffectiveEnsureList = PrecursorBody.EnsureList;
                        }
                    }
                    break;

                case ICSharpProcedureFeature AsProcedureFeature:
                    if (AsProcedureFeature.OriginalPrecursor != null)
                    {
                        ICSharpCommandOverload ParentOverload = null;
                        foreach (ICSharpCommandOverload Overload in AsProcedureFeature.OverloadList)
                            if (Overload.Body == this)
                            {
                                ParentOverload = Overload;
                                break;
                            }

                        Debug.Assert(ParentOverload != null);

                        ICSharpCommandOverload ParentPrecursorOverload = ParentOverload.Precursor;
                        if (ParentPrecursorOverload != null)
                        {
                            ICSharpBody PrecursorBody = ParentPrecursorOverload.Body;

                            if (RequireList.Count == 0 && PrecursorBody.RequireList.Count > 0)
                                EffectiveRequireList = PrecursorBody.RequireList;

                            if (EnsureList.Count == 0 && PrecursorBody.EnsureList.Count > 0)
                                EffectiveEnsureList = PrecursorBody.EnsureList;
                        }
                    }
                    break;

                case ICSharpPropertyFeature AsPropertyFeature:
                    if (AsPropertyFeature.OriginalPrecursor != null)
                    {
                        ICSharpBody PrecursorBody = null;

                        if (this == AsPropertyFeature.GetterBody)
                            PrecursorBody = AsPropertyFeature.OriginalPrecursor.GetterBody;
                        else if (this == AsPropertyFeature.SetterBody)
                            PrecursorBody = AsPropertyFeature.OriginalPrecursor.SetterBody;

                        if (PrecursorBody != null)
                        {
                            if (RequireList.Count == 0 && PrecursorBody.RequireList.Count > 0)
                                EffectiveRequireList = PrecursorBody.RequireList;

                            if (EnsureList.Count == 0 && PrecursorBody.EnsureList.Count > 0)
                                EffectiveEnsureList = PrecursorBody.EnsureList;
                        }
                    }
                    break;

                case ICSharpIndexerFeature AsIndexerFeature:
                    if (AsIndexerFeature.OriginalPrecursor != null)
                    {
                        ICSharpBody PrecursorBody = null;

                        if (this == AsIndexerFeature.GetterBody)
                            PrecursorBody = AsIndexerFeature.OriginalPrecursor.GetterBody;
                        else if (this == AsIndexerFeature.SetterBody)
                            PrecursorBody = AsIndexerFeature.OriginalPrecursor.SetterBody;

                        if (PrecursorBody != null)
                        {
                            if (RequireList.Count == 0 && PrecursorBody.RequireList.Count > 0)
                                EffectiveRequireList = PrecursorBody.RequireList;

                            if (EnsureList.Count == 0 && PrecursorBody.EnsureList.Count > 0)
                                EffectiveEnsureList = PrecursorBody.EnsureList;
                        }
                    }
                    break;
            }

            foreach (ICSharpAssertion Assertion in EffectiveRequireList)
                Assertion.WriteCSharp(writer);

            if (EffectiveRequireList.Count > 0)
                writer.WriteEmptyLine();

            /*TODO
            List<AttachmentAlias> AttachmentVariableTable = new List<AttachmentAlias>();
            foreach (IInstruction Item in BodyInstructionList)
                Item.AddAttachmentVariables(Context, AttachmentVariableTable);
            */

            if (flags.HasFlag(CSharpBodyFlags.HasResult))
                writer.WriteIndentedLine($"{resultType} Result = default;");

            foreach (ICSharpScopeAttributeFeature Item in EntityDeclarationList)
                Item.WriteCSharp(writer);

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
                writer.WriteEmptyLine();

            foreach (string s in initialisationStringList)
                writer.WriteIndentedLine(s);

            if (initialisationStringList.Count > 0)
                writer.WriteEmptyLine();

            for (int i = 0; i < BodyInstructionList.Count; i++)
            {
                if (i == 0 && skipFirstInstruction)
                    continue;

                ICSharpInstruction Item = BodyInstructionList[i];
                Item.WriteCSharp(writer);
            }

            if (EffectiveEnsureList.Count > 0)
            {
                writer.WriteEmptyLine();

                foreach (ICSharpAssertion Assertion in EffectiveEnsureList)
                    Assertion.WriteCSharp(writer);

                if (flags.HasFlag(CSharpBodyFlags.HasResult))
                    writer.WriteEmptyLine();
            }

            // TODO: ExceptionHandlerList

            if (ParentFeature.Owner.InvariantList.Count > 0)
            {
                writer.WriteEmptyLine();
                writer.WriteIndentedLine("CheckInvariant();");
            }

            if (flags.HasFlag(CSharpBodyFlags.HasResult))
                writer.WriteIndentedLine("return Result;");

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
