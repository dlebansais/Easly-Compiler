namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpOverLoopInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IOverLoopInstruction Source { get; }

        /// <summary>
        /// The loop source.
        /// </summary>
        ICSharpExpression OverList { get; }

        /// <summary>
        /// The list of indexers.
        /// </summary>
        IList<ICSharpScopeAttributeFeature> IndexerList { get; }

        /// <summary>
        /// Loop instructions.
        /// </summary>
        ICSharpScope LoopInstructions { get; }

        /// <summary>
        /// The exit entity. Can be null.
        /// </summary>
        string ExitEntityName { get; }

        /// <summary>
        /// The list of loop invariants.
        /// </summary>
        IList<ICSharpAssertion> InvariantList { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpOverLoopInstruction : CSharpInstruction, ICSharpOverLoopInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpOverLoopInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IOverLoopInstruction source)
        {
            return new CSharpOverLoopInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpOverLoopInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpOverLoopInstruction(ICSharpContext context, ICSharpFeature parentFeature, IOverLoopInstruction source)
            : base(context, parentFeature, source)
        {
            OverList = CSharpExpression.Create(context, (IExpression)source.OverList);
            LoopInstructions = CSharpScope.Create(context, parentFeature, (IScope)source.LoopInstructions);

            foreach (IName Name in source.IndexerList)
            {
                string IndexerName = Name.ValidText.Item;
                IScopeAttributeFeature IndexerFeature = Source.InnerLoopScope[IndexerName];

                ICSharpScopeAttributeFeature NewIndexer = CSharpScopeAttributeFeature.Create(context, ParentFeature.Owner, IndexerFeature);
                IndexerList.Add(NewIndexer);
            }

            if (source.ExitEntityName.IsAssigned)
                ExitEntityName = ((IIdentifier)source.ExitEntityName.Item).ValidText.Item;

            foreach (IAssertion Item in Source.InvariantList)
            {
                ICSharpAssertion NewAssertion = CSharpAssertion.Create(context, Item);
                InvariantList.Add(NewAssertion);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IOverLoopInstruction Source { get { return (IOverLoopInstruction)base.Source; } }

        /// <summary>
        /// The loop source.
        /// </summary>
        public ICSharpExpression OverList { get; }

        /// <summary>
        /// The list of indexers.
        /// </summary>
        public IList<ICSharpScopeAttributeFeature> IndexerList { get; } = new List<ICSharpScopeAttributeFeature>();

        /// <summary>
        /// Loop instructions.
        /// </summary>
        public ICSharpScope LoopInstructions { get; }

        /// <summary>
        /// The exit entity. Can be null.
        /// </summary>
        public string ExitEntityName { get; }

        /// <summary>
        /// The list of loop invariants.
        /// </summary>
        public IList<ICSharpAssertion> InvariantList { get; } = new List<ICSharpAssertion>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public override void WriteCSharp(ICSharpWriter writer)
        {
            Debug.Assert(WriteDown);

            ICSharpExpressionContext SourceExpressionContext = new CSharpExpressionContext();
            OverList.WriteCSharp(writer, SourceExpressionContext, -1);

            string OverListString = SourceExpressionContext.ReturnValue;

            ICSharpScopeAttributeFeature Indexer = IndexerList[0];
            string IndexerNameString = Indexer.Name;
            string TypeString = Indexer.Type.Type2CSharpString(writer, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);

            //TODO: support multiple indexers and IterationType

            if (ExitEntityName != null)
            {
                string ExitEntityNameString = CSharpNames.ToCSharpIdentifier(ExitEntityName);

                writer.WriteIndentedLine($"{ExitEntityNameString} = false;");
                writer.WriteIndentedLine($"foreach ({TypeString} {IndexerNameString} in {OverListString})");

                writer.WriteIndentedLine("{");

                LoopInstructions.WriteCSharp(writer, CSharpCurlyBracketsInsertions.AlreadyInserted, false);

                WriteCSharpInvariant(writer);

                writer.WriteEmptyLine();
                writer.IncreaseIndent();
                writer.WriteIndentedLine($"if ({ExitEntityNameString})");
                writer.IncreaseIndent();
                writer.WriteIndentedLine("break;");
                writer.DecreaseIndent();
                writer.DecreaseIndent();
                writer.WriteIndentedLine("}");
            }
            else if (InvariantList.Count > 0)
            {
                writer.WriteIndentedLine($"foreach ({TypeString} {IndexerNameString} in {OverListString})");

                writer.WriteIndentedLine("{");
                LoopInstructions.WriteCSharp(writer, CSharpCurlyBracketsInsertions.AlreadyInserted, false);

                writer.IncreaseIndent();
                WriteCSharpInvariant(writer);
                writer.DecreaseIndent();

                writer.WriteIndentedLine("}");
            }
            else
            {
                writer.WriteIndentedLine($"foreach ({TypeString} {IndexerNameString} in {OverListString})");

                LoopInstructions.WriteCSharp(writer, CSharpCurlyBracketsInsertions.Indifferent, false);
            }

            //TODO: InvariantBlocks
            //TODO: Variant
        }

        private void WriteCSharpInvariant(ICSharpWriter writer)
        {
            if (InvariantList.Count > 0)
                writer.WriteEmptyLine();

            foreach (ICSharpAssertion Assertion in InvariantList)
                Assertion.WriteCSharp(writer);
        }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// Sets the <see cref="ICSharpOutputNode.WriteDown"/> flag.
        /// </summary>
        public override void SetWriteDown()
        {
            if (WriteDown)
                return;

            WriteDown = true;

            OverList.SetWriteDown();

            foreach (ICSharpScopeAttributeFeature Item in IndexerList)
                Item.SetWriteDown();

            LoopInstructions.SetWriteDown();

            foreach (ICSharpAssertion Assertion in InvariantList)
                Assertion.SetWriteDown();
        }
        #endregion
    }
}
