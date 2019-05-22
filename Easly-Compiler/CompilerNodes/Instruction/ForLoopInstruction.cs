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
        /// <summary>
        /// Replicated list from <see cref="BaseNode.ForLoopInstruction.EntityDeclarationBlocks"/>.
        /// </summary>
        IList<IEntityDeclaration> EntityDeclarationList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.ForLoopInstruction.InitInstructionBlocks"/>.
        /// </summary>
        IList<IInstruction> InitInstructionList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.ForLoopInstruction.LoopInstructionBlocks"/>.
        /// </summary>
        IList<IInstruction> LoopInstructionList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.ForLoopInstruction.IterationInstructionBlocks"/>.
        /// </summary>
        IList<IInstruction> IterationInstructionList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.ForLoopInstruction. InvariantBlocks"/>.
        /// </summary>
        IList<IAssertion> InvariantList { get; }
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
        public IOverload EmbeddingOverload { get; private set; }

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
            EmbeddingOverload = parentSource is IOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
            EmbeddingBody = parentSource is IBody AsBody ? AsBody : parentSource?.EmbeddingBody;
            EmbeddingAssertion = parentSource is IAssertion AsAssertion ? AsAssertion : parentSource?.EmbeddingAssertion;
        }

        /// <summary>
        /// Reset some intermediate results.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to reset.</param>
        public void Reset(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                LocalScope = new HashtableEx<string, IScopeAttributeFeature>();
                InnerScopes = new List<IScopeHolder>();
                FullScope = new HashtableEx<string, IScopeAttributeFeature>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ResolvedResult = new OnceReference<IResultType>();
                ResolvedException = new OnceReference<IResultException>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsResolved = false;

            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = LocalScope.IsSealed;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ResolvedResult.IsAssigned;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IInstruction
        /// <summary>
        /// Types of results of the instruction.
        /// </summary>
        public OnceReference<IResultType> ResolvedResult { get; private set; } = new OnceReference<IResultType>();

        /// <summary>
        /// List of exceptions the instruction can throw.
        /// </summary>
        public OnceReference<IResultException> ResolvedException { get; private set; } = new OnceReference<IResultException>();
        #endregion

        #region Implementation of IScopeHolder
        /// <summary>
        /// Entities local to a scope.
        /// </summary>
        public IHashtableEx<string, IScopeAttributeFeature> LocalScope { get; private set; } = new HashtableEx<string, IScopeAttributeFeature>();

        /// <summary>
        /// List of scopes containing the current instance.
        /// </summary>
        public IList<IScopeHolder> InnerScopes { get; private set; } = new List<IScopeHolder>();

        /// <summary>
        /// All reachable entities.
        /// </summary>
        public IHashtableEx<string, IScopeAttributeFeature> FullScope { get; private set; } = new HashtableEx<string, IScopeAttributeFeature>();
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the instruction.
        /// </summary>
        public string InstructionToString { get { return "for... "; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"For Loop Instruction '{InstructionToString}'";
        }
        #endregion
    }
}
