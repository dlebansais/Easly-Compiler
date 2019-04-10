namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IForLoopInstruction.
    /// </summary>
    public interface IForLoopInstruction : BaseNode.IForLoopInstruction, IInstruction, INodeWithReplicatedBlocks
    {
    }

    /// <summary>
    /// Compiler IForLoopInstruction.
    /// </summary>
    public class ForLoopInstruction : BaseNode.ForLoopInstruction, IForLoopInstruction
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.ForLoopInstruction.EntityDeclarationBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> EntityDeclarationList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.ForLoopInstruction.InitInstructionBlocks"/>.
        /// </summary>
        public IList<IInstruction> InitInstructionList { get; } = new List<IInstruction>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.ForLoopInstruction.LoopInstructionBlocks"/>.
        /// </summary>
        public IList<IInstruction> LoopInstructionList { get; } = new List<IInstruction>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.ForLoopInstruction.IterationInstructionBlocks"/>.
        /// </summary>
        public IList<IInstruction> IterationInstructionList { get; } = new List<IInstruction>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.ForLoopInstruction. InvariantBlocks"/>.
        /// </summary>
        public IList<IAssertion> InvariantList { get; } = new List<IAssertion>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyEntityDeclaration">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyEntityDeclaration, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyEntityDeclaration)
            {
                case nameof(EntityDeclarationBlocks):
                    TargetList = (IList)EntityDeclarationList;
                    break;

                case nameof(InitInstructionBlocks):
                    TargetList = (IList)InitInstructionList;
                    break;

                case nameof(LoopInstructionBlocks):
                    TargetList = (IList)LoopInstructionList;
                    break;

                case nameof(IterationInstructionBlocks):
                    TargetList = (IList)IterationInstructionList;
                    break;

                case nameof(InvariantBlocks):
                    TargetList = (IList)InvariantList;
                    break;
            }

            Debug.Assert(TargetList != null);
            Debug.Assert(TargetList.Count == 0);

            foreach (BaseNode.INode Node in nodeList)
                TargetList.Add(Node);
        }
        #endregion

        #region Implementation of ISource
        /// <summary>
        /// The parent node, null if root.
        /// </summary>
        public ISource ParentSource { get; private set; }

        /// <summary>
        /// The parent class, null if none.
        /// </summary>
        public IClass EmbeddingClass { get; private set; }

        /// <summary>
        /// The parent feature, null if none.
        /// </summary>
        public IFeature EmbeddingFeature { get; private set; }

        /// <summary>
        /// The parent overload, null if none.
        /// </summary>
        public IQueryOverload EmbeddingOverload { get; private set; }

        /// <summary>
        /// The parent body, null if none.
        /// </summary>
        public IBody EmbeddingBody { get; private set; }

        /// <summary>
        /// The parent assertion, null if none.
        /// </summary>
        public IAssertion EmbeddingAssertion { get; private set; }

        /// <summary>
        /// Initializes parents based on the provided <paramref name="parentSource"/> node.
        /// </summary>
        /// <param name="parentSource">The parent node.</param>
        public virtual void InitializeSource(ISource parentSource)
        {
            ParentSource = parentSource;

            EmbeddingClass = parentSource is IClass AsClass ? AsClass : parentSource?.EmbeddingClass;
            EmbeddingFeature = parentSource is IFeature AsFeature ? AsFeature : parentSource?.EmbeddingFeature;
            EmbeddingOverload = parentSource is IQueryOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
            EmbeddingBody = parentSource is IBody AsBody ? AsBody : parentSource?.EmbeddingBody;
            EmbeddingAssertion = parentSource is IAssertion AsAssertion ? AsAssertion : parentSource?.EmbeddingAssertion;
        }

        /// <summary>
        /// Reset some intermediate results.
        /// </summary>
        /// <param name="engine">The engine requesting reset.</param>
        public void Reset(InferenceEngine engine)
        {
            bool IsHandled = false;

            if (engine.RuleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }
        #endregion
    }
}
