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
        public void Reset(IRuleTemplateList ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                LocalScope = new SealableDictionary<string, IScopeAttributeFeature>();
                InnerScopes = new List<IScopeHolder>();
                FullScope = new SealableDictionary<string, IScopeAttributeFeature>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                AdditionalScope = new SealableDictionary<string, IScopeAttributeFeature>();
                AdditionalScope.Seal();
                ResolvedResult = new OnceReference<IResultType>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                ResolvedException = new OnceReference<IResultException>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IRuleTemplateList ruleTemplateList)
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
                IsResolved = ResolvedException.IsAssigned;
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
        public ISealableDictionary<string, IScopeAttributeFeature> LocalScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// Additional entities such as loop indexer.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> AdditionalScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// List of scopes containing the current instance.
        /// </summary>
        public IList<IScopeHolder> InnerScopes { get; private set; } = new List<IScopeHolder>();

        /// <summary>
        /// All reachable entities.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> FullScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();
        #endregion

        #region Numbers
        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
            foreach (IEntityDeclaration EntityDeclaration in EntityDeclarationList)
                EntityDeclaration.RestartNumberType(ref isChanged);

            foreach (IInstruction Instruction in InitInstructionList)
                Instruction.RestartNumberType(ref isChanged);

            ((IExpression)WhileCondition).RestartNumberType(ref isChanged);

            foreach (IInstruction Instruction in LoopInstructionList)
                Instruction.RestartNumberType(ref isChanged);

            foreach (IInstruction Instruction in IterationInstructionList)
                Instruction.RestartNumberType(ref isChanged);

            if (Variant.IsAssigned)
                ((IExpression)Variant.Item).RestartNumberType(ref isChanged);
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            foreach (IEntityDeclaration EntityDeclaration in EntityDeclarationList)
                EntityDeclaration.CheckNumberType(ref isChanged);

            foreach (IInstruction Instruction in InitInstructionList)
                Instruction.CheckNumberType(ref isChanged);

            ((IExpression)WhileCondition).CheckNumberType(ref isChanged);

            foreach (IInstruction Instruction in LoopInstructionList)
                Instruction.CheckNumberType(ref isChanged);

            foreach (IInstruction Instruction in IterationInstructionList)
                Instruction.CheckNumberType(ref isChanged);

            if (Variant.IsAssigned)
                ((IExpression)Variant.Item).CheckNumberType(ref isChanged);
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
            foreach (IEntityDeclaration EntityDeclaration in EntityDeclarationList)
                EntityDeclaration.ValidateNumberType(errorList);

            foreach (IInstruction Instruction in InitInstructionList)
                Instruction.ValidateNumberType(errorList);

            ((IExpression)WhileCondition).ValidateNumberType(errorList);

            foreach (IInstruction Instruction in LoopInstructionList)
                Instruction.ValidateNumberType(errorList);

            foreach (IInstruction Instruction in IterationInstructionList)
                Instruction.ValidateNumberType(errorList);

            if (Variant.IsAssigned)
                ((IExpression)Variant.Item).ValidateNumberType(errorList);
        }
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
